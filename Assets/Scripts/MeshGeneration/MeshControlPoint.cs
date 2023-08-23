using System.Collections;
using System.Collections.Generic;
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
}
