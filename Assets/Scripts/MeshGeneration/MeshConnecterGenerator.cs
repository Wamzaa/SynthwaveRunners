using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static MeshGenerator;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MeshConnecterGenerator : MonoBehaviour
{
    public MeshGenerator roadIn;
    public MeshGenerator roadOut;

    public bool reversed;

    public int resolution;
    public int pointResolution;

    public Material mat;


    private Vector3[] vertices;
    private Vector2[] uvs;
    private Vector3[] normals;
    private int[] triangles;
    private bool done;

    private void Start()
    {
        done = false;
    }

    private void Update()
    {
        if (!done && roadOut.controlPointsList[0].GetRoadDir().magnitude != 0.0f && roadIn.controlPointsList.Last().GetRoadDir().magnitude != 0.0f)
        {
            SetUp();
            done = true;
        }
    }

    public void SetUp()
    {
        if(roadIn.mode == GeneratorMode.Road) 
        {
            if (roadOut.mode == GeneratorMode.Road)
            {
                BuildConnecter1();
            }
        }
    }

    private void BuildConnecter1()
    {
        MeshControlPoint pointIn = roadIn.controlPointsList.Last();
        MeshControlPoint pointOut = roadOut.controlPointsList[0];

        Vector3 pCenter1 = (roadIn.roadMode == RoadMode.FlexibleRoad) ? pointIn.transform.position : (pointIn.transform.position - pointIn.width * pointIn.GetRoadNormal());
        Vector3 pCenter2 = (roadOut.roadMode == RoadMode.FlexibleRoad) ? pointOut.transform.position : (pointOut.transform.position - pointOut.width * pointOut.GetRoadNormal());

        List<Vector3> pCenters = GetHermitInterpolationPoints(pCenter1, pointIn.GetRoadDir(), pCenter2, pointOut.GetRoadDir(), pointResolution);
        pCenters.Insert(0, pCenter1);
        pCenters.Add(pCenter2);

        List<Vector3> dirList = new List<Vector3>();
        dirList.Add(pointIn.GetRoadDir());
        for (int i = 1; i < pCenters.Count - 1; i++)
        {
            Vector3 dir = pCenters[i + 1] - pCenters[i - 1];
            dir = dir.normalized;
            dirList.Add(dir);
        }
        dirList.Add(pointOut.GetRoadDir());

        List<Vector3> roadNormals = new List<Vector3>();

        Vector3 oldProj = pointIn.GetRoadNormal();
        Vector3 proj = pointOut.GetRoadNormal();
        if (reversed)
        {
            proj = -proj;
        }

        roadNormals.Add(oldProj);

        for (int k = 0; k < pointResolution; k++)
        {
            float t = (k + 1) / ((float)(pointResolution + 1));
            Vector3 interNormal = (1 - t) * oldProj + t * proj;
            interNormal = interNormal.normalized;
            Vector3 interDir = dirList[k + 1];
            Vector3 interProj = interNormal - Vector3.Dot(interNormal, interDir) * interDir;
            if (interProj.magnitude == 0.0f)
            {
                interProj = Vector3.up - Vector3.Dot(Vector3.up, interDir) * interDir;
            }
            interProj = interProj.normalized;
            roadNormals.Add(interProj);
        }
        roadNormals.Add(proj);


        List<Vector3> roadLats = new List<Vector3>();
        for(int i =0; i<roadNormals.Count; i++)
        {
            Vector3 roadLat = Vector3.Cross(dirList[i], roadNormals[i]);
            roadLat = roadLat.normalized;
            roadLats.Add(roadLat);
        }


        List<Vector2> paramIn = new List<Vector2>();
        List<Vector2> paramIn2 = new List<Vector2>();
        float paramInUV = 0.0f;
        if (roadIn.roadMode == RoadMode.FlexibleRoad)
        {
            for(int i=0; i<=resolution; i++)
            {
                float distCenter = -pointIn.width + 2 * pointIn.width * (i / (float) resolution);
                paramIn.Add(new Vector2(distCenter, 0));
                paramIn2.Add(new Vector2(distCenter, -pointIn.height));
            }
            paramInUV = 2 * pointIn.width  / (float)resolution;
        }
        else
        {
            for (int i = 0; i <= resolution; i++)
            {
                float angle = -0.5f * Mathf.PI - pointIn.borderHeight * Mathf.PI + (2 * i * pointIn.borderHeight / resolution) * Mathf.PI;
                Vector2 dirDist = new Vector2(Mathf.Cos(angle), 1.0f + Mathf.Sin(angle));
                paramIn.Add(new Vector2(pointIn.width * Mathf.Cos(angle), pointIn.width + pointIn.width * Mathf.Sin(angle)));
                paramIn2.Add(new Vector2((pointIn.width + pointIn.height) * Mathf.Cos(angle), pointIn.width + (pointIn.width + pointIn.height) * Mathf.Sin(angle)));
            }
            paramInUV = 2 * Mathf.PI * pointIn.width * pointIn.borderHeight / (float)resolution;
        }

        /*for(int i=0; i<pCenters.Count; i++)
        {
            GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mark.transform.position = pCenters[i];
            GameObject mark1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mark1.transform.position = pCenters[i] + roadNormals[i];
        }*/

        List<Vector2> paramOut = new List<Vector2>();
        List<Vector2> paramOut2 = new List<Vector2>();
        float paramOutUV = 0.0f;
        if (roadOut.roadMode == RoadMode.FlexibleRoad)
        {
            for (int i = 0; i <= resolution; i++)
            {
                float distCenter = - pointOut.width + 2 * pointOut.width * (i / (float)resolution);
                if (!reversed)
                {
                    paramOut.Add(new Vector2(distCenter, 0));
                    paramOut2.Add(new Vector2(distCenter, -pointOut.height));
                }
                else
                {
                    paramOut.Add(new Vector2(distCenter, pointOut.height));
                    paramOut2.Add(new Vector2(distCenter, 0.0f));
                }
            }
            paramOutUV = 2 * pointOut.width / (float)resolution;
        }
        else
        {
            for (int i = 0; i <= resolution; i++)
            {
                float angle = -0.5f * Mathf.PI - pointOut.borderHeight * Mathf.PI + (2 * i * pointOut.borderHeight / resolution) * Mathf.PI;
                Vector2 dirDist = new Vector2(Mathf.Cos(angle), 1.0f + Mathf.Sin(angle));
                if (!reversed)
                {
                    paramOut.Add(new Vector2(pointOut.width * Mathf.Cos(angle), pointOut.width + pointOut.width * Mathf.Sin(angle)));
                    paramOut2.Add(new Vector2((pointOut.width + pointOut.height) * Mathf.Cos(angle), pointOut.width + (pointOut.width + pointOut.height) * Mathf.Sin(angle)));
                }
                else
                {
                    paramOut2.Add(new Vector2(pointOut.width * Mathf.Cos(angle), pointOut.width + pointOut.width * Mathf.Sin(angle)));
                    paramOut.Add(new Vector2((pointOut.width + pointOut.height) * Mathf.Cos(angle), pointOut.width + (pointOut.width + pointOut.height) * Mathf.Sin(angle)));
                }
            }
            paramOutUV = 2 * Mathf.PI * pointOut.width * pointOut.borderHeight / (float)resolution;
        }

        List<Vector3> verticesList = new List<Vector3>();
        List<Vector3> normalsList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<int> trianglesList = new List<int>();

        int indexCpt = 0;
        float currentLenght = 0.0f;

        float tInter = 1.0f / (float) (pCenters.Count - 1);
        for (int i = 0; i < pCenters.Count - 1; i++)
        {
            float partLen = (pCenters[i + 1] - pCenters[i]).magnitude;

            verticesList.Add(pCenters[i] + i * tInter * (paramOut2[0].x * roadLats[i] + paramOut2[0].y * roadNormals[i]) + (1.0f - (i * tInter)) * (paramIn2[0].x * roadLats[i] + paramIn2[0].y * roadNormals[i]));
            verticesList.Add(pCenters[i] + i * tInter * (paramOut[0].x * roadLats[i] + paramOut[0].y * roadNormals[i]) + (1.0f - (i * tInter)) * (paramIn[0].x * roadLats[i] + paramIn[0].y * roadNormals[i]));
            verticesList.Add(pCenters[i + 1] + (i + 1) * tInter * (paramOut[0].x * roadLats[i + 1] + paramOut[0].y * roadNormals[i + 1]) + (1.0f - ((i + 1) * tInter)) * (paramIn[0].x * roadLats[i + 1] + paramIn[0].y * roadNormals[i + 1]));
            verticesList.Add(pCenters[i + 1] + (i + 1) * tInter * (paramOut2[0].x * roadLats[i + 1] + paramOut2[0].y * roadNormals[i + 1]) + (1.0f - ((i + 1) * tInter)) * (paramIn2[0].x * roadLats[i + 1] + paramIn2[0].y * roadNormals[i + 1]));

            verticesList.Add(pCenters[i] + i * tInter * (paramOut[resolution].x * roadLats[i] + paramOut[resolution].y * roadNormals[i]) + (1.0f - (i * tInter)) * (paramIn[resolution].x * roadLats[i] + paramIn[resolution].y * roadNormals[i]));
            verticesList.Add(pCenters[i] + i * tInter * (paramOut2[resolution].x * roadLats[i] + paramOut2[resolution].y * roadNormals[i]) + (1.0f - (i * tInter)) * (paramIn2[resolution].x * roadLats[i] + paramIn2[resolution].y * roadNormals[i]));
            verticesList.Add(pCenters[i + 1] + (i + 1) * tInter * (paramOut2[resolution].x * roadLats[i + 1] + paramOut2[resolution].y * roadNormals[i + 1]) + (1.0f - ((i + 1) * tInter)) * (paramIn2[resolution].x * roadLats[i + 1] + paramIn2[resolution].y * roadNormals[i + 1]));
            verticesList.Add(pCenters[i + 1] + (i + 1) * tInter * (paramOut[resolution].x * roadLats[i + 1] + paramOut[resolution].y * roadNormals[i + 1]) + (1.0f - ((i + 1) * tInter)) * (paramIn[resolution].x * roadLats[i + 1] + paramIn[resolution].y * roadNormals[i + 1]));


            uvsList.Add(new Vector2(16, currentLenght));
            uvsList.Add(new Vector2(14, currentLenght));
            uvsList.Add(new Vector2(14, currentLenght + partLen));
            uvsList.Add(new Vector2(16, currentLenght + partLen));

            uvsList.Add(new Vector2(14, currentLenght));
            uvsList.Add(new Vector2(16, currentLenght));
            uvsList.Add(new Vector2(16, currentLenght + partLen));
            uvsList.Add(new Vector2(14, currentLenght + partLen));


            trianglesList.Add(indexCpt + 0);
            trianglesList.Add(indexCpt + 1);
            trianglesList.Add(indexCpt + 2);
            trianglesList.Add(indexCpt + 0);
            trianglesList.Add(indexCpt + 2);
            trianglesList.Add(indexCpt + 3);

            trianglesList.Add(indexCpt + 4);
            trianglesList.Add(indexCpt + 5);
            trianglesList.Add(indexCpt + 6);
            trianglesList.Add(indexCpt + 4);
            trianglesList.Add(indexCpt + 6);
            trianglesList.Add(indexCpt + 7);

            indexCpt += 8;

            float maxWidth = Mathf.Max(pointIn.width, pointOut.width);
            float lateralOffset = (Mathf.Ceil(2 * Mathf.PI * maxWidth / 20.0f) * 20.0f) / 2 + 0.5f;

            for (int j = 0; j < resolution; j++)
            {
                verticesList.Add(pCenters[i] + i * tInter * (paramOut[j].x * roadLats[i] + paramOut[j].y * roadNormals[i]) + (1.0f - (i * tInter)) * (paramIn[j].x * roadLats[i] + paramIn[j].y * roadNormals[i]));
                verticesList.Add(pCenters[i] + i * tInter * (paramOut[j + 1].x * roadLats[i] + paramOut[j + 1].y * roadNormals[i]) + (1.0f - (i * tInter)) * (paramIn[j + 1].x * roadLats[i] + paramIn[j + 1].y * roadNormals[i]));
                verticesList.Add(pCenters[i + 1] + (i + 1) * tInter * (paramOut[j + 1].x * roadLats[i + 1] + paramOut[j + 1].y * roadNormals[i + 1]) + (1.0f - ((i + 1) * tInter)) * (paramIn[j + 1].x * roadLats[i + 1] + paramIn[j + 1].y * roadNormals[i + 1]));
                verticesList.Add(pCenters[i + 1] + (i + 1) * tInter * (paramOut[j].x * roadLats[i + 1] + paramOut[j].y * roadNormals[i + 1]) + (1.0f - ((i + 1) * tInter)) * (paramIn[j].x * roadLats[i + 1] + paramIn[j].y * roadNormals[i + 1]));
                
                verticesList.Add(pCenters[i] + i * tInter * (paramOut2[j + 1].x * roadLats[i] + paramOut2[j + 1].y * roadNormals[i]) + (1.0f - (i * tInter)) * (paramIn2[j + 1].x * roadLats[i] + paramIn2[j + 1].y * roadNormals[i]));
                verticesList.Add(pCenters[i] + i * tInter * (paramOut2[j].x * roadLats[i] + paramOut2[j].y * roadNormals[i]) + (1.0f - (i * tInter)) * (paramIn2[j].x * roadLats[i] + paramIn2[j].y * roadNormals[i]));
                verticesList.Add(pCenters[i + 1] + (i + 1) * tInter * (paramOut2[j].x * roadLats[i + 1] + paramOut2[j].y * roadNormals[i + 1]) + (1.0f - ((i + 1) * tInter)) * (paramIn2[j].x * roadLats[i + 1] + paramIn2[j].y * roadNormals[i + 1]));
                verticesList.Add(pCenters[i + 1] + (i + 1) * tInter * (paramOut2[j + 1].x * roadLats[i + 1] + paramOut2[j + 1].y * roadNormals[i + 1]) + (1.0f - ((i + 1) * tInter)) * (paramIn2[j + 1].x * roadLats[i + 1] + paramIn2[j + 1].y * roadNormals[i + 1]));



                uvsList.Add(new Vector3(lateralOffset + (i * tInter * (j - 0.5f * resolution) * paramOutUV + (1.0f - (i * tInter)) * (j - 0.5f * resolution) * paramInUV), currentLenght));
                uvsList.Add(new Vector3(lateralOffset + (i * tInter * ((j + 1) - 0.5f * resolution) * paramOutUV + (1.0f - (i * tInter)) * ((j + 1) - 0.5f * resolution) * paramInUV), currentLenght));
                uvsList.Add(new Vector3(lateralOffset + ((i + 1) * tInter * ((j + 1) - 0.5f * resolution) * paramOutUV + (1.0f - ((i + 1) * tInter)) * ((j + 1) - 0.5f * resolution) * paramInUV), currentLenght + partLen));
                uvsList.Add(new Vector3(lateralOffset + ((i + 1) * tInter * (j - 0.5f * resolution) * paramOutUV + (1.0f - ((i + 1) * tInter)) * (j - 0.5f * resolution) * paramInUV), currentLenght + partLen));

                uvsList.Add(new Vector3(lateralOffset + (i * tInter * ((j + 1) - 0.5f * resolution) * paramOutUV + (1.0f - (i * tInter)) * ((j + 1) - 0.5f * resolution) * paramInUV), currentLenght));
                uvsList.Add(new Vector3(lateralOffset + (i * tInter * (j - 0.5f * resolution) * paramOutUV + (1.0f - (i * tInter)) * (j - 0.5f * resolution) * paramInUV), currentLenght));
                uvsList.Add(new Vector3(lateralOffset + ((i + 1) * tInter * (j - 0.5f * resolution) * paramOutUV + (1.0f - ((i + 1) * tInter)) * (j - 0.5f * resolution) * paramInUV), currentLenght + partLen));
                uvsList.Add(new Vector3(lateralOffset + ((i + 1) * tInter * ((j + 1) - 0.5f * resolution) * paramOutUV + (1.0f - ((i + 1) * tInter)) * ((j + 1) - 0.5f * resolution) * paramInUV), currentLenght + partLen));



                trianglesList.Add(indexCpt + 0);
                trianglesList.Add(indexCpt + 1);
                trianglesList.Add(indexCpt + 2);
                trianglesList.Add(indexCpt + 0);
                trianglesList.Add(indexCpt + 2);
                trianglesList.Add(indexCpt + 3);

                trianglesList.Add(indexCpt + 4);
                trianglesList.Add(indexCpt + 5);
                trianglesList.Add(indexCpt + 6);
                trianglesList.Add(indexCpt + 4);
                trianglesList.Add(indexCpt + 6);
                trianglesList.Add(indexCpt + 7);

                indexCpt += 8;
            }

            currentLenght += partLen;
        }


        GameObject quadObj = new GameObject("RoadMesh");
        MeshFilter meshFilter = quadObj.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();

        vertices = verticesList.ToArray();
        triangles = trianglesList.ToArray();
        uvs = uvsList.ToArray();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = quadObj.AddComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { mat };

        MeshCollider collider = quadObj.AddComponent<MeshCollider>();
        quadObj.isStatic = true;

        quadObj.layer = LayerMask.NameToLayer("Floor");
    }

    public List<Vector3> GetHermitInterpolationPoints(Vector3 pos0, Vector3 vel0, Vector3 pos1, Vector3 vel1, int nbPoints)
    {
        Vector3 a = 2 * pos0 - 2 * pos1 + vel0 + vel1;
        Vector3 b = -3 * pos0 + 3 * pos1 - 2 * vel0 - vel1;
        Vector3 c = vel0;
        Vector3 d = pos0;

        List<Vector3> listPos = new List<Vector3>();
        for (int i = 1; i < nbPoints + 1; i++)
        {
            float t = ((float)i / ((float)(nbPoints + 1)));
            Vector3 newPos = (t * t * t) * a + (t * t) * b + t * c + d;
            listPos.Add(newPos);
        }

        return listPos;
    }

}
