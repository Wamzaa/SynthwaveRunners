using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscawaypesGenerator : MonoBehaviour
{
    [Header("--- Primary Settings ---")]
    public int resX;
    public int resY;
    public float scale;

    [Header("--- Randomness Settings ---")]
    public float maxHeight;
    public int seed;
    public float edgeReduction;

    public Material mat;

    void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        Quaternion rot = this.transform.rotation;
        this.transform.rotation = Quaternion.identity;

        if(seed == 0)
        {
            GenerateSeed();
        }

        GenerateLandscawaypes();

        this.transform.rotation = rot;
    }

    public void GenerateSeed()
    {
        seed = Random.Range(0, 10000);
    }

    public void GenerateLandscawaypes()
    {
        Vector3[,] points = new Vector3[resX, resY];

        for(int i=0; i< resX; i++)
        {
            for (int j = 0; j < resY; j++)
            {
                points[i, j] = GetRandom(i, j) * maxHeight * Vector3.up + i * scale * Vector3.right + j * scale * Vector3.forward;
            }
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        int offset = 0;

        for (int i = 0; i < resX - 1; i++)
        {
            for (int j = 0; j < resY - 1; j++)
            {
                vertices.Add(points[i, j]);
                vertices.Add(points[i+1, j]);
                vertices.Add(points[i+1, j+1]);

                vertices.Add(points[i, j]);
                vertices.Add(points[i+1, j+1]);
                vertices.Add(points[i, j+1]);

                triangles.Add(0 + offset); 
                triangles.Add(2 + offset); 
                triangles.Add(1 + offset);

                triangles.Add(3 + offset);
                triangles.Add(5 + offset);
                triangles.Add(4 + offset);

                offset += 6;
            }
        }

        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        Mesh mesh = mf.mesh;

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { mat };
    }

    public float GetRandom(int x, int y)
    {
        float distCoeff = Mathf.Min(Mathf.Min(x, resX - x - 1), edgeReduction) * Mathf.Min(Mathf.Min(y, resY - y - 1), edgeReduction) / (edgeReduction * edgeReduction);
        return Mathf.PerlinNoise(0.5f * x, 0.5f * y) * distCoeff;
    }

}
