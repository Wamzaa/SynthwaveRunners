using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderGenerator : MonoBehaviour
{
    public float radius;
    public float height;
    public int resolution;
    public bool hasVerticalLines;

    public float gapLength;

    public Material mat;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        GenerateCubePrimitive();
    }

    private void GenerateCubePrimitive()
    {
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUvs = new List<Vector2>();
        List<Vector2> newUvs2 = new List<Vector2>();

        Vector3 upPoint = (height / 2) * Vector3.up;
        Vector3 downPoint = - (height / 2) * Vector3.up;

        int off = 0;

        for (int i=0; i<resolution; i++)
        {
            Vector3 p1 = new Vector3(radius * Mathf.Cos(2 * Mathf.PI * i / resolution), height / 2, radius * Mathf.Sin(2 * Mathf.PI * i / resolution));
            Vector3 p2 = new Vector3(radius * Mathf.Cos(2 * Mathf.PI * i / resolution), -height / 2, radius * Mathf.Sin(2 * Mathf.PI * i / resolution));
            Vector3 p3 = new Vector3(radius * Mathf.Cos(2 * Mathf.PI * (i + 1) / resolution), height / 2, radius * Mathf.Sin(2 * Mathf.PI * (i + 1) / resolution));
            Vector3 p4 = new Vector3(radius * Mathf.Cos(2 * Mathf.PI * (i + 1) / resolution), -height / 2, radius * Mathf.Sin(2 * Mathf.PI * (i + 1) / resolution));

            TriangleGenerator.TriMesh triMesh = TriangleGenerator.GetTriMesh(off, p3, p1, upPoint, false, false, true, gapLength);
            newVertices.AddRange(triMesh.vertices);
            newTriangles.AddRange(triMesh.triangles);
            newNormals.AddRange(triMesh.normals);
            newUvs.AddRange(triMesh.uvs);
            newUvs2.AddRange(triMesh.uvs2);

            off += 3;

            triMesh = TriangleGenerator.GetTriMesh(off, p2, p1, p4, false, true, hasVerticalLines, gapLength);
            newVertices.AddRange(triMesh.vertices);
            newTriangles.AddRange(triMesh.triangles);
            newNormals.AddRange(triMesh.normals);
            newUvs.AddRange(triMesh.uvs);
            newUvs2.AddRange(triMesh.uvs2);

            off += 3;

            triMesh = TriangleGenerator.GetTriMesh(off, p4, p1, p3, true, hasVerticalLines, false, gapLength);
            newVertices.AddRange(triMesh.vertices);
            newTriangles.AddRange(triMesh.triangles);
            newNormals.AddRange(triMesh.normals);
            newUvs.AddRange(triMesh.uvs);
            newUvs2.AddRange(triMesh.uvs2);

            off += 3;

            triMesh = TriangleGenerator.GetTriMesh(off, p2, p4, downPoint, false, false, true, gapLength);
            newVertices.AddRange(triMesh.vertices);
            newTriangles.AddRange(triMesh.triangles);
            newNormals.AddRange(triMesh.normals);
            newUvs.AddRange(triMesh.uvs);
            newUvs2.AddRange(triMesh.uvs2);

            off += 3;
        }

        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();

        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.normals = newNormals.ToArray();
        mesh.uv = newUvs.ToArray();
        mesh.uv2 = newUvs2.ToArray();

        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { mat };
    }

    public static void SetupCylinderMesh(string name, Transform parent, Vector3 position, float radius, float height, int resolution, bool hasLines, float gapLength, Material mat)
    {
        GameObject newCylinder = new GameObject(name);
        newCylinder.transform.parent = parent;
        newCylinder.transform.position = position;
        CylinderGenerator newCylinderGenerator = newCylinder.AddComponent<CylinderGenerator>();
        newCylinderGenerator.mat = mat;
        newCylinderGenerator.gapLength = gapLength;
        newCylinderGenerator.radius = radius;
        newCylinderGenerator.height = height;
        newCylinderGenerator.resolution = resolution;
        newCylinderGenerator.hasVerticalLines = hasLines;
        newCylinderGenerator.Init();
    }

}
