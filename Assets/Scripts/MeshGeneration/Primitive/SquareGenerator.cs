using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGenerator : MonoBehaviour
{
    public static TriMesh GetTriMesh(int indexOffset, Vector3 _p1, Vector3 _p2, Vector3 _p3, Vector3 _p4, bool hasHoriLines, bool hasVertLines, float gapLength)
    {
        TriMesh triMesh = new TriMesh();

        if((_p4 - _p3 + _p2 - _p1).magnitude > 0.1f)
        {
            Debug.Log("Warning : this is not a square (" + (_p4 - _p3 + _p2 - _p1).magnitude + ")");
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
        xMin = (tempE == 1.0f) ? 1.0f : -0.5f / (1 - tempE);
        xMax = (tempE == 1.0f) ? 1.0f : 0.5f / (1 - tempE);

        tempE = Mathf.Min(2 * gapLength / (_p2 - _p1).magnitude, 1.0f);
        yMin = (tempE == 1.0f) ? 1.0f : -0.5f / (1 - tempE);
        yMax = (tempE == 1.0f) ? 1.0f : 0.5f / (1 - tempE);

        if (!hasHoriLines)
        {
            yMin = 0.0f;
            yMax = 0.0f;
        }
        if (!hasVertLines)
        {
            xMin = 0.0f;
            xMax = 0.0f;
        }

        triMesh.uvs = new Vector2[]
        {
            new Vector2(xMin, yMin),
            new Vector2(xMin, yMax),
            new Vector2(xMax, yMax),
            new Vector2(xMax, yMin)
        };

        triMesh.uvs2 = new Vector2[]
        {
            Vector2.zero,
            Vector2.zero,
            Vector2.zero,
            Vector2.zero
        };

        triMesh.normals = new Vector3[]
        {
            n,n,n,n
        };

        return triMesh;
    }
}
