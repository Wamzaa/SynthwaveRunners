using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EllipseGenerator : MonoBehaviour
{
    public static Transform SetupEllipseMesh(string name, Transform parent, Vector3 position, float height, float width, float depth, int resolution, bool hasLines, float gapLength, Material triMat, Material squaMat)
    {
        GameObject newEllipse = new GameObject(name);
        newEllipse.transform.parent = parent;
        newEllipse.transform.position = position;

        TriMesh meshTri = new TriMesh();
        List<int> trianglesTri = new List<int>();
        List<int> trianglesSqua = new List<int>();

        Vector3 upPoint = (height / 2) * Vector3.up;
        Vector3 downPoint = -(height / 2) * Vector3.up;

        int off = 0;

        for (int i = 0; i < resolution; i++)
        {
            Vector3 p1 = new Vector3((width / 2) * Mathf.Cos(2 * Mathf.PI * i / resolution), height / 2, (depth / 2) * Mathf.Sin(2 * Mathf.PI * i / resolution));
            Vector3 p2 = new Vector3((width / 2) * Mathf.Cos(2 * Mathf.PI * i / resolution), -height / 2, (depth / 2) * Mathf.Sin(2 * Mathf.PI * i / resolution));
            Vector3 p3 = new Vector3((width / 2) * Mathf.Cos(2 * Mathf.PI * (i + 1) / resolution), height / 2, (depth / 2) * Mathf.Sin(2 * Mathf.PI * (i + 1) / resolution));
            Vector3 p4 = new Vector3((width / 2) * Mathf.Cos(2 * Mathf.PI * (i + 1) / resolution), -height / 2, (depth / 2) * Mathf.Sin(2 * Mathf.PI * (i + 1) / resolution));

            TriMesh triMesh = TriangleGenerator.GetTriMesh(off, p3, p1, upPoint, false, false, true, gapLength);
            trianglesTri.AddRange(triMesh.triangles);
            meshTri.Combine(triMesh);

            off += 3;

            triMesh = TriangleGenerator.GetTriMesh(off, p2, p4, downPoint, false, false, true, gapLength);
            trianglesTri.AddRange(triMesh.triangles);
            meshTri.Combine(triMesh);

            off += 3;

            triMesh = SquareGenerator.GetTriMesh(off, p2, p1, p3, p4, true, hasLines, gapLength);
            trianglesSqua.AddRange(triMesh.triangles);
            meshTri.Combine(triMesh);

            off += 4;


        }

        MeshFilter meshFilter = newEllipse.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();

        mesh.subMeshCount = 2;

        mesh.vertices = meshTri.vertices;

        mesh.SetTriangles(trianglesTri, 0);
        mesh.SetTriangles(trianglesSqua, 1);

        mesh.normals = meshTri.normals;
        mesh.uv = meshTri.uvs;
        mesh.uv2 = meshTri.uvs2;

        MeshRenderer meshRenderer = newEllipse.AddComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { triMat, squaMat };

        return newEllipse.transform;
    }

}
