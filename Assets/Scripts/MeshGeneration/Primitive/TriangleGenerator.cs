using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriangleGenerator : MonoBehaviour
{
    public static TriMesh GetTriMesh(int indexOffset, Vector3 _p1, Vector3 _p2, Vector3 _p3, bool edge1, bool edge2, bool edge3, float gapLength)
    {
        TriMesh triMesh = new TriMesh();

        triMesh.vertices = new Vector3[]
        {
            _p1,_p2,_p3
        };

        triMesh.triangles = new int[]
        {
            indexOffset + 0, indexOffset + 1, indexOffset + 2
        };

        Vector3 n = Vector3.Cross(_p2 - _p1, _p3 - _p1);
        n = n.normalized;
        float h1 = Mathf.Abs(Vector3.Dot(Vector3.Cross(Vector3.Normalize(_p3 - _p2), n), (_p1 - _p3)));
        float h2 = Mathf.Abs(Vector3.Dot(Vector3.Cross(Vector3.Normalize(_p1 - _p3), n), (_p2 - _p1)));
        float h3 = Mathf.Abs(Vector3.Dot(Vector3.Cross(Vector3.Normalize(_p2 - _p1), n), (_p3 - _p2)));

        float a1 = edge1 ? (h1 > gapLength ? gapLength / (h1 - gapLength) : 1.0f) : -1.0f;
        float a2 = edge2 ? (h2 > gapLength ? gapLength / (h2 - gapLength) : 1.0f) : -1.0f;
        float a3 = edge3 ? (h3 > gapLength ? gapLength / (h3 - gapLength) : 1.0f) : -1.0f;

        float b1 = edge1 ? (h1 > gapLength ? 1.0f : -1.0f) : 1.0f;
        float b2 = edge2 ? (h2 > gapLength ? 1.0f : -1.0f) : 1.0f;
        float b3 = edge3 ? (h3 > gapLength ? 1.0f : -1.0f) : 1.0f;

        triMesh.uvs = new Vector2[]
        {
            new Vector2(b1, -a2),
            new Vector2(-a1, b2),
            new Vector2(-a1, -a2)
        };

        triMesh.uvs2 = new Vector2[]
        {
            new Vector2(-a3, 0.0f),
            new Vector2(-a3, 0.0f),
            new Vector2(b3, 0.0f)
        };

        triMesh.normals = new Vector3[]
        {
            n,n,n
        };

        return triMesh;
    }
}

public class TriMesh
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Vector2[] uvs;
    public Vector2[] uvs2;

    public TriMesh()
    {
        vertices = new Vector3[] { };
        triangles = new int[] { };
        normals = new Vector3[] { };
        uvs = new Vector2[] { };
        uvs2 = new Vector2[] { };
    }

    // TODO -> 
    // vérifier si tout transformer en liste et uiliser l1.AddRange(l2) est mieux ou pas 
    public void Combine(TriMesh m)
    {
        vertices = vertices.Concat(m.vertices).ToArray();
        triangles = triangles.Concat(m.triangles).ToArray();
        normals = normals.Concat(m.normals).ToArray();
        uvs = uvs.Concat(m.uvs).ToArray();
        uvs2 = uvs2.Concat(m.uvs2).ToArray();
    }
}
