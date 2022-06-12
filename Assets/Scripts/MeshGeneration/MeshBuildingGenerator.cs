using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuildingGenerator : MonoBehaviour
{
    public float height;
    public float width;
    public float depth;

    public float windowGap;
    public float windowSize;

    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uvs;
    private List<int> triangles;

    void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        BuildSimpleBlocBuilding();
    }

    public void BuildSimpleBlocBuilding()
    {
        /*

        GameObject quadObj = new GameObject("QuadObj");
        quadObj.transform.parent = meshParent.transform;
        MeshFilter meshFilter = quadObj.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = quadObj.AddComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { mat };

        MeshCollider collider = quadObj.AddComponent<MeshCollider>();
        collider.convex = true;

        quadObj.layer = LayerMask.NameToLayer("Floor");*/
    }
}
