using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapGenerator : MonoBehaviour
{
    public bool seeded;
    public float seed;

    //temp
    public Material mat;

    private int cpt;

    private void Awake()
    {
        if(seed == 0f)
        {
            seed = Random.Range(0.0f, 1.0f);
        }

        cpt = 0;
    }

    private void Start()
    {
        //GenerateListObject(MeshGenerator.GeneratorMode.Tunnel, 20, 30, 40, 100, 50, 15, 0, 30, 33, 5, 50, 10);
        GenerateFlatRoad(MeshGenerator.GeneratorMode.Road, 50, 100, 200, 500, 200, 30, 15, 0, 30, 33, 5, 50, 10, MeshGenerator.RoadMode.TornadoRoad);
        //GenerateFlatRoad(MeshGenerator.GeneratorMode.Tunnel, 50, 100, 200, 500, 500, 30, 15, 0, 200, 220, 5, 50, 10, MeshGenerator.RoadMode.TornadoRoad);
    }

    public float RandomFloat()
    {
        if (seeded)
        {
            cpt += 1;
            Debug.Log(Mathf.PerlinNoise(seed, cpt) + " --- " + cpt);
            return (Mathf.PerlinNoise(seed, cpt));
        }
        else
        {
            return (Random.Range(0.0f, 1.0f));
        }
    }

    public bool RandomBool()
    {
        return (RandomFloat() > 0.5f);
    }

    public int RandomIntRange(int min, int max)
    {
        int n = max - min + 1;
        int val = min + (int) Mathf.Floor(n * RandomFloat());
        if(val == max + 1)
        {
            val = max;
        }
        return (val);
    }

    public Vector2 RandomVector2()
    {
        float a = RandomFloat() - 0.5f;
        float b = RandomFloat() - 0.5f;
        Vector2 vec = new Vector2(a, b);
        vec = vec.normalized;
        return (vec);
    }

    public Vector3 RandomVector3()
    {
        float a = RandomFloat() - 0.5f;
        float b = RandomFloat() - 0.5f;
        float c = RandomFloat() - 0.5f;
        Vector3 vec = new Vector3(a, b, c);
        vec = vec.normalized;
        return (vec);
    }

    public void GenerateListObject(MeshGenerator.GeneratorMode mode, int minElemList, int maxElemList, int minLenght, int maxLenght, int resolution, int pointResolution, float initialRoadRotation, float internRadius, float externRadius, float height, float width, float borderHeight, MeshGenerator.RoadMode roadMode = MeshGenerator.RoadMode.TornadoRoad)
    {
        GameObject meshGen = new GameObject("MeshGenerator");
        MeshGenerator meshGenerator = meshGen.AddComponent<MeshGenerator>();
        meshGenerator.mode = mode;
        if(mode == MeshGenerator.GeneratorMode.Road)
        {
            meshGenerator.roadMode = roadMode;
        }
        meshGenerator.resolution = resolution;
        meshGenerator.pointResolution = pointResolution;
        meshGenerator.initialRoadRotation = initialRoadRotation;
        meshGenerator.mat = mat;

        int n = RandomIntRange(minElemList, maxElemList);
        List<MeshControlPoint> controlPointsList = new List<MeshControlPoint>();
        Vector3 prec = Vector3.zero;
        Vector3 current = new Vector3(1.0f, 0.0f, 0.0f);

        GameObject firstControlPoint = new GameObject("ControlPoint");
        firstControlPoint.transform.position = current;
        MeshControlPoint firstMeshControlPoint = firstControlPoint.AddComponent<MeshControlPoint>();
        firstMeshControlPoint.internRadius = internRadius;
        firstMeshControlPoint.externRadius = externRadius;
        firstMeshControlPoint.height = height;
        firstMeshControlPoint.width = width;
        firstMeshControlPoint.borderHeight = borderHeight;

        controlPointsList.Add(firstMeshControlPoint);

        for (int i=0; i<n; i++)
        {
            Vector3 oldDir = current - prec;
            oldDir = oldDir.normalized;
            Vector3 dir = RandomVector3();
            dir = dir + 0.7f * (oldDir);
            dir = dir.normalized;

            prec = current;
            current = prec + RandomIntRange(minLenght, maxLenght) * dir;

            GameObject controlPoint = new GameObject("ControlPoint");
            controlPoint.transform.position = current;
            MeshControlPoint meshControlPoint = controlPoint.AddComponent<MeshControlPoint>();
            meshControlPoint.internRadius = internRadius;
            meshControlPoint.externRadius = externRadius;
            meshControlPoint.height = height;
            meshControlPoint.width = width;
            meshControlPoint.borderHeight = borderHeight;

            controlPointsList.Add(meshControlPoint);
        }
        meshGenerator.controlPointsList = controlPointsList;

        meshGenerator.SetUp();
    }

    public void GenerateFlatRoad(MeshGenerator.GeneratorMode mode, int minElemList, int maxElemList, int minLenght, int maxLenght, int maxHeight, int resolution, int pointResolution, float initialRoadRotation, float internRadius, float externRadius, float height, float width, float borderHeight, MeshGenerator.RoadMode roadMode = MeshGenerator.RoadMode.TornadoRoad)
    {
        GameObject meshGen = new GameObject("MeshGenerator");
        MeshGenerator meshGenerator = meshGen.AddComponent<MeshGenerator>();
        meshGenerator.mode = mode;
        if (mode == MeshGenerator.GeneratorMode.Road)
        {
            meshGenerator.roadMode = roadMode;
        }
        meshGenerator.resolution = resolution;
        meshGenerator.pointResolution = pointResolution;
        meshGenerator.initialRoadRotation = initialRoadRotation;
        meshGenerator.mat = mat;

        int n = RandomIntRange(minElemList, maxElemList);
        List<MeshControlPoint> controlPointsList = new List<MeshControlPoint>();
        Vector3 prec = Vector3.zero;
        Vector3 current = new Vector3(1.0f, 0.0f, 0.0f);

        GameObject firstControlPoint = new GameObject("ControlPoint");
        firstControlPoint.transform.position = current;
        MeshControlPoint firstMeshControlPoint = firstControlPoint.AddComponent<MeshControlPoint>();
        firstMeshControlPoint.internRadius = internRadius;
        firstMeshControlPoint.externRadius = externRadius;
        firstMeshControlPoint.height = height;
        firstMeshControlPoint.width = width;
        firstMeshControlPoint.borderHeight = borderHeight;

        controlPointsList.Add(firstMeshControlPoint);

        for (int i = 0; i < n; i++)
        {
            Vector3 temp = current - prec;
            Vector2 oldDir = new Vector2(temp.x, temp.z);
            oldDir = oldDir.normalized;
            Vector2 dir = RandomVector2();
            dir = dir + 1.1f * (oldDir);
            dir = dir.normalized;

            prec = current;
            current = prec + RandomIntRange(minLenght, maxLenght) * new Vector3(dir.x, 0, dir.y);
            if (RandomBool())
            {
                current = current + RandomIntRange(-maxHeight, maxHeight) * Vector3.up;
            }

            GameObject controlPoint = new GameObject("ControlPoint");
            controlPoint.transform.position = current;
            MeshControlPoint meshControlPoint = controlPoint.AddComponent<MeshControlPoint>();
            meshControlPoint.internRadius = internRadius;
            meshControlPoint.externRadius = externRadius;
            meshControlPoint.height = height;
            meshControlPoint.width = width;
            meshControlPoint.borderHeight = borderHeight;

            controlPointsList.Add(meshControlPoint);
        }
        meshGenerator.controlPointsList = controlPointsList;

        meshGenerator.SetUp();
    }

}
