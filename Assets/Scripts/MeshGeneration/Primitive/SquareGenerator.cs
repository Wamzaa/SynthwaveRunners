using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGenerator : MonoBehaviour
{
    public static TriMesh GetTriMesh(int indexOffset, Vector3 _p1, Vector3 _p2, Vector3 _p3, Vector3 _p4, float gapLength)
    {
        TriMesh triMesh = new TriMesh();

        if((_p4 - _p3 - _p2 + _p1).magnitude > 0.001f)
        {
            Debug.Log("Warning : this is not a square");
        }

        triMesh.vertices = new Vector3[]
        {
            _p1,_p2,_p3,_p4
        };

        triMesh.triangles = new int[]
        {
            indexOffset + 0, indexOffset + 1, indexOffset + 2,
            indexOffset + 0, indexOffset + 2, indexOffset + 3
        };

        Vector3 n = Vector3.Cross(_p2 - _p1, _p3 - _p1);
        n = n.normalized;

        float xMin, xMax, yMin, yMax;
        float tempE;

        tempE = Mathf.Min(2 * gapLength / (_p3 - _p2).magnitude, 1.0f);
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

        tempE = Mathf.Min(2 * gapLength / (_p2 - _p1).magnitude, 1.0f);
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

        triMesh.uvs = new Vector2[]
        {
            new Vector2(xMin, yMin),
            new Vector2(xMin, yMax),
            new Vector2(xMax, yMax),
            new Vector2(xMax, yMin)
        };

        triMesh.uvs2 = new Vector2[]{};

        triMesh.normals = new Vector3[]
        {
            n,n,n,n
        };

        return triMesh;
    }
}
