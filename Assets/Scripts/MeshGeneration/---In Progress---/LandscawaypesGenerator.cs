using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscawaypesGenerator : MonoBehaviour
{
    [Header("--- Primary Settings ---")]
    public int width;
    public int depth;
    public float maxHeight;
    public float resolution;

    public Material mat;

    void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        Quaternion rot = this.transform.rotation;
        this.transform.rotation = Quaternion.identity;

        GenerateLandscawaypes();

        this.transform.rotation = rot;
    }

    public void GenerateLandscawaypes()
    {
        Vector3[,] points = new Vector3[width,depth];

        for(int i=0; i<width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                points[i, j] = GetRandom(i, j) * maxHeight * Vector3.up + i * resolution * Vector3.right + j * resolution * Vector3.forward;
            }
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        int offset = 0;

        for (int i = 0; i < width - 1; i++)
        {
            for (int j = 0; j < depth - 1; j++)
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
        float distCoeff = Mathf.Min(Mathf.Min(x, width - x - 1) * Mathf.Min(y, depth - y - 1) / (0.25f*width*depth), 1.0f);
        return Random.Range(0.0f, 1.0f) * distCoeff;
    }

}
