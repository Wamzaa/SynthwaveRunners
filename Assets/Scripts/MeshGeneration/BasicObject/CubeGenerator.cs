using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    public static Transform SetupCubeMesh(string name, Transform parent, Vector3 position, Vector3 size, float gapLength, Material mat)
    {
        GameObject newCube = new GameObject(name);
        newCube.transform.parent = parent;
        newCube.transform.position = position;

        Vector3 x = (size.x / 2) * newCube.transform.right;
        Vector3 y = (size.y / 2) * newCube.transform.up;
        Vector3 z = (size.z / 2) * newCube.transform.forward;

        Vector3[] vertices = new Vector3[]
        {
            - x - y - z,
            - x - y + z,
            x - y + z,
            x - y - z,

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

        float xMin, xMax;
        float yMin, yMax;
        float zMin, zMax;
        float tempE;

        tempE = Mathf.Min(2 * gapLength / size.x, 1.0f);
        if (tempE == 1.0f)
        {
            xMin = 1.0f;
            xMax = 1.0f;
        }
        else
        {
            xMin = -0.5f / (1 - tempE);
            xMax = 0.5f / (1 - tempE);
        }

        tempE = Mathf.Min(2 * gapLength / size.y, 1.0f);
        if (tempE == 1.0f)
        {
            yMin = 1.0f;
            yMax = 1.0f;
        }
        else
        {
            yMin = -0.5f / (1 - tempE);
            yMax = 0.5f / (1 - tempE);
        }

        tempE = Mathf.Min(2 * gapLength / size.z, 1.0f);
        if (tempE == 1.0f)
        {
            zMin = 1.0f;
            zMax = 1.0f;
        }
        else
        {
            zMin = -0.5f / (1 - tempE);
            zMax = 0.5f / (1 - tempE);
        }

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(xMin, zMin),
            new Vector2(xMin, zMax),
            new Vector2(xMax, zMax),
            new Vector2(xMax, zMin),

            new Vector2(xMin, yMin),
            new Vector2(xMax, yMin),
            new Vector2(xMax, yMax),
            new Vector2(xMin, yMax),

            new Vector2(yMin, zMin),
            new Vector2(yMin, zMax),
            new Vector2(yMax, zMax),
            new Vector2(yMax, zMin),

            new Vector2(xMax, yMin),
            new Vector2(xMin, yMin),
            new Vector2(xMin, yMax),
            new Vector2(xMax, yMax),

            new Vector2(yMin, zMax),
            new Vector2(yMin, zMin),
            new Vector2(yMax, zMin),
            new Vector2(yMax, zMax),

            new Vector2(xMin, zMin),
            new Vector2(xMax, zMin),
            new Vector2(xMax, zMax),
            new Vector2(xMin, zMax)
        };

        MeshFilter meshFilter = newCube.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = newCube.AddComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { mat };

        return newCube.transform;
    }

}
