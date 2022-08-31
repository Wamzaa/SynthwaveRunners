using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLightGenerator : MonoBehaviour
{
    public float scale;
    public float gapLength;
    public Material mat;

    private void Start()
    {
        GenerateRoadLight();
    }

    private void GenerateRoadLight()
    {
        Quaternion rot = this.transform.rotation;
        this.transform.rotation = Quaternion.identity;
        
        Vector3 tempSize; 

        tempSize = scale * new Vector3(0.3f, 1.0f, 0.3f);
        CubeGenerator.SetupCubeMesh("base(RoadLight)", this.transform, this.transform.position + scale * new Vector3(0.0f, 0.5f, 0.0f), tempSize , gapLength, mat);

        tempSize = scale * new Vector3(0.15f, 2.2f, 0.15f);
        CubeGenerator.SetupCubeMesh("arm(RoadLight)", this.transform, this.transform.position + scale * new Vector3(0.0f, 2.0f, 0.0f), tempSize , gapLength, mat);

        tempSize = scale * new Vector3(0.35f, 0.2f, 0.8f);
        CubeGenerator.SetupCubeMesh("head(RoadLight)", this.transform, this.transform.position + scale * new Vector3(0.0f, 3.2f, 0.25f), tempSize , gapLength, mat);

        tempSize = scale * new Vector3(0.25f, 0.12f, 0.45f);
        CubeGenerator.SetupCubeMesh("light(RoadLight)", this.transform, this.transform.position + scale * new Vector3(0.0f, 3.1f, 0.35f), tempSize , scale * 0.45f, mat);


        this.transform.rotation = rot;
    }
}
