using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicMeshConverter : MonoBehaviour
{
    private MeshFilter meshFilter;
    public Material mat;
    public float gapLength;

    private void Awake()
    {
        meshFilter = this.GetComponent<MeshFilter>();
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        mesh.Clear();

        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUvs = new List<Vector2>();
        List<Vector2> newUvs2 = new List<Vector2>();

        int off = 0;
        for (int i = 0; i < triangles.Length / 3; i++)
        {


            TriangleGenerator.TriMesh triMesh = TriangleGenerator.GetTriMesh(off, vertices[triangles[3 * i + 0]], vertices[triangles[3 * i + 1]], vertices[triangles[3 * i + 2]], true, true, true, gapLength);
            newVertices.Add(triMesh.vertices[0]);
            newVertices.Add(triMesh.vertices[1]);
            newVertices.Add(triMesh.vertices[2]);
            newTriangles.Add(triMesh.triangles[0]);
            newTriangles.Add(triMesh.triangles[1]);
            newTriangles.Add(triMesh.triangles[2]);
            newNormals.Add(triMesh.normals[0]);
            newNormals.Add(triMesh.normals[1]);
            newNormals.Add(triMesh.normals[2]);
            newUvs.Add(triMesh.uvs[0]);
            newUvs.Add(triMesh.uvs[1]);
            newUvs.Add(triMesh.uvs[2]);
            newUvs2.Add(triMesh.uvs2[0]);
            newUvs2.Add(triMesh.uvs2[1]);
            newUvs2.Add(triMesh.uvs2[2]);
            off += 3;
        }

        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.normals = newNormals.ToArray();
        mesh.uv = newUvs.ToArray();
        mesh.uv2 = newUvs2.ToArray();

        MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { mat };
    }
}
