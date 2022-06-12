using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    public List<List<float>> heightMap;
    public int resolution;

    private void Start()
    {
        List<float> test = new List<float>(){ 0.0f, 1.1f };
    }
}
