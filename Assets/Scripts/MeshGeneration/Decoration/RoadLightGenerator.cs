using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLightGenerator : MonoBehaviour
{
    public float scale;
    public float gapLength;
    public Material mat;

    private void Start()
    {
        GenerateRoadLight();
    }

    private void GenerateRoadLight()
    {
        GameObject baseCube = new GameObject("base(RoadLight)");
        baseCube.transform.parent = this.transform;
        baseCube.transform.position = this.transform.position + scale * 0.5f * this.transform.up;
        CubeGenerator baseCubeGenerator = baseCube.AddComponent<CubeGenerator>();
        baseCubeGenerator.mat = mat;
        baseCubeGenerator.gapLength = gapLength;
        Vector3 baseCubeScale = new Vector3(0.3f, 1.0f, 0.3f);
        baseCubeGenerator.size = scale * baseCubeScale;
        baseCubeGenerator.Init();

        GameObject armCube = new GameObject("arm(RoadLight)");
        armCube.transform.parent = this.transform;
        armCube.transform.position = this.transform.position + scale * 2.0f * this.transform.up;
        CubeGenerator armCubeGenerator = armCube.AddComponent<CubeGenerator>();
        armCubeGenerator.mat = mat;
        armCubeGenerator.gapLength = gapLength;
        Vector3 armCubeScale = new Vector3(0.15f, 2.2f, 0.15f);
        armCubeGenerator.size = scale * armCubeScale;
        armCubeGenerator.Init();

        GameObject headCube = new GameObject("head(RoadLight)");
        headCube.transform.parent = this.transform;
        headCube.transform.position = this.transform.position + scale * 3.2f * this.transform.up + scale * 0.25f * this.transform.forward;
        CubeGenerator headCubeGenerator = headCube.AddComponent<CubeGenerator>();
        headCubeGenerator.mat = mat;
        headCubeGenerator.gapLength = gapLength;
        Vector3 headCubeScale = new Vector3(0.35f, 0.2f, 0.8f);
        headCubeGenerator.size = scale * headCubeScale;
        headCubeGenerator.Init();

        GameObject lightCube = new GameObject("light(RoadLight)");
        lightCube.transform.parent = this.transform;
        lightCube.transform.position = this.transform.position + scale * 3.1f * this.transform.up + scale * 0.35f * this.transform.forward;
        CubeGenerator lightCubeGenerator = lightCube.AddComponent<CubeGenerator>();
        lightCubeGenerator.mat = mat;
        lightCubeGenerator.gapLength = scale * 0.45f;
        Vector3 lightCubeScale = new Vector3(0.25f, 0.12f, 0.45f);
        lightCubeGenerator.size = scale * lightCubeScale;
        lightCubeGenerator.Init();
    }
}
