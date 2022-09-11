using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public enum GeneratorMode { Tunnel, Road }
    public enum RoadMode { TornadoRoad, FlatRoad}

    [Header ("--- Mode ---")]
    public GeneratorMode mode;

    public RoadMode roadMode;

    [Header ("--- Control Points ---")]
    public List<MeshControlPoint> controlPointsList;

    [Header ("--- Mode Settings ---")]
    public int resolution;
    public int pointResolution;
    public float initialRoadRotation;

    [Header ("--- Materials ---")]
    public Material mat;
    public Material edgeMat;

    [Header("--- Decoration ---")]
    public bool withRoadLight;
    public float roadLightScale;
    public float roadLightInter;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector3[] normals;
    private Vector2[] uvs;


    private void Start()
    {
        if(controlPointsList != null)
        {
            SetUp();
        }
        
    }

    public void SetUp()
    {
        switch (mode)
        {
            case GeneratorMode.Tunnel:
                BuildTunnel();
                break;
            case GeneratorMode.Road:
                switch (roadMode)
                {
                    case RoadMode.TornadoRoad:
                        BuildTornadoRoad();
                        break;
                    case RoadMode.FlatRoad:
                        BuildFlatRoad();
                        break;
                }
                break;
            default:
                break;
        }
    }

    private void BuildTunnel()
    {
        List<Vector3> velocityList = new List<Vector3>();

        Vector3 vel = controlPointsList[1].transform.position - controlPointsList[0].transform.position;
        //vel = vel.normalized;
        velocityList.Add(0.5f*vel);
        for (int i = 1; i < controlPointsList.Count - 1; i++)
        {
            vel = controlPointsList[i + 1].transform.position - controlPointsList[i - 1].transform.position;
            //vel = vel.normalized;
            velocityList.Add(0.5f * vel);
        }
        vel = controlPointsList[controlPointsList.Count - 1].transform.position - controlPointsList[controlPointsList.Count - 2].transform.position;
        //vel = vel.normalized;
        velocityList.Add(0.5f * vel);

        GameObject interpolatedControlPointsParent = new GameObject("InterpolatedControlPointsParent");
        List<MeshControlPoint> controlPoints = new List<MeshControlPoint>();
        controlPoints.Add(controlPointsList[0]);
        for(int i=1; i<controlPointsList.Count; i++)
        {
            List<Vector3> listInterpolatedPoints = GetHermitInterpolationPoints(controlPointsList[i - 1].transform.position, velocityList[i - 1], controlPointsList[i].transform.position, velocityList[i], pointResolution);
            foreach(Vector3 vec in listInterpolatedPoints)
            {
                GameObject interpolatedControlPoint = new GameObject("interpolatedControlPoint");
                interpolatedControlPoint.transform.position = vec;
                interpolatedControlPoint.transform.parent = interpolatedControlPointsParent.transform;
                MeshControlPoint meshControlPoint = interpolatedControlPoint.AddComponent<MeshControlPoint>();
                meshControlPoint.internRadius = controlPointsList[i].internRadius;
                meshControlPoint.externRadius = controlPointsList[i].externRadius;

                controlPoints.Add(meshControlPoint);
            }
            controlPoints.Add(controlPointsList[i]);
        }


        List<Vector3> controlPointsOrientation = new List<Vector3>();
        
        Vector3 dir = controlPoints[1].transform.position - controlPoints[0].transform.position;
        dir = dir.normalized;
        controlPointsOrientation.Add(dir);
        for(int i = 1; i < controlPoints.Count-1; i++)
        {
            dir = controlPoints[i+1].transform.position - controlPoints[i-1].transform.position;
            dir = dir.normalized;
            controlPointsOrientation.Add(dir);
        }
        dir = controlPoints[controlPoints.Count-1].transform.position - controlPoints[controlPoints.Count-2].transform.position;
        dir = dir.normalized;
        controlPointsOrientation.Add(dir);

        float currentLength = 0.0f;

        Vector3 pos1Init = controlPoints[0].transform.position;
        Vector3 pos2Init = controlPoints[1].transform.position;
        Vector3 vecUpInit = Vector3.up;
        Vector3 vecH1Init = Vector3.Cross(vecUpInit, controlPointsOrientation[0]);
        Vector3 vecH2Init = Vector3.Cross(vecUpInit, controlPointsOrientation[1]);
        Vector3 vecY1Init = Vector3.Cross(controlPointsOrientation[0], vecH1Init);
        Vector3 vecY2Init = Vector3.Cross(controlPointsOrientation[1], vecH2Init);
        Vector3.Normalize(vecH1Init);
        Vector3.Normalize(vecY1Init);
        Vector3.Normalize(vecY2Init);
        Vector3.Normalize(vecH2Init);

        Vector3 sectionLength = controlPoints[0].internRadius * vecH1Init - (controlPoints[0].internRadius * Mathf.Sin(2 * Mathf.PI / resolution) * vecY1Init + controlPoints[0].internRadius * Mathf.Cos(2 * Mathf.PI / resolution) * vecH1Init);
        float maxLateralUv = Mathf.Round(resolution*sectionLength.magnitude/20.0f)*20.0f;

        GameObject meshParent = new GameObject("MeshParent");
        for (int c=0; c<controlPoints.Count-1; c++)
        {
            Vector3 pos1 = controlPoints[c].transform.position;
            Vector3 pos2 = controlPoints[c+1].transform.position;
            Vector3 vecUp = Vector3.up;
            Vector3 vecH1 = Vector3.Cross(vecUp, controlPointsOrientation[c]);
            Vector3 vecH2 = Vector3.Cross(vecUp, controlPointsOrientation[c+1]);
            Vector3 vecY1 = Vector3.Cross(controlPointsOrientation[c], vecH1);
            Vector3 vecY2 = Vector3.Cross(controlPointsOrientation[c+1], vecH2);
            Vector3.Normalize(vecH1);
            Vector3.Normalize(vecY1);
            Vector3.Normalize(vecY2);
            Vector3.Normalize(vecH2);

            float rIntern1 = controlPoints[c].internRadius;
            float rExtern1 = controlPoints[c].externRadius;
            float rIntern2 = controlPoints[c+1].internRadius;
            float rExtern2 = controlPoints[c+1].externRadius;

            float partLen = (pos2 - pos1).magnitude;

            for (int i=0; i<resolution; i++)
            {
                vertices = new Vector3[] 
                {
                    pos1 + rIntern1*Mathf.Sin(2*Mathf.PI*i/resolution)*vecY1 + rIntern1*Mathf.Cos(2*Mathf.PI*i/resolution)*vecH1,
                    pos1 + rIntern1*Mathf.Sin(2*Mathf.PI*((i+1)%resolution)/resolution)*vecY1 + rIntern1*Mathf.Cos(2*Mathf.PI*((i+1)%resolution)/resolution)*vecH1,
                    pos1 + rExtern1*Mathf.Sin(2*Mathf.PI*((i+1)%resolution)/resolution)*vecY1 + rExtern1*Mathf.Cos(2*Mathf.PI*((i+1)%resolution)/resolution)*vecH1,
                    pos1 + rExtern1*Mathf.Sin(2*Mathf.PI*i/resolution)*vecY1 + rExtern1*Mathf.Cos(2*Mathf.PI*i/resolution)*vecH1,

                    pos2 + rExtern2*Mathf.Sin(2*Mathf.PI*i/resolution)*vecY2 + rExtern2*Mathf.Cos(2*Mathf.PI*i/resolution)*vecH2,
                    pos2 + rExtern2*Mathf.Sin(2*Mathf.PI*((i+1)%resolution)/resolution)*vecY2 + rExtern2*Mathf.Cos(2*Mathf.PI*((i+1)%resolution)/resolution)*vecH2,
                    pos2 + rIntern2*Mathf.Sin(2*Mathf.PI*((i+1)%resolution)/resolution)*vecY2 + rIntern2*Mathf.Cos(2*Mathf.PI*((i+1)%resolution)/resolution)*vecH2,
                    pos2 + rIntern2*Mathf.Sin(2*Mathf.PI*i/resolution)*vecY2 + rIntern2*Mathf.Cos(2*Mathf.PI*i/resolution)*vecH2,

                    pos2 + rIntern2*Mathf.Sin(2*Mathf.PI*i/resolution)*vecY2 + rIntern2*Mathf.Cos(2*Mathf.PI*i/resolution)*vecH2,
                    pos2 + rIntern2*Mathf.Sin(2*Mathf.PI*((i+1)%resolution)/resolution)*vecY2 + rIntern2*Mathf.Cos(2*Mathf.PI*((i+1)%resolution)/resolution)*vecH2,
                    pos1 + rIntern1*Mathf.Sin(2*Mathf.PI*((i+1)%resolution)/resolution)*vecY1 + rIntern1*Mathf.Cos(2*Mathf.PI*((i+1)%resolution)/resolution)*vecH1,
                    pos1 + rIntern1*Mathf.Sin(2*Mathf.PI*i/resolution)*vecY1 + rIntern1*Mathf.Cos(2*Mathf.PI*i/resolution)*vecH1,

                    pos1 + rExtern1*Mathf.Sin(2*Mathf.PI*i/resolution)*vecY1 + rExtern1*Mathf.Cos(2*Mathf.PI*i/resolution)*vecH1,
                    pos1 + rExtern1*Mathf.Sin(2*Mathf.PI*((i+1)%resolution)/resolution)*vecY1 + rExtern1*Mathf.Cos(2*Mathf.PI*((i+1)%resolution)/resolution)*vecH1,
                    pos2 + rExtern2*Mathf.Sin(2*Mathf.PI*((i+1)%resolution)/resolution)*vecY2 + rExtern2*Mathf.Cos(2*Mathf.PI*((i+1)%resolution)/resolution)*vecH2,
                    pos2 + rExtern2*Mathf.Sin(2*Mathf.PI*i/resolution)*vecY2 + rExtern2*Mathf.Cos(2*Mathf.PI*i/resolution)*vecH2,
                };

                Vector3 internNormal = (vertices[8] - pos2 + vertices[9] - pos2) * (1 / rIntern2) + (vertices[10] - pos1 + vertices[11] - pos1) * (1 / rIntern1);
                Vector3 externNormal = (vertices[12] - pos1 + vertices[13] - pos1) * (1 / rIntern1) + (vertices[14] - pos2 + vertices[15] - pos2) * (1 / rIntern2);
                internNormal = internNormal.normalized;
                externNormal = externNormal.normalized;

                normals = new Vector3[]
                {
                    -controlPointsOrientation[c], -controlPointsOrientation[c], -controlPointsOrientation[c], -controlPointsOrientation[c],
                    controlPointsOrientation[c+1], controlPointsOrientation[c+1], controlPointsOrientation[c+1], controlPointsOrientation[c+1],
                    internNormal, internNormal, internNormal, internNormal,
                    externNormal, externNormal, externNormal, externNormal,
                    /*Vector2.up,Vector2.up,Vector2.up,Vector2.up,
                    Vector2.up,Vector2.up,Vector2.up,Vector2.up,
                    Vector2.up,Vector2.up,Vector2.up,Vector2.up,
                    Vector2.up,Vector2.up,Vector2.up,Vector2.up,*/
                };

                uvs = new Vector2[]
                {
                    new Vector2(i*maxLateralUv/resolution, 14),
                    new Vector2((i+1)*maxLateralUv/resolution, 14),
                    new Vector2((i+1)*maxLateralUv/resolution, 16),
                    new Vector2(i*maxLateralUv/resolution, 16),

                    new Vector2(i*maxLateralUv/resolution, 16),
                    new Vector2((i+1)*maxLateralUv/resolution, 14),
                    new Vector2((i+1)*maxLateralUv/resolution, 14),
                    new Vector2(i*maxLateralUv/resolution, 16),

                    new Vector2(i*maxLateralUv/resolution, currentLength+partLen),
                    new Vector2((i+1)*maxLateralUv/resolution, currentLength+partLen),
                    new Vector2((i+1)*maxLateralUv/resolution, currentLength),
                    new Vector2(i*maxLateralUv/resolution, currentLength),

                    new Vector2(i*maxLateralUv/resolution, currentLength),
                    new Vector2((i+1)*maxLateralUv/resolution, currentLength),
                    new Vector2((i+1)*maxLateralUv/resolution, currentLength+partLen),
                    new Vector2(i*maxLateralUv/resolution, currentLength+partLen),
                };


                triangles = new int[]
                {
                    0,1,2,
                    0,2,3,
                    4,5,6,
                    4,6,7,
                    8,9,10,
                    8,10,11,
                    12,13,14,
                    12,14,15,
                };

                GameObject quadObj = new GameObject("QuadObj");
                quadObj.transform.parent = meshParent.transform;
                MeshFilter meshFilter = quadObj.AddComponent<MeshFilter>();
                Mesh mesh = meshFilter.mesh;
                mesh.Clear();

                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.normals = normals;
                mesh.uv = uvs;

                mesh.RecalculateNormals();

                MeshRenderer meshRenderer = quadObj.AddComponent<MeshRenderer>();
                meshRenderer.materials = new Material[] { mat };

                MeshCollider collider = quadObj.AddComponent<MeshCollider>();
                collider.convex = true;

                quadObj.layer = LayerMask.NameToLayer("Floor");
            }

            currentLength += partLen;
        }
    }





    private void BuildTornadoRoad()
    {
        List<Vector3> velocityList = new List<Vector3>();

        Vector3 vel = controlPointsList[1].transform.position - controlPointsList[0].transform.position;
        //vel = vel.normalized;
        velocityList.Add(0.5f * vel);
        for (int i = 1; i < controlPointsList.Count - 1; i++)
        {
            vel = controlPointsList[i + 1].transform.position - controlPointsList[i - 1].transform.position;
            //vel = vel.normalized;
            velocityList.Add(0.5f * vel);
        }
        vel = controlPointsList[controlPointsList.Count - 1].transform.position - controlPointsList[controlPointsList.Count - 2].transform.position;
        //vel = vel.normalized;
        velocityList.Add(0.5f * vel);

        GameObject interpolatedControlPointsParent = new GameObject("InterpolatedControlPointsParent");
        List<MeshControlPoint> controlPoints = new List<MeshControlPoint>();
        controlPoints.Add(controlPointsList[0]);
        for (int i = 1; i < controlPointsList.Count; i++)
        {
            List<Vector3> listInterpolatedPoints = GetHermitInterpolationPoints(controlPointsList[i - 1].transform.position, velocityList[i - 1], controlPointsList[i].transform.position, velocityList[i], pointResolution);
            foreach (Vector3 vec in listInterpolatedPoints)
            {
                GameObject interpolatedControlPoint = new GameObject("interpolatedControlPoint");
                interpolatedControlPoint.transform.position = vec;
                interpolatedControlPoint.transform.parent = interpolatedControlPointsParent.transform;
                MeshControlPoint meshControlPoint = interpolatedControlPoint.AddComponent<MeshControlPoint>();
                meshControlPoint.internRadius = controlPointsList[i].internRadius;
                meshControlPoint.externRadius = controlPointsList[i].externRadius;
                meshControlPoint.height = controlPointsList[i].height;
                meshControlPoint.width = controlPointsList[i].width;
                meshControlPoint.borderHeight = controlPointsList[i].borderHeight;

                controlPoints.Add(meshControlPoint);
            }
            controlPoints.Add(controlPointsList[i]);
        }


        List<Vector3> controlPointsOrientation = new List<Vector3>();

        Vector3 dir = controlPoints[1].transform.position - controlPoints[0].transform.position;
        dir = dir.normalized;
        controlPointsOrientation.Add(dir);
        for (int i = 1; i < controlPoints.Count - 1; i++)
        {
            dir = controlPoints[i + 1].transform.position - controlPoints[i - 1].transform.position;
            dir = dir.normalized;
            controlPointsOrientation.Add(dir);
        }
        dir = controlPoints[controlPoints.Count - 1].transform.position - controlPoints[controlPoints.Count - 2].transform.position;
        dir = dir.normalized;
        controlPointsOrientation.Add(dir);

        List<Vector3> anchorList = new List<Vector3>();
        List<Vector3> roadNormals = new List<Vector3>();
        float roadLightCount = 0.0f;

        Vector3 vecHInit = Vector3.Cross(Vector3.up, controlPointsOrientation[0]);
        Vector3 vecYInit = Vector3.Cross(controlPointsOrientation[0], vecHInit);
        Vector3.Normalize(vecHInit);
        Vector3.Normalize(vecYInit);
        float widthInit = controlPoints[0].width;
        Vector3 pAnchor1 = controlPoints[0].transform.position + widthInit * Mathf.Sin(initialRoadRotation * 2 * Mathf.PI) * vecYInit + widthInit * Mathf.Cos(initialRoadRotation * 2 * Mathf.PI) * vecHInit;
        Vector3 pAnchor2 = controlPoints[0].transform.position - widthInit * Mathf.Sin(initialRoadRotation * 2 * Mathf.PI) * vecYInit - widthInit * Mathf.Cos(initialRoadRotation * 2 * Mathf.PI) * vecHInit;
        anchorList.Add(pAnchor1);
        anchorList.Add(pAnchor2);

        GameObject meshParent = new GameObject("MeshParent");
        for (int c = 1; c < controlPoints.Count; c++)
        {
            Vector3 posCenter = controlPoints[c].transform.position;
            Vector3 roadNormal = Vector3.Cross(posCenter - pAnchor1, posCenter - pAnchor2);
            Vector3 vecH = Vector3.Cross(roadNormal, controlPointsOrientation[c]);
            vecH = vecH.normalized;
            float width = controlPoints[c].width;
            pAnchor1 = posCenter + width * vecH;
            pAnchor2 = posCenter - width * vecH;
            anchorList.Add(pAnchor1);
            anchorList.Add(pAnchor2);
            roadNormals.Add(roadNormal);
        }

        for (int c = 1; c < controlPoints.Count; c++)
        {
            float lightDeltaDist = (controlPoints[c].transform.position - controlPoints[c-1].transform.position).magnitude;
            for(int i= (int)Mathf.Floor(roadLightCount / roadLightInter) + 1; i < ((roadLightCount + lightDeltaDist) / roadLightInter); i++)
            {
                float t = (i * roadLightInter - roadLightCount) / lightDeltaDist;

                Vector3 midPos = Vector3.Lerp(controlPoints[c - 1].transform.position, controlPoints[c].transform.position, t);
                Vector3 posCenter = controlPoints[c].transform.position;
                Vector3 roadNormal = Vector3.Cross(posCenter - anchorList[2 * (c - 1)], posCenter - anchorList[2 * (c - 1) + 1]);
                Vector3 vecH = Vector3.Cross(roadNormal, controlPointsOrientation[c]);
                vecH = vecH.normalized;

                GameObject arkRoad = new GameObject("arkRoad");
                ArkGenerator arkGenerator = arkRoad.AddComponent<ArkGenerator>();
                arkGenerator.gapLength = 0.3f;
                arkGenerator.mat = edgeMat;
                arkGenerator.scale = 2 * (t * (controlPoints[c - 1].width - controlPoints[c - 1].height / 2) + (1 - t) * (controlPoints[c].width - controlPoints[c].height / 2));
                arkGenerator.transform.position = midPos;
                arkGenerator.transform.LookAt(midPos + controlPointsOrientation[c-1]);
                Quaternion leftLightRot = Quaternion.LookRotation(controlPointsOrientation[c - 1], roadNormal);
                arkGenerator.transform.rotation = leftLightRot;

                /*GameObject leftRoadLight = new GameObject("leftRoadLight");
                RoadLightGenerator leftGen = leftRoadLight.AddComponent<RoadLightGenerator>();
                leftGen.gapLength = 0.3f;
                leftGen.mat = edgeMat;
                leftGen.scale = roadLightScale;
                leftGen.transform.position = midPos + (t * (controlPoints[c-1].width - controlPoints[c - 1].height / 2) * vecH + (1-t) * (controlPoints[c].width - controlPoints[c].height / 2) * vecH);
                leftGen.transform.LookAt(midPos);
                Quaternion leftLightRot = Quaternion.LookRotation(midPos - leftGen.transform.position, roadNormal);
                leftGen.transform.rotation = leftLightRot;

                GameObject rightRoadLight = new GameObject("rightRoadLight");
                RoadLightGenerator rightGen = rightRoadLight.AddComponent<RoadLightGenerator>();
                rightGen.gapLength = 0.3f;
                rightGen.mat = edgeMat;
                rightGen.scale = roadLightScale;
                rightGen.transform.position = midPos - (t * (controlPoints[c - 1].width - controlPoints[c - 1].height / 2) * vecH + (1 - t) * (controlPoints[c].width - controlPoints[c].height / 2) * vecH);
                rightGen.transform.LookAt(midPos);
                Quaternion rightLightRot = Quaternion.LookRotation(midPos - rightGen.transform.position, roadNormal); 
                rightGen.transform.rotation = rightLightRot;*/
            }
            roadLightCount += lightDeltaDist;
        }

        float currentLength = 0.0f;
        float lateralOffset = (Mathf.Ceil(2 * widthInit / 20.0f) * 20.0f - 2 * widthInit) / 2 + 0.5f;

        for(int c = 0; c < controlPoints.Count - 1; c++){

            Vector3 normal1;
            Vector3 normal2;
            if (c == 0)
            {
                normal1 = roadNormals[0];
                normal1 = normal1.normalized;
            }
            else
            {
                normal1 = roadNormals[c - 1] + roadNormals[c];
                normal1 = normal1.normalized;
            }

            if (c == controlPoints.Count-2)
            {
                normal2 = roadNormals[c];
                normal2 = normal2.normalized;
            }
            else
            {
                normal2 = roadNormals[c] + roadNormals[c+1];
                normal2 = normal2.normalized;
            }

            float height1 = controlPoints[c].height;
            float height2 = controlPoints[c+1].height;
            float border1 = controlPoints[c].borderHeight;
            float border2 = controlPoints[c + 1].borderHeight;

            vertices = new Vector3[]
            {
                anchorList[c * 2],
                anchorList[c * 2 + 1],
                anchorList[(c+1) * 2 + 1],
                anchorList[(c+1) * 2],

                anchorList[c * 2 + 1],
                anchorList[c * 2 + 1]- height1*normal1,
                anchorList[(c+1) * 2 + 1]- height2*normal2,
                anchorList[(c+1) * 2 + 1],

                anchorList[c * 2 + 1]- height1*normal1,
                anchorList[c * 2] - height1*normal1,
                anchorList[(c+1) * 2]- height2*normal2,
                anchorList[(c+1) * 2 + 1]- height2*normal2,

                anchorList[c * 2] - height1*normal1,
                anchorList[c * 2],
                anchorList[(c+1) * 2],
                anchorList[(c+1) * 2]- height2*normal2,

                anchorList[c * 2 + 1]- height1*normal1,
                anchorList[c * 2 + 1],
                anchorList[c * 2],
                anchorList[c * 2] - height1*normal1,

                anchorList[(c+1) * 2]- height2*normal2,
                anchorList[(c+1) * 2],
                anchorList[(c+1) * 2 + 1],
                anchorList[(c+1) * 2 + 1]- height2*normal2,
            };

            normals = new Vector3[]
            {

            };

            float partLen = (controlPoints[c + 1].transform.position - controlPoints[c].transform.position).magnitude;
            

            uvs = new Vector2[]
            {
                new Vector2(lateralOffset, currentLength),
                new Vector2(lateralOffset + 2*controlPoints[c].width, currentLength),
                new Vector2(lateralOffset + 2*controlPoints[c+1].width, currentLength + partLen),
                new Vector2(lateralOffset, currentLength + partLen),

                new Vector2(14, currentLength),
                new Vector2(16, currentLength),
                new Vector2(16, currentLength + partLen),
                new Vector2(14, currentLength + partLen),


                new Vector2(lateralOffset + 2*controlPoints[c].width, currentLength),
                new Vector2(lateralOffset, currentLength),
                new Vector2(lateralOffset, currentLength + partLen),
                new Vector2(lateralOffset + 2*controlPoints[c+1].width, currentLength + partLen),

                new Vector2(16, currentLength),
                new Vector2(14, currentLength),
                new Vector2(14, currentLength + partLen),
                new Vector2(16, currentLength + partLen),


                new Vector2(lateralOffset + 2*controlPoints[c].width, 14),
                new Vector2(lateralOffset + 2*controlPoints[c].width, 16),
                new Vector2(lateralOffset, 16),
                new Vector2(lateralOffset, 14),

                new Vector2(lateralOffset, 14),
                new Vector2(lateralOffset, 16),
                new Vector2(lateralOffset + 2*controlPoints[c+1].width, 16),
                new Vector2(lateralOffset + 2*controlPoints[c+1].width, 14),
            };

            triangles = new int[]
            {
                0,1,2,
                0,2,3,
                4,5,6,
                4,6,7,
                8,9,10,
                8,10,11,
                12,13,14,
                12,14,15,
                16,17,18,
                16,18,19,
                20,21,22,
                20,22,23,
            };


            GameObject quadObj = new GameObject("QuadObj");
            quadObj.transform.parent = meshParent.transform;
            MeshFilter meshFilter = quadObj.AddComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();

            MeshRenderer meshRenderer = quadObj.AddComponent<MeshRenderer>();
            meshRenderer.materials = new Material[] { mat };

            MeshCollider collider = quadObj.AddComponent<MeshCollider>();
            collider.convex = true;

            quadObj.layer = LayerMask.NameToLayer("Floor");

            //Border 1

            Vector3 offset1Wall1 = anchorList[c * 2 + 1] - anchorList[c * 2];
            offset1Wall1 = offset1Wall1.normalized;
            Vector3 offset2Wall1 = anchorList[(c+1) * 2 + 1] - anchorList[(c+1) * 2];
            offset2Wall1 = offset2Wall1.normalized;

            vertices = new Vector3[]
            {
                anchorList[c * 2] + border1*normal1,
                anchorList[c * 2] + border1*normal1 + height1*offset1Wall1,
                anchorList[(c+1) * 2] + border2*normal2 + height2*offset2Wall1,
                anchorList[(c+1) * 2] + border2*normal2,

                anchorList[c * 2] + border1*normal1 + height1*offset1Wall1,
                anchorList[c * 2] + height1*offset1Wall1,
                anchorList[(c+1) * 2] + height2*offset2Wall1,
                anchorList[(c+1) * 2] + border2*normal2 + height2*offset2Wall1,

                anchorList[c * 2] + height1*offset1Wall1,
                anchorList[c * 2],
                anchorList[(c+1) * 2],
                anchorList[(c+1) * 2] + height2*offset2Wall1,

                anchorList[c * 2],
                anchorList[c * 2] + border1*normal1,
                anchorList[(c+1) * 2] + border2*normal2,
                anchorList[(c+1) * 2],

                anchorList[c * 2] + height1*offset1Wall1,
                anchorList[c * 2] + border1*normal1 + height1*offset1Wall1,
                anchorList[c * 2] + border1*normal1,
                anchorList[c * 2],


                anchorList[(c+1) * 2],
                anchorList[(c+1) * 2] + border2*normal2,
                anchorList[(c+1) * 2] + border2*normal2 + height2*offset2Wall1,
                anchorList[(c+1) * 2] + height2*offset2Wall1,
            };

            normals = new Vector3[]
            {

            };

            uvs = new Vector2[]
            {
                new Vector2(height1, currentLength),
                new Vector2(0, currentLength),
                new Vector2(0, currentLength + partLen),
                new Vector2(height2, currentLength + partLen),

                new Vector2(0, currentLength),
                new Vector2(border1, currentLength),
                new Vector2(border2, currentLength + partLen),
                new Vector2(0, currentLength + partLen),

                new Vector2(14, currentLength),
                new Vector2(16, currentLength),
                new Vector2(16, currentLength + partLen),
                new Vector2(14, currentLength + partLen),

                new Vector2(height1+border1, currentLength),
                new Vector2(height1, currentLength),
                new Vector2(height2, currentLength + partLen),
                new Vector2(height2+border2, currentLength + partLen),

                new Vector2(border1, 0),
                new Vector2(0, 0),
                new Vector2(0, height1),
                new Vector2(border1, height1),

                new Vector2(border2, height2),
                new Vector2(0, height2),
                new Vector2(0, 0),
                new Vector2(border2, 0),
            };

            triangles = new int[]
            {
                0,1,2,
                0,2,3,
                4,5,6,
                4,6,7,
                8,9,10,
                8,10,11,
                12,13,14,
                12,14,15,
                16,17,18,
                16,18,19,
                20,21,22,
                20,22,23,
            };


            GameObject wall1Obj = new GameObject("Wall1Obj");
            wall1Obj.transform.parent = meshParent.transform;
            MeshFilter wall1MeshFilter = wall1Obj.AddComponent<MeshFilter>();
            Mesh wall1Mesh = wall1MeshFilter.mesh;
            wall1Mesh.Clear();

            wall1Mesh.vertices = vertices;
            wall1Mesh.triangles = triangles;
            wall1Mesh.uv = uvs;

            wall1Mesh.RecalculateNormals();

            MeshRenderer wall1MeshRenderer = wall1Obj.AddComponent<MeshRenderer>();
            wall1MeshRenderer.materials = new Material[] { mat };

            MeshCollider wall1Collider = wall1Obj.AddComponent<MeshCollider>();
            wall1Collider.convex = true;

            wall1Obj.layer = LayerMask.NameToLayer("Floor");


            //Border2

            Vector3 offset1Wall2 = anchorList[c * 2] - anchorList[c * 2 + 1];
            offset1Wall2 = offset1Wall2.normalized;
            Vector3 offset2Wall2 = anchorList[(c + 1) * 2] - anchorList[(c + 1) * 2 + 1];
            offset2Wall2 = offset2Wall2.normalized;

            vertices = new Vector3[]
            {
                anchorList[c * 2 + 1] + border1*normal1 + height1*offset1Wall2,
                anchorList[c * 2 + 1] + border1*normal1,
                anchorList[(c+1) * 2 + 1] + border2*normal2,
                anchorList[(c+1) * 2 + 1] + border2*normal2 + height2*offset2Wall2,

                anchorList[c * 2 + 1] + border1*normal1,
                anchorList[c * 2 + 1],
                anchorList[(c+1) * 2 + 1],
                anchorList[(c+1) * 2 + 1] + border2*normal2,

                anchorList[c * 2 + 1],
                anchorList[c * 2 + 1] + height1*offset1Wall2,
                anchorList[(c+1) * 2 + 1] + height2*offset2Wall2,
                anchorList[(c+1) * 2 + 1],

                anchorList[c * 2 + 1] + height1*offset1Wall2,
                anchorList[c * 2 + 1] + border1*normal1 + height1*offset1Wall2,
                anchorList[(c+1) * 2 + 1] + border2*normal2 + height2*offset2Wall2,
                anchorList[(c+1) * 2 + 1] + height2*offset2Wall2,

                anchorList[c * 2 + 1],
                anchorList[c * 2 + 1] + border1*normal1,
                anchorList[c * 2 + 1] + border1*normal1 + height1*offset1Wall2,
                anchorList[c * 2 + 1] + height1*offset1Wall2,

                anchorList[(c+1) * 2 + 1] + height2*offset2Wall2,
                anchorList[(c+1) * 2 + 1] + border2*normal2 + height2*offset2Wall2,
                anchorList[(c+1) * 2 + 1] + border2*normal2,
                anchorList[(c+1) * 2 + 1],
            };

            normals = new Vector3[]
            {

            };

            uvs = new Vector2[]
            {
                new Vector2(0, currentLength),
                new Vector2(height1, currentLength),
                new Vector2(height2, currentLength + partLen),
                new Vector2(0, currentLength + partLen),

                new Vector2(height1, currentLength),
                new Vector2(height1+border1, currentLength),
                new Vector2(height2+border2, currentLength + partLen),
                new Vector2(height2, currentLength + partLen),

                new Vector2(14, currentLength),
                new Vector2(16, currentLength),
                new Vector2(16, currentLength + partLen),
                new Vector2(14, currentLength + partLen),

                new Vector2(border1, currentLength),
                new Vector2(0, currentLength),
                new Vector2(0, currentLength + partLen),
                new Vector2(border2, currentLength + partLen),

                new Vector2(border1, height1),
                new Vector2(0, height1),
                new Vector2(0, 0),
                new Vector2(border1, 0),

                new Vector2(border2, 0),
                new Vector2(0, 0),
                new Vector2(0, height2),
                new Vector2(border2, height2),
            };

            triangles = new int[]
            {
                0,1,2,
                0,2,3,
                4,5,6,
                4,6,7,
                8,9,10,
                8,10,11,
                12,13,14,
                12,14,15,
                16,17,18,
                16,18,19,
                20,21,22,
                20,22,23,
            };


            GameObject wall2Obj = new GameObject("Wall2Obj");
            wall2Obj.transform.parent = meshParent.transform;
            MeshFilter wall2MeshFilter = wall2Obj.AddComponent<MeshFilter>();
            Mesh wall2Mesh = wall2MeshFilter.mesh;
            wall2Mesh.Clear();

            wall2Mesh.vertices = vertices;
            wall2Mesh.triangles = triangles;
            wall2Mesh.uv = uvs;

            wall2Mesh.RecalculateNormals();

            MeshRenderer wall2MeshRenderer = wall2Obj.AddComponent<MeshRenderer>();
            wall2MeshRenderer.materials = new Material[] { mat };

            MeshCollider wall2Collider = wall2Obj.AddComponent<MeshCollider>();
            wall2Collider.convex = true;

            wall2Obj.layer = LayerMask.NameToLayer("Floor");

            currentLength += partLen;
        }
        
    }


    private void BuildFlatRoad()
    {

        List<Vector3> controlPointsOrientation = new List<Vector3>();

        Vector3 dir = controlPointsList[1].transform.position - controlPointsList[0].transform.position;
        dir = dir.normalized;
        controlPointsOrientation.Add(dir);
        for (int i = 1; i < controlPointsList.Count - 1; i++)
        {
            dir = controlPointsList[i + 1].transform.position - controlPointsList[i - 1].transform.position;
            dir = dir.normalized;
            controlPointsOrientation.Add(dir);
        }
        dir = controlPointsList[controlPointsList.Count - 1].transform.position - controlPointsList[controlPointsList.Count - 2].transform.position;
        dir = dir.normalized;
        controlPointsOrientation.Add(dir);

        Vector3 firstOrtho = Vector3.Cross(Vector3.up, controlPointsOrientation[0]);
        firstOrtho = firstOrtho.normalized;
        Vector3 roadNormal = Mathf.Cos(initialRoadRotation * 2 * Mathf.PI) * Vector3.up + Mathf.Sin(initialRoadRotation * 2 * Mathf.PI) * firstOrtho;

        GameObject meshParent = new GameObject("MeshParent");

        float currentLength = 0.0f;
        float lateralOffset = (Mathf.Ceil(2 * controlPointsList[0].width / 20.0f) * 20.0f - 2 * controlPointsList[0].width) / 2 + 0.5f;

        for (int c = 0; c < controlPointsList.Count - 1; c++)
        {
            float height1 = controlPointsList[c].height;
            float height2 = controlPointsList[c + 1].height;
            float width1 = controlPointsList[c].width;
            float width2 = controlPointsList[c + 1].width;
            float border1 = controlPointsList[c].borderHeight;
            float border2 = controlPointsList[c + 1].borderHeight;

            Vector3 mainRoadOrtho = Vector3.Cross(roadNormal, (controlPointsList[c+1].transform.position - controlPointsList[c].transform.position));
            mainRoadOrtho = mainRoadOrtho.normalized;
            Vector3 roadOrtho1 = Vector3.Cross(roadNormal, controlPointsOrientation[c]);
            roadOrtho1 = roadOrtho1.normalized;
            Vector3 roadOrtho2 = Vector3.Cross(roadNormal, controlPointsOrientation[c+1]);
            roadOrtho2 = roadOrtho2.normalized;

            Vector3 mainRoadDir = Vector3.Cross(mainRoadOrtho, roadNormal);
            mainRoadDir = mainRoadDir.normalized;
            Vector3 roadDir1 = Vector3.Cross(roadOrtho1, roadNormal);
            roadDir1 = roadDir1.normalized;
            Vector3 roadDir2 = Vector3.Cross(roadOrtho2, roadNormal);
            roadDir2 = roadDir2.normalized;

            Vector3 offDir1 = mainRoadDir + roadDir1;
            offDir1 = offDir1.normalized;
            Vector3 offDir2 = mainRoadDir + roadDir2;
            offDir2 = offDir2.normalized;

            Vector3 offCenter1 = controlPointsList[c].transform.position + width1 * (roadOrtho1-mainRoadOrtho).magnitude * offDir1;
            Vector3 offCenter2 = controlPointsList[c + 1].transform.position - width2 * (roadOrtho2 - mainRoadOrtho).magnitude * offDir2;

            //Debug.DrawLine(controlPointsList[c].transform.position, controlPointsList[c + 1].transform.position, Color.black, Mathf.Infinity);
            //Debug.DrawLine(controlPointsList[c].transform.position + width1 * roadOrtho1, controlPointsList[c].transform.position - width1 * roadOrtho1, Color.green, Mathf.Infinity);
            //Debug.DrawLine(controlPointsList[c].transform.position, offCenter1, Color.red, Mathf.Infinity);

            float partLen = (controlPointsList[c].transform.position - offCenter1).magnitude;

            // --- First Triangle ---
            if ((controlPointsList[c].transform.position - offCenter1).magnitude > 0.01f)
            {
                vertices = new Vector3[]
                {
                    controlPointsList[c].transform.position + width1 * roadOrtho1,
                    controlPointsList[c].transform.position - width1 * roadOrtho1,
                    offCenter1 - width1 * mainRoadOrtho,
                    offCenter1 + width1 * mainRoadOrtho,

                    controlPointsList[c].transform.position - width1 * roadOrtho1,
                    controlPointsList[c].transform.position - width1 * roadOrtho1 - height1 * roadNormal,
                    offCenter1 - width1 * mainRoadOrtho - height1 * roadNormal,
                    offCenter1 - width1 * mainRoadOrtho,

                    controlPointsList[c].transform.position - width1 * roadOrtho1 - height1 * roadNormal,
                    controlPointsList[c].transform.position + width1 * roadOrtho1 - height1 * roadNormal,
                    offCenter1 + width1 * mainRoadOrtho - height1 * roadNormal,
                    offCenter1 - width1 * mainRoadOrtho - height1 * roadNormal,

                    controlPointsList[c].transform.position + width1 * roadOrtho1 - height1 * roadNormal,
                    controlPointsList[c].transform.position + width1 * roadOrtho1,
                    offCenter1 + width1 * mainRoadOrtho,
                    offCenter1 + width1 * mainRoadOrtho - height1 * roadNormal,

                    controlPointsList[c].transform.position - width1 * roadOrtho1 - height1 * roadNormal,
                    controlPointsList[c].transform.position - width1 * roadOrtho1,
                    controlPointsList[c].transform.position + width1 * roadOrtho1,
                    controlPointsList[c].transform.position + width1 * roadOrtho1 - height1 * roadNormal,
                };

                normals = new Vector3[]
                {
                
                };

                uvs = new Vector2[]
                {
                    new Vector2(lateralOffset, currentLength),
                    new Vector2(lateralOffset + 2*controlPointsList[c].width, currentLength),
                    new Vector2(lateralOffset + 2*controlPointsList[c+1].width, currentLength + partLen),
                    new Vector2(lateralOffset, currentLength + partLen),

                    new Vector2(14, currentLength),
                    new Vector2(16, currentLength),
                    new Vector2(16, currentLength + partLen),
                    new Vector2(14, currentLength + partLen),


                    new Vector2(lateralOffset + 2*controlPointsList[c].width, currentLength),
                    new Vector2(lateralOffset, currentLength),
                    new Vector2(lateralOffset, currentLength + partLen),
                    new Vector2(lateralOffset + 2*controlPointsList[c+1].width, currentLength + partLen),

                    new Vector2(16, currentLength),
                    new Vector2(14, currentLength),
                    new Vector2(14, currentLength + partLen),
                    new Vector2(16, currentLength + partLen),


                    new Vector2(lateralOffset + 2*controlPointsList[c].width, 14),
                    new Vector2(lateralOffset + 2*controlPointsList[c].width, 16),
                    new Vector2(lateralOffset, 16),
                    new Vector2(lateralOffset, 14),
                };

                triangles = new int[]
                {
                    0,1,2,
                    0,2,3,
                    4,5,6,
                    4,6,7,
                    8,9,10,
                    8,10,11,
                    12,13,14,
                    12,14,15,
                    16,17,18,
                    16,18,19,
                };


                GameObject quadObj1 = new GameObject("QuadObj1");
                quadObj1.transform.parent = meshParent.transform;
                MeshFilter meshFilter1 = quadObj1.AddComponent<MeshFilter>();
                Mesh mesh1 = meshFilter1.mesh;
                mesh1.Clear();

                mesh1.vertices = vertices;
                mesh1.triangles = triangles;
                mesh1.uv = uvs;

                mesh1.RecalculateNormals();

                MeshRenderer meshRenderer1 = quadObj1.AddComponent<MeshRenderer>();
                meshRenderer1.materials = new Material[] { mat };

                MeshCollider collider1 = quadObj1.AddComponent<MeshCollider>();
                collider1.convex = true;

                quadObj1.layer = LayerMask.NameToLayer("Floor");
            }
            

            // Border 1

            if((controlPointsList[c].transform.position - offCenter1).magnitude > 0.01f)
            {
                vertices = new Vector3[]
                {
                    controlPointsList[c].transform.position + width1 * roadOrtho1 + border1 * roadNormal,
                    controlPointsList[c].transform.position + (width1-height1) * roadOrtho1 + border1 * roadNormal,
                    offCenter1 + (width1-height1) * mainRoadOrtho + border1 * roadNormal,
                    offCenter1 + width1 * mainRoadOrtho + border1 * roadNormal,

                    controlPointsList[c].transform.position + (width1-height1) * roadOrtho1 + border1 * roadNormal,
                    controlPointsList[c].transform.position + (width1-height1) * roadOrtho1,
                    offCenter1 + (width1-height1) * mainRoadOrtho,
                    offCenter1 + (width1-height1) * mainRoadOrtho + border1 * roadNormal,

                    controlPointsList[c].transform.position + (width1-height1) * roadOrtho1,
                    controlPointsList[c].transform.position + width1 * roadOrtho1,
                    offCenter1 + width1 * mainRoadOrtho,
                    offCenter1 + (width1-height1) * mainRoadOrtho,

                    controlPointsList[c].transform.position + width1 * roadOrtho1,
                    controlPointsList[c].transform.position + width1 * roadOrtho1 + border1 * roadNormal,
                    offCenter1 + width1 * mainRoadOrtho + border1 * roadNormal,
                    offCenter1 + width1 * mainRoadOrtho,

                    controlPointsList[c].transform.position + (width1-height1) * roadOrtho1,
                    controlPointsList[c].transform.position + (width1-height1) * roadOrtho1 + border1 * roadNormal,
                    controlPointsList[c].transform.position + width1 * roadOrtho1 + border1 * roadNormal,
                    controlPointsList[c].transform.position + width1 * roadOrtho1,
                };

                normals = new Vector3[]
                {

                };

                uvs = new Vector2[]
                {
                    new Vector2(height1, currentLength),
                    new Vector2(0, currentLength),
                    new Vector2(0, currentLength + partLen),
                    new Vector2(height2, currentLength + partLen),

                    new Vector2(0, currentLength),
                    new Vector2(border1, currentLength),
                    new Vector2(border2, currentLength + partLen),
                    new Vector2(0, currentLength + partLen),

                    new Vector2(14, currentLength),
                    new Vector2(16, currentLength),
                    new Vector2(16, currentLength + partLen),
                    new Vector2(14, currentLength + partLen),

                    new Vector2(height1+border1, currentLength),
                    new Vector2(height1, currentLength),
                    new Vector2(height2, currentLength + partLen),
                    new Vector2(height2+border2, currentLength + partLen),

                    new Vector2(border1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, height1),
                    new Vector2(border1, height1),
                };

                triangles = new int[]
                {
                    0,1,2,
                    0,2,3,
                    4,5,6,
                    4,6,7,
                    8,9,10,
                    8,10,11,
                    12,13,14,
                    12,14,15,
                    16,17,18,
                    16,18,19,
                };


                GameObject quadObj11 = new GameObject("QuadObj11");
                quadObj11.transform.parent = meshParent.transform;
                MeshFilter meshFilter11 = quadObj11.AddComponent<MeshFilter>();
                Mesh mesh11 = meshFilter11.mesh;
                mesh11.Clear();

                mesh11.vertices = vertices;
                mesh11.triangles = triangles;
                mesh11.uv = uvs;

                mesh11.RecalculateNormals();

                MeshRenderer meshRenderer11 = quadObj11.AddComponent<MeshRenderer>();
                meshRenderer11.materials = new Material[] { mat };

                MeshCollider collider11 = quadObj11.AddComponent<MeshCollider>();
                collider11.convex = true;

                quadObj11.layer = LayerMask.NameToLayer("Floor");
            }

            

            // Border 2

            if((controlPointsList[c].transform.position - offCenter1).magnitude > 0.01f)
            {
                vertices = new Vector3[]
                {
                    controlPointsList[c].transform.position - (width1-height1) * roadOrtho1 + border1 * roadNormal,
                    controlPointsList[c].transform.position - width1 * roadOrtho1 + border1 * roadNormal,
                    offCenter1 - width1 * mainRoadOrtho + border1 * roadNormal,
                    offCenter1 - (width1-height1) * mainRoadOrtho + border1 * roadNormal,

                    controlPointsList[c].transform.position - width1 * roadOrtho1 + border1 * roadNormal,
                    controlPointsList[c].transform.position - width1 * roadOrtho1,
                    offCenter1 - width1 * mainRoadOrtho,
                    offCenter1 - width1 * mainRoadOrtho + border1 * roadNormal,

                    controlPointsList[c].transform.position - width1 * roadOrtho1,
                    controlPointsList[c].transform.position - (width1-height1) * roadOrtho1,
                    offCenter1 - (width1-height1) * mainRoadOrtho,
                    offCenter1 - width1 * mainRoadOrtho,

                    controlPointsList[c].transform.position - (width1-height1) * roadOrtho1,
                    controlPointsList[c].transform.position - (width1-height1) * roadOrtho1 + border1 * roadNormal,
                    offCenter1 - (width1-height1) * mainRoadOrtho + border1 * roadNormal,
                    offCenter1 - (width1-height1) * mainRoadOrtho,

                    controlPointsList[c].transform.position - width1 * roadOrtho1,
                    controlPointsList[c].transform.position - width1 * roadOrtho1 + border1 * roadNormal,
                    controlPointsList[c].transform.position - (width1-height1) * roadOrtho1 + border1 * roadNormal,
                    controlPointsList[c].transform.position - (width1-height1) * roadOrtho1,
                };

                normals = new Vector3[]
                {

                };

                uvs = new Vector2[]
                {
                    new Vector2(0, currentLength),
                    new Vector2(height1, currentLength),
                    new Vector2(height2, currentLength + partLen),
                    new Vector2(0, currentLength + partLen),

                    new Vector2(height1, currentLength),
                    new Vector2(height1+border1, currentLength),
                    new Vector2(height2+border2, currentLength + partLen),
                    new Vector2(height2, currentLength + partLen),

                    new Vector2(14, currentLength),
                    new Vector2(16, currentLength),
                    new Vector2(16, currentLength + partLen),
                    new Vector2(14, currentLength + partLen),

                    new Vector2(border1, currentLength),
                    new Vector2(0, currentLength),
                    new Vector2(0, currentLength + partLen),
                    new Vector2(border2, currentLength + partLen),

                    new Vector2(border1, height1),
                    new Vector2(0, height1),
                    new Vector2(0, 0),
                    new Vector2(border1, 0),
                };

                triangles = new int[]
                {
                    0,1,2,
                    0,2,3,
                    4,5,6,
                    4,6,7,
                    8,9,10,
                    8,10,11,
                    12,13,14,
                    12,14,15,
                    16,17,18,
                    16,18,19,
                };


                GameObject quadObj12 = new GameObject("QuadObj12");
                quadObj12.transform.parent = meshParent.transform;
                MeshFilter meshFilter12 = quadObj12.AddComponent<MeshFilter>();
                Mesh mesh12 = meshFilter12.mesh;
                mesh12.Clear();

                mesh12.vertices = vertices;
                mesh12.triangles = triangles;
                mesh12.uv = uvs;

                mesh12.RecalculateNormals();

                MeshRenderer meshRenderer12 = quadObj12.AddComponent<MeshRenderer>();
                meshRenderer12.materials = new Material[] { mat };

                MeshCollider collider12 = quadObj12.AddComponent<MeshCollider>();
                collider12.convex = true;

                quadObj12.layer = LayerMask.NameToLayer("Floor");
            }

            



            currentLength += partLen + (offCenter2-offCenter1).magnitude;

            partLen = (controlPointsList[c+1].transform.position-offCenter2).magnitude;

            // --- Second Triangle ---

            if((controlPointsList[c+1].transform.position - offCenter2).magnitude > 0.01f)
            {
                vertices = new Vector3[]
                {
                    offCenter2 + width2 * mainRoadOrtho,
                    offCenter2 - width2 * mainRoadOrtho,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2,
                    controlPointsList[c+1].transform.position + width2 * roadOrtho2,

                    offCenter2 - width2 * mainRoadOrtho,
                    offCenter2 - width2 * mainRoadOrtho - height2 * roadNormal,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2 - height2 * roadNormal,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2,

                    offCenter2 - width2 * mainRoadOrtho - height2 * roadNormal,
                    offCenter2 + width2 * mainRoadOrtho - height2 * roadNormal,
                    controlPointsList[c+1].transform.position + width2 * roadOrtho2 - height2 * roadNormal,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2 - height2 * roadNormal,

                    offCenter2 + width2 * mainRoadOrtho - height2 * roadNormal,
                    offCenter2 + width2 * mainRoadOrtho,
                    controlPointsList[c+1].transform.position + width2 * roadOrtho2 - height2 * roadNormal,
                    controlPointsList[c+1].transform.position + width2 * roadOrtho2,

                    controlPointsList[c+1].transform.position + width2 * roadOrtho2 - height2 * roadNormal,
                    controlPointsList[c+1].transform.position + width2 * roadOrtho2,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2 - height2 * roadNormal,
                };

                normals = new Vector3[]
                {

                };

                uvs = new Vector2[]
                {
                    new Vector2(lateralOffset, currentLength),
                    new Vector2(lateralOffset + 2*controlPointsList[c].width, currentLength),
                    new Vector2(lateralOffset + 2*controlPointsList[c+1].width, currentLength + partLen),
                    new Vector2(lateralOffset, currentLength + partLen),

                    new Vector2(14, currentLength),
                    new Vector2(16, currentLength),
                    new Vector2(16, currentLength + partLen),
                    new Vector2(14, currentLength + partLen),


                    new Vector2(lateralOffset + 2*controlPointsList[c].width, currentLength),
                    new Vector2(lateralOffset, currentLength),
                    new Vector2(lateralOffset, currentLength + partLen),
                    new Vector2(lateralOffset + 2*controlPointsList[c+1].width, currentLength + partLen),

                    new Vector2(16, currentLength),
                    new Vector2(14, currentLength),
                    new Vector2(14, currentLength + partLen),
                    new Vector2(16, currentLength + partLen),


                    new Vector2(lateralOffset + 2*controlPointsList[c].width, 14),
                    new Vector2(lateralOffset + 2*controlPointsList[c].width, 16),
                    new Vector2(lateralOffset, 16),
                    new Vector2(lateralOffset, 14),
                };

                triangles = new int[]
                {
                    0,1,2,
                    0,2,3,
                    4,5,6,
                    4,6,7,
                    8,9,10,
                    8,10,11,
                    12,13,14,
                    12,14,15,
                    16,17,18,
                    16,18,19,
                };


                GameObject quadObj2 = new GameObject("QuadObj2");
                quadObj2.transform.parent = meshParent.transform;
                MeshFilter meshFilter2 = quadObj2.AddComponent<MeshFilter>();
                Mesh mesh2 = meshFilter2.mesh;
                mesh2.Clear();

                mesh2.vertices = vertices;
                mesh2.triangles = triangles;
                mesh2.uv = uvs;

                mesh2.RecalculateNormals();

                MeshRenderer meshRenderer2 = quadObj2.AddComponent<MeshRenderer>();
                meshRenderer2.materials = new Material[] { mat };

                MeshCollider collider2 = quadObj2.AddComponent<MeshCollider>();
                collider2.convex = true;

                quadObj2.layer = LayerMask.NameToLayer("Floor");
            }


            // Border 1

            if ((controlPointsList[c + 1].transform.position - offCenter2).magnitude > 0.01f)
            {
                vertices = new Vector3[]
                {
                    offCenter2 + width2 * mainRoadOrtho + border2 * roadNormal,
                    offCenter2 + (width2-height2) * mainRoadOrtho + border2 * roadNormal,
                    controlPointsList[c+1].transform.position + (width2-height2) * roadOrtho2 + border2 * roadNormal,
                    controlPointsList[c+1].transform.position + width2 * roadOrtho2 + border2 * roadNormal,

                    offCenter2 + (width2-height2) * mainRoadOrtho + border2 * roadNormal,
                    offCenter2 + (width2-height2) * mainRoadOrtho,
                    controlPointsList[c+1].transform.position + (width2-height2) * roadOrtho2,
                    controlPointsList[c+1].transform.position + (width2-height2) * roadOrtho2 + border2 * roadNormal,

                    offCenter2 + (width2-height2) * mainRoadOrtho,
                    offCenter2 + width2 * mainRoadOrtho,
                    controlPointsList[c+1].transform.position + width2 * roadOrtho2,
                    controlPointsList[c+1].transform.position + (width2-height2) * roadOrtho2,

                    offCenter2 + width2 * mainRoadOrtho,
                    offCenter2 + width2 * mainRoadOrtho + border2 * roadNormal,
                    controlPointsList[c+1].transform.position + width2 * roadOrtho2 + border2 * roadNormal,
                    controlPointsList[c+1].transform.position + width2 * roadOrtho2,

                    controlPointsList[c+1].transform.position + width2 * roadOrtho2,
                    controlPointsList[c+1].transform.position + width2 * roadOrtho2 + border2 * roadNormal,
                    controlPointsList[c+1].transform.position + (width2-height2) * roadOrtho2 + border2 * roadNormal,
                    controlPointsList[c+1].transform.position + (width2-height2) * roadOrtho2,
                };

                normals = new Vector3[]
                {

                };

                uvs = new Vector2[]
                {
                    new Vector2(height1, currentLength),
                    new Vector2(0, currentLength),
                    new Vector2(0, currentLength + partLen),
                    new Vector2(height2, currentLength + partLen),

                    new Vector2(0, currentLength),
                    new Vector2(border1, currentLength),
                    new Vector2(border2, currentLength + partLen),
                    new Vector2(0, currentLength + partLen),

                    new Vector2(14, currentLength),
                    new Vector2(16, currentLength),
                    new Vector2(16, currentLength + partLen),
                    new Vector2(14, currentLength + partLen),

                    new Vector2(height1+border1, currentLength),
                    new Vector2(height1, currentLength),
                    new Vector2(height2, currentLength + partLen),
                    new Vector2(height2+border2, currentLength + partLen),

                    new Vector2(border1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, height1),
                    new Vector2(border1, height1),
                };

                triangles = new int[]
                {
                    0,1,2,
                    0,2,3,
                    4,5,6,
                    4,6,7,
                    8,9,10,
                    8,10,11,
                    12,13,14,
                    12,14,15,
                    16,17,18,
                    16,18,19,
                };


                GameObject quadObj21 = new GameObject("QuadObj21");
                quadObj21.transform.parent = meshParent.transform;
                MeshFilter meshFilter21 = quadObj21.AddComponent<MeshFilter>();
                Mesh mesh21 = meshFilter21.mesh;
                mesh21.Clear();

                mesh21.vertices = vertices;
                mesh21.triangles = triangles;
                mesh21.uv = uvs;

                mesh21.RecalculateNormals();

                MeshRenderer meshRenderer21 = quadObj21.AddComponent<MeshRenderer>();
                meshRenderer21.materials = new Material[] { mat };

                MeshCollider collider21 = quadObj21.AddComponent<MeshCollider>();
                collider21.convex = true;

                quadObj21.layer = LayerMask.NameToLayer("Floor");
            }


            // Border 2

            if ((controlPointsList[c + 1].transform.position - offCenter2).magnitude > 0.01f)
            {
                vertices = new Vector3[]
                {   
                    offCenter2 - (width2-height2) * mainRoadOrtho + border2 * roadNormal,
                    offCenter2 - width2 * mainRoadOrtho + border2 * roadNormal,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2 + border2 * roadNormal,
                    controlPointsList[c+1].transform.position - (width2-height2) * roadOrtho2 + border2 * roadNormal,

                    offCenter2 - width2 * mainRoadOrtho + border2 * roadNormal,
                    offCenter2 - width2 * mainRoadOrtho,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2 + border2 * roadNormal,

                    offCenter2 - width2 * mainRoadOrtho,
                    offCenter2 - (width2-height2) * mainRoadOrtho,
                    controlPointsList[c+1].transform.position - (width2-height2) * roadOrtho2,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2,

                    offCenter2 - (width2-height2) * mainRoadOrtho,
                    offCenter2 - (width2-height2) * mainRoadOrtho + border2 * roadNormal,
                    controlPointsList[c+1].transform.position - (width2-height2) * roadOrtho2 + border2 * roadNormal,
                    controlPointsList[c+1].transform.position - (width2-height2) * roadOrtho2,

                    controlPointsList[c+1].transform.position - (width2-height2) * roadOrtho2,
                    controlPointsList[c+1].transform.position - (width2-height2) * roadOrtho2 + border2 * roadNormal,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2 + border2 * roadNormal,
                    controlPointsList[c+1].transform.position - width2 * roadOrtho2,
                };

                normals = new Vector3[]
                {

                };

                uvs = new Vector2[]
                {
                    new Vector2(0, currentLength),
                    new Vector2(height1, currentLength),
                    new Vector2(height2, currentLength + partLen),
                    new Vector2(0, currentLength + partLen),

                    new Vector2(height1, currentLength),
                    new Vector2(height1+border1, currentLength),
                    new Vector2(height2+border2, currentLength + partLen),
                    new Vector2(height2, currentLength + partLen),

                    new Vector2(14, currentLength),
                    new Vector2(16, currentLength),
                    new Vector2(16, currentLength + partLen),
                    new Vector2(14, currentLength + partLen),

                    new Vector2(border1, currentLength),
                    new Vector2(0, currentLength),
                    new Vector2(0, currentLength + partLen),
                    new Vector2(border2, currentLength + partLen),

                    new Vector2(border1, height1),
                    new Vector2(0, height1),
                    new Vector2(0, 0),
                    new Vector2(border1, 0),
                };

                triangles = new int[]
                {
                    0,1,2,
                    0,2,3,
                    4,5,6,
                    4,6,7,
                    8,9,10,
                    8,10,11,
                    12,13,14,
                    12,14,15,
                    16,17,18,
                    16,18,19,
                };


                GameObject quadObj22 = new GameObject("QuadObj22");
                quadObj22.transform.parent = meshParent.transform;
                MeshFilter meshFilter22 = quadObj22.AddComponent<MeshFilter>();
                Mesh mesh22 = meshFilter22.mesh;
                mesh22.Clear();

                mesh22.vertices = vertices;
                mesh22.triangles = triangles;
                mesh22.uv = uvs;

                mesh22.RecalculateNormals();

                MeshRenderer meshRenderer22 = quadObj22.AddComponent<MeshRenderer>();
                meshRenderer22.materials = new Material[] { mat };

                MeshCollider collider22 = quadObj22.AddComponent<MeshCollider>();
                collider22.convex = true;

                quadObj22.layer = LayerMask.NameToLayer("Floor");
            }

            currentLength -= (offCenter2 - offCenter1).magnitude;

            partLen = (offCenter2 - offCenter1).magnitude;

            // --- Flat Zone ---

            vertices = new Vector3[]
            {
                offCenter1 + width1 * mainRoadOrtho,
                offCenter1 - width1 * mainRoadOrtho,
                offCenter2 - width2 * mainRoadOrtho,
                offCenter2 + width2 * mainRoadOrtho,

                offCenter1 - width1 * mainRoadOrtho,
                offCenter1 - width1 * mainRoadOrtho - height1 * roadNormal,
                offCenter2 - width2 * mainRoadOrtho - height2 * roadNormal,
                offCenter2 - width2 * mainRoadOrtho,

                offCenter1 - width1 * mainRoadOrtho - height1 * roadNormal,
                offCenter1 + width1 * mainRoadOrtho - height1 * roadNormal,
                offCenter2 + width2 * mainRoadOrtho - height2 * roadNormal,
                offCenter2 - width2 * mainRoadOrtho - height2 * roadNormal,

                offCenter1 + width1 * mainRoadOrtho - height1 * roadNormal,
                offCenter1 + width1 * mainRoadOrtho,
                offCenter2 + width2 * mainRoadOrtho,
                offCenter2 + width2 * mainRoadOrtho - height2 * roadNormal,


                offCenter1 - width1 * mainRoadOrtho - height1 * roadNormal,
                offCenter1 - width1 * mainRoadOrtho,
                offCenter1 + width1 * mainRoadOrtho,
                offCenter1 + width1 * mainRoadOrtho - height1 * roadNormal,

                offCenter2 + width2 * mainRoadOrtho - height2 * roadNormal,
                offCenter2 + width2 * mainRoadOrtho,
                offCenter2 - width2 * mainRoadOrtho,
                offCenter2 - width2 * mainRoadOrtho - height2 * roadNormal,
            };

            normals = new Vector3[]
            {

            };

            uvs = new Vector2[]
            {
                new Vector2(lateralOffset, currentLength),
                new Vector2(lateralOffset + 2*width1, currentLength),
                new Vector2(lateralOffset + 2*width2, currentLength + partLen),
                new Vector2(lateralOffset, currentLength + partLen),

                new Vector2(14, currentLength),
                new Vector2(16, currentLength),
                new Vector2(16, currentLength + partLen),
                new Vector2(14, currentLength + partLen),


                new Vector2(lateralOffset + 2*width1, currentLength),
                new Vector2(lateralOffset, currentLength),
                new Vector2(lateralOffset, currentLength + partLen),
                new Vector2(lateralOffset + 2*width2, currentLength + partLen),

                new Vector2(16, currentLength),
                new Vector2(14, currentLength),
                new Vector2(14, currentLength + partLen),
                new Vector2(16, currentLength + partLen),


                new Vector2(lateralOffset + 2*width1, 14),
                new Vector2(lateralOffset + 2*width1, 16),
                new Vector2(lateralOffset, 16),
                new Vector2(lateralOffset, 14),

                new Vector2(lateralOffset, 14),
                new Vector2(lateralOffset, 16),
                new Vector2(lateralOffset + 2*width2, 16),
                new Vector2(lateralOffset + 2*width2, 14),
            };

            triangles = new int[]
            {
                0,1,2,
                0,2,3,
                4,5,6,
                4,6,7,
                8,9,10,
                8,10,11,
                12,13,14,
                12,14,15,
                16,17,18,
                16,18,19,
                20,21,22,
                20,22,23,
            };

            GameObject quadObj3 = new GameObject("QuadObj3");
            quadObj3.transform.parent = meshParent.transform;
            MeshFilter meshFilter3 = quadObj3.AddComponent<MeshFilter>();
            Mesh mesh3 = meshFilter3.mesh;
            mesh3.Clear();

            mesh3.vertices = vertices;
            mesh3.triangles = triangles;
            mesh3.uv = uvs;

            mesh3.RecalculateNormals();

            MeshRenderer meshRenderer3 = quadObj3.AddComponent<MeshRenderer>();
            meshRenderer3.materials = new Material[] { mat };

            MeshCollider collider3 = quadObj3.AddComponent<MeshCollider>();
            collider3.convex = true;

            quadObj3.layer = LayerMask.NameToLayer("Floor");


            // Border 1

            vertices = new Vector3[]
            {
                offCenter1 + width1 * mainRoadOrtho + border1 * roadNormal,
                offCenter1 + (width1-height1) * mainRoadOrtho + border1 * roadNormal,
                offCenter2 + (width2-height2) * mainRoadOrtho + border2 * roadNormal,
                offCenter2 + width2 * mainRoadOrtho + border2 * roadNormal,

                offCenter1 + (width1-height1) * mainRoadOrtho + border1 * roadNormal,
                offCenter1 + (width1-height1) * mainRoadOrtho,
                offCenter2 + (width2-height2) * mainRoadOrtho,
                offCenter2 + (width2-height2) * mainRoadOrtho + border2 * roadNormal,

                offCenter1 + (width1-height1) * mainRoadOrtho,
                offCenter1 + width1 * mainRoadOrtho,
                offCenter2 + width2 * mainRoadOrtho,
                offCenter2 + (width2-height2) * mainRoadOrtho,

                offCenter1 + width1 * mainRoadOrtho,
                offCenter1 + width1 * mainRoadOrtho + border1 * roadNormal,
                offCenter2 + width2 * mainRoadOrtho + border2 * roadNormal,
                offCenter2 + width2 * mainRoadOrtho,

                offCenter1 + (width1-height1) * mainRoadOrtho,
                offCenter1 + (width1-height1) * mainRoadOrtho + border1 * roadNormal,
                offCenter1 + width1 * mainRoadOrtho + border1 * roadNormal,
                offCenter1 + width1 * mainRoadOrtho,

                offCenter2 + width2 * mainRoadOrtho,
                offCenter2 + width2 * mainRoadOrtho + border2 * roadNormal,
                offCenter2 + (width2-height2) * mainRoadOrtho + border2 * roadNormal,
                offCenter2 + (width2-height2) * mainRoadOrtho,
            };


            normals = new Vector3[]
            {

            };

            uvs = new Vector2[]
            {
                new Vector2(height1, currentLength),
                new Vector2(0, currentLength),
                new Vector2(0, currentLength + partLen),
                new Vector2(height2, currentLength + partLen),

                new Vector2(0, currentLength),
                new Vector2(border1, currentLength),
                new Vector2(border2, currentLength + partLen),
                new Vector2(0, currentLength + partLen),

                new Vector2(14, currentLength),
                new Vector2(16, currentLength),
                new Vector2(16, currentLength + partLen),
                new Vector2(14, currentLength + partLen),

                new Vector2(height1+border1, currentLength),
                new Vector2(height1, currentLength),
                new Vector2(height2, currentLength + partLen),
                new Vector2(height2+border2, currentLength + partLen),

                new Vector2(border1, 0),
                new Vector2(0, 0),
                new Vector2(0, height1),
                new Vector2(border1, height1),

                new Vector2(border2, height2),
                new Vector2(0, height2),
                new Vector2(0, 0),
                new Vector2(border2, 0),
            };

            triangles = new int[]
            {
                0,1,2,
                0,2,3,
                4,5,6,
                4,6,7,
                8,9,10,
                8,10,11,
                12,13,14,
                12,14,15,
                16,17,18,
                16,18,19,
                20,21,22,
                20,22,23,
            };


            GameObject quadObj31 = new GameObject("QuadObj31");
            quadObj31.transform.parent = meshParent.transform;
            MeshFilter meshFilter31 = quadObj31.AddComponent<MeshFilter>();
            Mesh mesh31 = meshFilter31.mesh;
            mesh31.Clear();

            mesh31.vertices = vertices;
            mesh31.triangles = triangles;
            mesh31.uv = uvs;

            mesh31.RecalculateNormals();

            MeshRenderer meshRenderer31 = quadObj31.AddComponent<MeshRenderer>();
            meshRenderer31.materials = new Material[] { mat };

            MeshCollider collider31 = quadObj31.AddComponent<MeshCollider>();
            collider31.convex = true;

            quadObj31.layer = LayerMask.NameToLayer("Floor");

            // Border 2

            vertices = new Vector3[]
            {
                offCenter1 - (width1-height1) * mainRoadOrtho + border1 * roadNormal,
                offCenter1 - width1 * mainRoadOrtho + border1 * roadNormal,
                offCenter2 - width2 * mainRoadOrtho + border2 * roadNormal,
                offCenter2 - (width2-height2) * mainRoadOrtho + border2 * roadNormal,

                offCenter1 - width1 * mainRoadOrtho + border1 * roadNormal,
                offCenter1 - width1 * mainRoadOrtho,
                offCenter2 - width2 * mainRoadOrtho,
                offCenter2 - width2 * mainRoadOrtho + border2 * roadNormal,

                offCenter1 - width1 * mainRoadOrtho,
                offCenter1 - (width1-height1) * mainRoadOrtho,
                offCenter2 - (width2-height2) * mainRoadOrtho,
                offCenter2 - width2 * mainRoadOrtho,

                offCenter1 - (width1-height1) * mainRoadOrtho,
                offCenter1 - (width1-height1) * mainRoadOrtho + border1 * roadNormal,
                offCenter2 - (width2-height2) * mainRoadOrtho + border2 * roadNormal,
                offCenter2 - (width2-height2) * mainRoadOrtho,

                offCenter1 - width1 * mainRoadOrtho,
                offCenter1 - width1 * mainRoadOrtho + border1 * roadNormal,
                offCenter1 - (width1-height1) * mainRoadOrtho + border1 * roadNormal,
                offCenter1 - (width1-height1) * mainRoadOrtho,

                offCenter2 - (width2-height2) * mainRoadOrtho,
                offCenter2 - (width2-height2) * mainRoadOrtho + border2 * roadNormal,
                offCenter2 - width2 * mainRoadOrtho + border2 * roadNormal,
                offCenter2 - width2 * mainRoadOrtho,
            };


            normals = new Vector3[]
            {

            };

            uvs = new Vector2[]
            {
                new Vector2(0, currentLength),
                new Vector2(height1, currentLength),
                new Vector2(height2, currentLength + partLen),
                new Vector2(0, currentLength + partLen),

                new Vector2(height1, currentLength),
                new Vector2(height1+border1, currentLength),
                new Vector2(height2+border2, currentLength + partLen),
                new Vector2(height2, currentLength + partLen),

                new Vector2(14, currentLength),
                new Vector2(16, currentLength),
                new Vector2(16, currentLength + partLen),
                new Vector2(14, currentLength + partLen),

                new Vector2(border1, currentLength),
                new Vector2(0, currentLength),
                new Vector2(0, currentLength + partLen),
                new Vector2(border2, currentLength + partLen),

                new Vector2(border1, height1),
                new Vector2(0, height1),
                new Vector2(0, 0),
                new Vector2(border1, 0),

                new Vector2(border2, 0),
                new Vector2(0, 0),
                new Vector2(0, height2),
                new Vector2(border2, height2),
            };

            triangles = new int[]
            {
                0,1,2,
                0,2,3,
                4,5,6,
                4,6,7,
                8,9,10,
                8,10,11,
                12,13,14,
                12,14,15,
                16,17,18,
                16,18,19,
                20,21,22,
                20,22,23,
            };



            GameObject quadObj32 = new GameObject("QuadObj32");
            quadObj32.transform.parent = meshParent.transform;
            MeshFilter meshFilter32 = quadObj32.AddComponent<MeshFilter>();
            Mesh mesh32 = meshFilter32.mesh;
            mesh32.Clear();

            mesh32.vertices = vertices;
            mesh32.triangles = triangles;
            mesh32.uv = uvs;

            mesh32.RecalculateNormals();

            MeshRenderer meshRenderer32 = quadObj32.AddComponent<MeshRenderer>();
            meshRenderer32.materials = new Material[] { mat };

            MeshCollider collider32 = quadObj32.AddComponent<MeshCollider>();
            collider32.convex = true;

            quadObj32.layer = LayerMask.NameToLayer("Floor");


            currentLength += (offCenter2 - offCenter1).magnitude + (controlPointsList[c+1].transform.position - offCenter2).magnitude;
        }
    }



    public List<Vector3> GetHermitInterpolationPoints(Vector3 pos0, Vector3 vel0, Vector3 pos1, Vector3 vel1, int nbPoints)
    {
        Vector3 a = 2 * pos0 - 2 * pos1 + vel0 + vel1;
        Vector3 b = -3 * pos0 + 3 * pos1 - 2 * vel0 - vel1;
        Vector3 c = vel0;
        Vector3 d = pos0;

        List<Vector3> listPos = new List<Vector3>();
        for(int i=1; i<nbPoints+1; i++)
        {
            float t = ((float)i / ((float)(nbPoints + 1)));
            Vector3 newPos = (t * t * t) * a + (t * t) * b + t * c + d;
            listPos.Add(newPos);
        }

        return listPos;
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        List<Vector3> velocityList = new List<Vector3>();
        Vector3 vel = controlPointsList[1].transform.position - controlPointsList[0].transform.position;
        velocityList.Add(0.5f * vel);
        for (int i = 1; i < controlPointsList.Count - 1; i++)
        {
            vel = controlPointsList[i + 1].transform.position - controlPointsList[i - 1].transform.position;
            velocityList.Add(0.5f * vel);
        }
        vel = controlPointsList[controlPointsList.Count - 1].transform.position - controlPointsList[controlPointsList.Count - 2].transform.position;
        velocityList.Add(0.5f * vel);

        List<Vector3> points = new List<Vector3>();
        points.Add(controlPointsList[0].transform.position);
        for (int i = 1; i < controlPointsList.Count; i++)
        {
            List<Vector3> listInterpolatedPoints = GetHermitInterpolationPoints(controlPointsList[i - 1].transform.position, velocityList[i - 1], controlPointsList[i].transform.position, velocityList[i], pointResolution);
            points.AddRange(listInterpolatedPoints);
            points.Add(controlPointsList[i].transform.position);
        }

        float width = 0.0f;
        if(mode == GeneratorMode.Road)
        {
            width = controlPointsList[0].width;
        }
        else if(mode == GeneratorMode.Tunnel)
        {
            width = controlPointsList[0].externRadius;
        }
        for(int j = 0; j<points.Count-1; j++)
        {
            Vector3 pos = points[j];
            Vector3 vecZ = points[j + 1] - points[j];
            vecZ = vecZ.normalized;
            Vector3 vecX = Vector3.Cross(Vector3.up, vecZ);
            vecX = vecX.normalized;
            Vector3 vecY = Vector3.Cross(vecX, vecZ);
            vecY = vecY.normalized;

            Vector3 x = width * vecX;
            Vector3 y = width * vecY;
            Vector3 z = (points[j+1]-points[j]);

            Gizmos.DrawLine(pos + x, pos + y);
            Gizmos.DrawLine(pos - x, pos + y);
            Gizmos.DrawLine(pos - x, pos - y);
            Gizmos.DrawLine(pos + x, pos - y);

            Gizmos.DrawLine(pos + x, pos + x + z); 
            Gizmos.DrawLine(pos + y, pos + y + z);
            Gizmos.DrawLine(pos - x, pos - x + z);
            Gizmos.DrawLine(pos - y, pos - y + z);

            Gizmos.DrawLine(pos + x + z, pos + y + z);
            Gizmos.DrawLine(pos - x + z, pos + y + z);
            Gizmos.DrawLine(pos - x + z, pos - y + z);
            Gizmos.DrawLine(pos + x + z, pos - y + z);
        }
    }
}
