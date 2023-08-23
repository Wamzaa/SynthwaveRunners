using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshControlPoint : MonoBehaviour
{
    public enum InterpolationMode { None, Linear, Smooth}

    public float height;
    public float width;
    public float borderHeight;
    public InterpolationMode interpolationMode;
}
