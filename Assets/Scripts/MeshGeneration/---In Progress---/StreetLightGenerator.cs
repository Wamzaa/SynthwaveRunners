using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLightGenerator : MonoBehaviour
{
    public float scale;
    public float depth;
    public Material mat;

    private void Start()
    {
        GenerateRoadLight();
    }

    private void GenerateRoadLight()
    {
        Quaternion rot = this.transform.rotation;
        this.transform.rotation = Quaternion.identity;


        Texture tex = mat.GetTexture("_EmissionMap");
        float invMaxDim = 1 / ((float)Mathf.Max(tex.width, tex.height)); 

        Vector3 x = (scale * invMaxDim * tex.width / 2) * Vector3.right;
        Vector3 y = (scale * invMaxDim * tex.height / 2) * Vector3.up;
        Vector3 z = (depth / 2) * Vector3.forward;

        Vector3[] vertices = new Vector3[]
        {
            x - y - z,
            - x - y - z,
            - x - y + z,
            x - y + z,

            - x - y - z,
            x - y - z,
            x + y - z,
            - x + y - z,

            x - y - z,
            x - y + z,
            x + y + z,
            x + y - z,

            x - y + z,
            - x - y + z,
            - x + y + z,
            x + y + z,

            - x - y + z,
            - x - y - z,
            - x + y - z,
            - x + y + z,

            - x + y - z,
            x + y - z,
            x + y + z,
            - x + y + z
        };

        int[] triangles = new int[]
        {
            0,3,2,
            0,2,1,
            4,7,6,
            4,6,5,
            8,11,10,
            8,10,9,
            12,15,14,
            12,14,13,
            16,19,18,
            16,18,17,
            20,23,22,
            20,22,21
        };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(1.0f, 0.0f),
            new Vector2(0.0f, 0.0f),
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),

            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),

            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(1.0f, 1.0f),

            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),

            new Vector2(0.0f, 0.0f),
            new Vector2(0.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(0.0f, 1.0f),

            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),
        };

        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { mat };

        this.transform.rotation = rot;
    }

    public void OnDrawGizmos()
    {
        Texture tex = mat.GetTexture("_EmissionMap");
        float invMaxDim = 1 / ((float)Mathf.Max(tex.width, tex.height));

        float x = scale * invMaxDim * tex.width;
        float y = scale * invMaxDim * tex.height;

        Gizmos.color = Color.magenta;
        DrawRotatedWireCube(x, y, depth);
    }

    public void DrawRotatedWireCube(float width, float height, float depth)
    {
        Vector3 pos = this.transform.position;
        Vector3 x = (width / 2) * this.transform.right;
        Vector3 y = height * this.transform.up;
        Vector3 z = (depth / 2) * this.transform.forward;

        Gizmos.DrawLine(pos - x - z, pos + x - z);
        Gizmos.DrawLine(pos + x - z, pos + x + z);
        Gizmos.DrawLine(pos + x + z, pos - x + z);
        Gizmos.DrawLine(pos - x + z, pos - x - z);

        Gizmos.DrawLine(pos - x - z, pos - x - z + y);
        Gizmos.DrawLine(pos + x - z, pos + x - z + y);
        Gizmos.DrawLine(pos + x + z, pos + x + z + y);
        Gizmos.DrawLine(pos - x + z, pos - x + z + y);

        Gizmos.DrawLine(pos - x - z + y, pos + x - z + y);
        Gizmos.DrawLine(pos + x - z + y, pos + x + z + y);
        Gizmos.DrawLine(pos + x + z + y, pos - x + z + y);
        Gizmos.DrawLine(pos - x + z + y, pos - x - z + y);
    }
}
