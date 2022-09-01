using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcosphereGenerator : MonoBehaviour
{
    public static Transform SetupIcoSphereMesh(string name, Transform parent, Vector3 position, float radius, int resolution, float gapLength, Material mat)
    {
        GameObject newSphere = new GameObject(name);
        newSphere.transform.parent = parent;
        newSphere.transform.position = position;

        List<Vector3> icoVertices = new List<Vector3>();
        List<int> icoTris = new List<int>();

        float phi = (1.0f + Mathf.Sqrt(5.0f)) * 0.5f; // golden ratio
        float a = 1.0f;
        float b = 1.0f / phi;

        // add vertices
        icoVertices.Add(new Vector3(0.0f, b, -a));
        icoVertices.Add(new Vector3(b, a, 0));
        icoVertices.Add(new Vector3(-b, a, 0));
        icoVertices.Add(new Vector3(0, b, a));
        icoVertices.Add(new Vector3(0, -b, a));
        icoVertices.Add(new Vector3(-a, 0, b));
        icoVertices.Add(new Vector3(0, -b, -a));
        icoVertices.Add(new Vector3(a, 0, -b));
        icoVertices.Add(new Vector3(a, 0, b));
        icoVertices.Add(new Vector3(-a, 0, -b));
        icoVertices.Add(new Vector3(b, -a, 0));
        icoVertices.Add(new Vector3(-b, -a, 0));

        for (int i = 0; i < 12; i++)
        {
            icoVertices[i] = radius * icoVertices[i].normalized;
        }

        // add triangles
        icoTris.AddRange(new List<int> { 2, 1, 0 });
        icoTris.AddRange(new List<int> { 1, 2, 3 });
        icoTris.AddRange(new List<int> { 5, 4, 3 });
        icoTris.AddRange(new List<int> { 4, 8, 3 });
        icoTris.AddRange(new List<int> { 7, 6, 0 });
        icoTris.AddRange(new List<int> { 6, 9, 0 });
        icoTris.AddRange(new List<int> { 11, 10, 4 });
        icoTris.AddRange(new List<int> { 10, 11, 6 });
        icoTris.AddRange(new List<int> { 9, 5, 2 });
        icoTris.AddRange(new List<int> { 5, 9, 11 });
        icoTris.AddRange(new List<int> { 8, 7, 1 });
        icoTris.AddRange(new List<int> { 7, 8, 10 });
        icoTris.AddRange(new List<int> { 2, 5, 3 });
        icoTris.AddRange(new List<int> { 8, 1, 3 });
        icoTris.AddRange(new List<int> { 9, 2, 0 });
        icoTris.AddRange(new List<int> { 1, 7, 0 });
        icoTris.AddRange(new List<int> { 11, 9, 6 });
        icoTris.AddRange(new List<int> { 7, 10, 6 });
        icoTris.AddRange(new List<int> { 5, 11, 4 });
        icoTris.AddRange(new List<int> { 10, 8, 4 });

        for (int it = 0; it < resolution; it++)
        {
            List<Vector3> newIcoVertices = new List<Vector3>();
            List<int> newIcoTris = new List<int>();
            int offsetIco = 0;

            for (int t = 0; t < icoTris.Count / 3; t++)
            {
                Vector3 icoA = icoVertices[icoTris[3 * t + 0]];
                Vector3 icoB = icoVertices[icoTris[3 * t + 1]];
                Vector3 icoC = icoVertices[icoTris[3 * t + 2]];

                newIcoVertices.Add(icoA);
                newIcoVertices.Add(icoB);
                newIcoVertices.Add(icoC);
                newIcoVertices.Add(0.5f * (icoA + icoB));
                newIcoVertices.Add(0.5f * (icoB + icoC));
                newIcoVertices.Add(0.5f * (icoC + icoA));

                newIcoTris.AddRange(new List<int> { offsetIco + 0, offsetIco + 3, offsetIco + 5 });
                newIcoTris.AddRange(new List<int> { offsetIco + 1, offsetIco + 4, offsetIco + 3 });
                newIcoTris.AddRange(new List<int> { offsetIco + 2, offsetIco + 5, offsetIco + 4 });
                newIcoTris.AddRange(new List<int> { offsetIco + 3, offsetIco + 4, offsetIco + 5 });

                offsetIco += 6;
            }

            for (int i = 0; i < newIcoVertices.Count; i++)
            {
                newIcoVertices[i] = radius * newIcoVertices[i].normalized;
            }

            icoVertices = newIcoVertices;
            icoTris = newIcoTris;
        }

        TriMesh icoMesh = new TriMesh();

        int off = 0;
        for (int i = 0; i < icoTris.Count / 3; i++)
        {
            TriMesh triMesh = TriangleGenerator.GetTriMesh(off, icoVertices[icoTris[3 * i + 0]], icoVertices[icoTris[3 * i + 1]], icoVertices[icoTris[3 * i + 2]], true, true, true, gapLength);
            icoMesh.Combine(triMesh);
            off += 3;
        }

        MeshFilter meshFilter = newSphere.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();

        mesh.vertices = icoMesh.vertices;
        mesh.triangles = icoMesh.triangles;
        mesh.normals = icoMesh.normals;
        mesh.uv = icoMesh.uvs;
        mesh.uv2 = icoMesh.uvs2;

        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = newSphere.AddComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { mat };

        return newSphere.transform;
    }

}
