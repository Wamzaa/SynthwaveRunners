using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE : Classe relue -> OK
//On peut enlever ce qui concerne la gestion des objets de la classe. cette classe semble plus performante si l'on utilise que les fonctions statiques de la classe
//Ce qui précède pourra être vérifier plus tard (calcul qu'à l'initialisation, plus performant de n'avoir qu'un mesh

public class TriangleGenerator : MonoBehaviour
{
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;

    public bool hasEdge1;
    public bool hasEdge2;
    public bool hasEdge3;

    public float gapLength;

    public Material mat;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector3[] normals;
    private Vector2[] uvs;
    private Vector2[] uvs2;


    private void Start()
    {
        //Init();
    }

    public void Init()
    {
        GenerateTrianglePrimitive();
    }

    private void GenerateTrianglePrimitive()
    {

        vertices = new Vector3[]
        {
            p1,p2,p3
        };

        triangles = new int[]
        {
            0,1,2
        };

        Vector3 n = Vector3.Cross(p2 - p1, p3 - p1);
        n = n.normalized;
        float h1 = Mathf.Abs(Vector3.Dot(Vector3.Cross(Vector3.Normalize(p3 - p2), n), (p1 - p3)));
        float h2 = Mathf.Abs(Vector3.Dot(Vector3.Cross(Vector3.Normalize(p1 - p3), n), (p2 - p1)));
        float h3 = Mathf.Abs(Vector3.Dot(Vector3.Cross(Vector3.Normalize(p2 - p1), n), (p3 - p2)));

        float a1 = hasEdge1 ? (h1 > gapLength ? gapLength / (h1 - gapLength) : 1.0f) : -1.0f;
        float a2 = hasEdge2 ? (h2 > gapLength ? gapLength / (h2 - gapLength) : 1.0f) : -1.0f;
        float a3 = hasEdge3 ? (h3 > gapLength ? gapLength / (h3 - gapLength) : 1.0f) : -1.0f;

        float b1 = hasEdge1 ? (h1 > gapLength ? 1.0f : -1.0f) : 1.0f;
        float b2 = hasEdge2 ? (h2 > gapLength ? 1.0f : -1.0f) : 1.0f;
        float b3 = hasEdge3 ? (h3 > gapLength ? 1.0f : -1.0f) : 1.0f;

        uvs = new Vector2[]
        {
            new Vector2(b1, -a2),
            new Vector2(-a1, b2),
            new Vector2(-a1, -a2)
        };

        uvs2 = new Vector2[]
        {
            new Vector2(-a3, 0.0f),
            new Vector2(-a3, 0.0f),
            new Vector2(b3, 0.0f)
        };


        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.uv2 = uvs2;

        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { mat };
    }

    public static void SetupTriangleMesh(string name, Transform parent, Vector3 _p1, Vector3 _p2, Vector3 _p3, bool edge1, bool edge2, bool edge3, float gapLength, Material mat)
    {
        GameObject newTri = new GameObject(name);
        newTri.transform.parent = parent;
        newTri.transform.position = parent.transform.position;
        TriangleGenerator newTriangleGenerator = newTri.AddComponent<TriangleGenerator>();
        newTriangleGenerator.mat = mat;
        newTriangleGenerator.gapLength = gapLength;
        newTriangleGenerator.p1 = _p1;
        newTriangleGenerator.p2 = _p2;
        newTriangleGenerator.p3 = _p3;
        newTriangleGenerator.hasEdge1 = edge1;
        newTriangleGenerator.hasEdge2 = edge2;
        newTriangleGenerator.hasEdge3 = edge3;
        newTriangleGenerator.Init();
    }

    public class TriMesh
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector3[] normals;
        public Vector2[] uvs;
        public Vector2[] uvs2;
    }

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
