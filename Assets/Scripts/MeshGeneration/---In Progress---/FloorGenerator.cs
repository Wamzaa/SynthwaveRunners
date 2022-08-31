using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    public float[,] heightMap;
    public int resolution;
    public int scale;
    public float maxHeight;
    public Material mat;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector3[] normals;
    /*private Vector2[] uvs;*/

    private void Start()
    {
        //heightMap = new float[resolution, resolution];
        /*for(int i=0; i<resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float coordX = i / (float)resolution*20.0f;
                float coordY = j / (float)resolution*20.0f;
                heightMap[i, j] = maxHeight * Mathf.PerlinNoise(coordX, coordY) - (maxHeight/2);
                Debug.Log(Mathf.PerlinNoise(coordX, coordY));
            }
        }*/

        float[] tempList = new float[10]
        {
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f
        };

        heightMap = new float[10,10]
        {
            { 1.0f, 1.0f, 1.0f, 0.9f, 0.5f, 0.7f, 0.7f, 0.7f, 0.3f, 1.0f },
            { 1.0f, 0.8f, 0.8f, 0.6f, 0.4f, 0.6f, 0.7f, 0.7f, 0.2f, 0.1f },
            { 1.0f, 0.8f, 0.7f, 0.7f, 0.3f, 0.3f, 0.7f, 0.7f, 0.4f, 0.3f },
            { 0.9f, 0.6f, 0.5f, 0.5f, 0.2f, 0.1f, 0.5f, 0.7f, 0.7f, 0.6f },
            { 0.8f, 0.3f, 0.4f, 0.0f, 0.2f, 0.0f, 0.2f, 0.7f, 0.7f, 0.5f },
            { 0.6f, 0.2f, 0.1f, 0.0f, 0.1f, 0.0f, 0.1f, 0.2f, 0.7f, 0.4f },
            { 0.4f, 0.3f, 0.2f, 0.3f, 0.1f, 0.1f, 0.2f, 0.3f, 0.7f, 0.7f },
            { 0.2f, 0.0f, 0.5f, 0.6f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f },
            { 0.0f, 0.0f, 0.6f, 0.7f, 0.4f, 0.5f, 0.6f, 0.7f, 0.7f, 0.8f },
            { 0.5f, 0.1f, 0.7f, 0.8f, 0.6f, 0.5f, 0.7f, 0.6f, 0.8f, 0.9f }
        };

        GenerateFloorMesh();
    }

    public void GenerateFloorMesh()
    {
        GameObject floorParent = new GameObject("GeneratedFloor");
        floorParent.transform.position = this.transform.position;

        Vector3 startPos = this.transform.position - scale * (resolution/2) * Vector3.right - scale * (resolution / 2) * Vector3.forward;

        for (int i = 0; i < resolution-1; i++)
        {
            for (int j = 0; j < resolution-1; j++)
            {
                // ---------- 1 -------------

                GameObject floorSection1 = new GameObject("FloorSection-" + i + "/" + j + "_1");
                floorSection1.transform.parent = floorParent.transform;

                vertices = new Vector3[]{
                    startPos + (i+1) * scale * Vector3.right + j * scale * Vector3.forward + maxHeight * heightMap[i+1, j] * Vector3.up,
                    startPos + i * scale * Vector3.right + j * scale * Vector3.forward + maxHeight * heightMap[i, j] * Vector3.up,
                    startPos + i * scale * Vector3.right + (j+1) * scale * Vector3.forward + maxHeight * heightMap[i, j+1] * Vector3.up,
                    startPos + (i+1) * scale * Vector3.right + j * scale * Vector3.forward + maxHeight * (heightMap[i+1, j] - 1.0f) * Vector3.up,
                    startPos + i * scale * Vector3.right + j * scale * Vector3.forward + maxHeight * (heightMap[i, j] - 1.0f) * Vector3.up,
                    startPos + i * scale * Vector3.right + (j+1) * scale * Vector3.forward + maxHeight * (heightMap[i, j+1] - 1.0f) * Vector3.up
                };

                triangles = new int[]
                {
                    0,1,2,
                    3,5,4,
                    0,3,4,
                    0,4,1,
                    1,4,5,
                    1,5,2,
                    2,5,3,
                    2,3,0
                };

                //Vector3 normal1 = Vector3.Cross(,);

                MeshFilter meshFilter1 = floorSection1.AddComponent<MeshFilter>();
                Mesh mesh1 = meshFilter1.mesh;
                mesh1.Clear();

                mesh1.vertices = vertices;
                mesh1.triangles = triangles;

                mesh1.RecalculateNormals();

                MeshRenderer meshRenderer1 = floorSection1.AddComponent<MeshRenderer>();
                meshRenderer1.materials = new Material[] { mat };

                MeshCollider collider1 = floorSection1.AddComponent<MeshCollider>();
                collider1.convex = true;

                floorSection1.layer = LayerMask.NameToLayer("Floor");


                // ---------- 2 -------------

                GameObject floorSection2 = new GameObject("FloorSection-" + i + "/" + j + "_2");
                floorSection2.transform.parent = floorParent.transform;

                vertices = new Vector3[]{
                    startPos + (i+1) * scale * Vector3.right + j * scale * Vector3.forward + maxHeight * heightMap[i+1, j] * Vector3.up,
                    startPos + i * scale * Vector3.right + (j+1) * scale * Vector3.forward + maxHeight * heightMap[i, j+1] * Vector3.up,
                    startPos + (i+1) * scale * Vector3.right + (j+1) * scale * Vector3.forward + maxHeight * heightMap[i+1, j+1] * Vector3.up,
                    startPos + (i+1) * scale * Vector3.right + j * scale * Vector3.forward + maxHeight * (heightMap[i+1, j] - 1.0f) * Vector3.up,
                    startPos + i * scale * Vector3.right + (j+1) * scale * Vector3.forward + maxHeight * (heightMap[i, j+1] - 1.0f) * Vector3.up,
                    startPos + (i+1) * scale * Vector3.right + (j+1) * scale * Vector3.forward + maxHeight * (heightMap[i+1, j+1] - 1.0f) * Vector3.up
                };

                triangles = new int[]
                {
                    0,1,2,
                    3,5,4,
                    0,3,4,
                    0,4,1,
                    1,4,5,
                    1,5,2,
                    2,5,3,
                    2,3,0
                };

                //Vector3 normal1 = Vector3.Cross(,);

                MeshFilter meshFilter2 = floorSection2.AddComponent<MeshFilter>();
                Mesh mesh2 = meshFilter2.mesh;
                mesh2.Clear();

                mesh2.vertices = vertices;
                mesh2.triangles = triangles;

                mesh1.RecalculateNormals();

                MeshRenderer meshRenderer2 = floorSection2.AddComponent<MeshRenderer>();
                meshRenderer2.materials = new Material[] { mat };

                MeshCollider collider2 = floorSection2.AddComponent<MeshCollider>();
                collider2.convex = true;

                floorSection2.layer = LayerMask.NameToLayer("Floor");

            }
        }

    }

}
