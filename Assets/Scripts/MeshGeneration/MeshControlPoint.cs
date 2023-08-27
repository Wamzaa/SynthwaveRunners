using System.Collections;
using System.Collections.Generic;
using System.Drawing.Design;
using UnityEngine;

public class MeshControlPoint : MonoBehaviour
{
    public enum InterpolationMode { None, Linear, Smooth}

    [Header("--- Interpolated Variables ---")]
    public float height;
    public float width;
    public float borderHeight;

    [Header("--- Interpolation Parameters ---")]
    public InterpolationMode interpolationMode;
    public float hermitVelocity = 1.0f;

    private Vector3 roadNormals;
    private Vector3 roadDir;

    public Vector3 GetRoadNormal()
    {
        return roadNormals;
    }

    public Vector3 GetRoadDir()
    {
        return roadDir;
    }

    public void SetRoadNormal(Vector3 value)
    {
        roadNormals = value;
    }

    public void SetRoadDir(Vector3 value)
    {
        roadDir = value;
    }
}
