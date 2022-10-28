using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float currentSpeed;
    private List<Speed> listSpeed = new List<Speed>();

    public float bestHeight;
    public float firePower;
    public float frictionCoefficient;
    public float jumpForce;

    public float rotationLookSpeed;

    private Rigidbody rb;
    private Camera cam;

    private Vector3 targetVelocity;
    private Vector3 fireForce;
    private bool isJumping;

    private Vector3 currentNormal;

    private float rotY;
    private float currentAngle = 0.0f;
    private float floorAdaptationTime = 0.2f;

    private float speedTimer = 0f;
    private int currentSpeedIndex = 0;

    //private LineRenderer meanRenderer;
    //private LineRenderer currentRenderer;

    private void Start()
    {
        currentSpeed = 0f;
        rb = this.GetComponent<Rigidbody>();
        cam = Camera.main;
        currentNormal = Vector3.up;
        isJumping = false;

        listSpeed.Add(new Speed(1f, 0, 10));
        listSpeed.Add(new Speed(2.5f, 10, 60));
        listSpeed.Add(new Speed(4f, 60, 100));
        listSpeed.Add(new Speed(7f, 100, 150));
        listSpeed.Add(new Speed(10f, 150, 200));

        /*GameObject meanObj = new GameObject();
        meanRenderer = meanObj.AddComponent<LineRenderer>();
        meanRenderer.endColor = Color.green;
        meanRenderer.startWidth = 0.01f;
        meanRenderer.endWidth = 0.01f;
        GameObject currentObj = new GameObject();
        currentRenderer = currentObj.AddComponent<LineRenderer>();
        currentRenderer.endColor = Color.red;
        currentRenderer.startWidth = 0.01f;
        currentRenderer.endWidth = 0.01f;*/
    }

    private void Update()
    {
        //float movY = Input.GetAxisRaw("Vertical");
        bool movY = Input.GetButton("Jump");

        if (Input.GetKeyDown(KeyCode.M))
        {
            isJumping = true;
        }

        if(rb.velocity.magnitude < 0.5 * targetVelocity.magnitude)
        {
            ChangeSpeed(Mathf.Max(-2f * targetVelocity.magnitude/rb.velocity.magnitude , -20.0f));
        }
        else
        {
            if (movY)
            {
                ChangeSpeed(1.0f);
            }
            else
            {
                ChangeSpeed(-2.5f);
            }
        }

        if(Mathf.Abs(rotY) > 0.25 * rotationLookSpeed)
        {
            ChangeSpeed(-0.7f);
        }

        Vector3 playerForward = rb.velocity.normalized;
        Vector3 playerRight = Vector3.Cross(currentNormal, playerForward);
        playerRight = playerRight.normalized;
        float coefOffRay = 1.0f;

        Bounds playerBounds = this.GetComponentInChildren<Collider>().bounds;
        float minSize = Mathf.Min(playerBounds.size.x, playerBounds.size.z); 

        Vector3[] listOriginOffsetRaycast = new Vector3[]
        {
            Vector3.zero, minSize*playerForward, minSize*playerForward + minSize*playerRight, minSize*playerRight, -minSize*playerForward + minSize*playerRight, -minSize*playerForward, - minSize*playerForward - minSize*playerRight, -minSize*playerRight, minSize*playerForward - minSize*playerRight
        };
        Vector3 meanNormal = Vector3.zero;
        RaycastHit hitOffDir;
        foreach (Vector3 off in listOriginOffsetRaycast)
        {
            if (Physics.Raycast(this.transform.position + off, -currentNormal, out hitOffDir, Mathf.Infinity, LayerMask.GetMask("Floor")))
            {
                meanNormal = meanNormal + hitOffDir.normal;
            }
        }
        meanNormal = meanNormal.normalized;

        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, -currentNormal, out hit, Mathf.Infinity, LayerMask.GetMask("Floor"))){
            if(hit.distance > 200f)
            {
                currentNormal = Vector3.up;
            }
            else
            {
                /*meanRenderer.SetPositions(new Vector3[] { this.transform.position, this.transform.position + currentNormal });
                currentRenderer.SetPositions(new Vector3[] { this.transform.position, this.transform.position + meanNormal });*/
                //currentNormal = hit.normal;
                currentNormal = meanNormal;
            }

            if(bestHeight > hit.distance)
            {
                fireForce = currentNormal * (bestHeight - hit.distance) * firePower - frictionCoefficient * currentNormal * Vector3.Dot(rb.velocity, currentNormal);
            }
            else
            {
                fireForce = currentNormal * (bestHeight - hit.distance) * firePower - frictionCoefficient * currentNormal * Vector3.Dot(rb.velocity, currentNormal);
            }
            //Debug.Log(fireForce.magnitude);

            AdaptToFloor();
        }
        else
        {
            currentNormal = Vector3.up;
            fireForce = -currentNormal * 100f * firePower;
        }
        

        float lookMovX = Input.GetAxisRaw("Mouse X");

        //rotY = rotationLookSpeed * lookMovX;
        rotY = Input.GetAxisRaw("Horizontal") * rotationLookSpeed;
        TurnCamera(rotY > 0, rotY < 0);
    }

    private void FixedUpdate()
    {
        Vector3 horizontalVelocity = targetVelocity;

        if (isJumping)
        {
            rb.AddForce(jumpForce * currentNormal);
            isJumping = false;
        }

        rb.AddForce(fireForce);

        //rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        rb.velocity = Vector3.Dot(rb.velocity, this.transform.up) * this.transform.up + targetVelocity;

        this.transform.Rotate(new Vector3(0f, rotY, 0f));
    }

    public void ChangeSpeed(float coeff)
    {
        speedTimer += coeff * Time.deltaTime;
        if (speedTimer >= listSpeed[currentSpeedIndex].duration)
        {
            if (currentSpeedIndex < listSpeed.Count - 1)
            {
                speedTimer = 0f;
                currentSpeedIndex += 1;
                currentSpeed = listSpeed[currentSpeedIndex].minSpeed;
            }
            else
            {
                currentSpeed = listSpeed[currentSpeedIndex].maxSpeed;
                speedTimer = listSpeed[currentSpeedIndex].duration;
            }

        }
        else if (speedTimer <= 0f)
        {
            if (currentSpeedIndex > 0)
            {
                currentSpeedIndex -= 1;
                speedTimer = listSpeed[currentSpeedIndex].duration;
                currentSpeed = listSpeed[currentSpeedIndex].maxSpeed;
            }
            else
            {
                speedTimer = 0f;
                currentSpeedIndex = 0;
            }
        }
        else
        {
            float percent = speedTimer / listSpeed[currentSpeedIndex].duration;
            currentSpeed = Mathf.Lerp(listSpeed[currentSpeedIndex].minSpeed, listSpeed[currentSpeedIndex].maxSpeed, percent);
            UIManager.Instance.UpdateSpeed(currentSpeed, percent, currentSpeedIndex);
        }

        targetVelocity = currentSpeed * this.transform.forward;
    }

    public void AdaptToFloor()
    {
        float scaZ = Vector3.Dot(currentNormal, this.transform.right);
        float scaX = Vector3.Dot(currentNormal, this.transform.forward);
      
        float angleZ = Mathf.Asin(scaZ) * (180.0f / Mathf.PI) * Time.deltaTime / floorAdaptationTime;
        float angleX = Mathf.Asin(scaX) * (180.0f / Mathf.PI) * Time.deltaTime / floorAdaptationTime;

        this.transform.Rotate(0f, 0f, -angleZ);
        this.transform.Rotate(angleX, 0f, 0f);
    }

    private void TurnCamera(bool turnRight, bool turnLeft)
    {
        float oldAngle = currentAngle;
        if (turnRight)
        {
            if (currentAngle < 20f)
            {
                currentAngle += 0.05f;
            }
            else
            {
                currentAngle = 20f;
            }
        }
        else if(turnLeft)
        {
            if (currentAngle > -20f)
            {
                currentAngle -= 0.05f;
            }
            else
            {
                currentAngle = -20f;
            }
        }
        else
        {
            if(currentAngle < 0.0f)
            {
                currentAngle += 0.05f;
            }
            if (currentAngle > 0.0f)
            {
                currentAngle -= 0.05f;
            }
        }
        cam.gameObject.transform.Rotate(0f, 0f, (currentAngle - oldAngle));
    }

}
