using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuildingGenerator : MonoBehaviour
{
    public float height;
    public float width;
    public float depth;

    public float windowGap;
    public float windowSize;

    public Material mat;

    void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        BuildSimpleBlocBuilding();
    }

    public void BuildSimpleBlocBuilding()
    {
        GameObject baseCube = new GameObject("base(Building)");
        baseCube.transform.parent = this.transform;
        baseCube.transform.position = this.transform.position + height * 0.5f * this.transform.up;
        CubeGenerator baseCubeGenerator = baseCube.AddComponent<CubeGenerator>();
        baseCubeGenerator.mat = mat;
        baseCubeGenerator.gapLength = 5.0f;
        Vector3 baseCubeScale = new Vector3(width, height, depth);
        baseCubeGenerator.size = baseCubeScale;
        baseCubeGenerator.Init();

        int nbWindows = (int) Mathf.Round(height / windowGap) - 1;
        for(int i=0; i<nbWindows; i++)
        {
            GameObject windowCube = new GameObject("window(Building)-" + i);
            windowCube.transform.parent = this.transform;
            windowCube.transform.position = this.transform.position + (i+1) * (height/(nbWindows+1)) * this.transform.up;
            CubeGenerator windowCubeGenerator = windowCube.AddComponent<CubeGenerator>();
            windowCubeGenerator.mat = mat;
            windowCubeGenerator.gapLength = 20.0f;
            Vector3 windowCubeScale = new Vector3(width * 1.05f, 10.0f, depth * 0.75f);
            windowCubeGenerator.size = windowCubeScale;
            windowCubeGenerator.Init();
        }
    }
}
