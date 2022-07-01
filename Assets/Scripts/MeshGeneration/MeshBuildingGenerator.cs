using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuildingGenerator : MonoBehaviour
{
    public float height;
    public float width;
    public float depth;

    public float gapLength;

    public BuildingType type;
    public enum BuildingType { SimpleBloc,  AlterTower};

    public float windowGap;
    public float windowSize;
    public bool bothSide;
    public WindowType windowType;
    public enum WindowType { None, Line, Square, OpenLine, OpenSquare};

    public int nbAlter;
    public FunctionShape alterFunction;
    public enum FunctionShape { Linear, ZigZag, SquareRoot, Square};

    public Material mat;

    void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        if(type == BuildingType.SimpleBloc)
        {
            BuildSimpleBlocBuilding();
        }
        else if(type == BuildingType.AlterTower)
        {
            BuildAlterTowerBuilding();
        }
    }

    public void BuildSimpleBlocBuilding()
    {
        GameObject baseCube = new GameObject("base(Building)");
        baseCube.transform.parent = this.transform;
        baseCube.transform.position = this.transform.position + height * 0.5f * this.transform.up;
        CubeGenerator baseCubeGenerator = baseCube.AddComponent<CubeGenerator>();
        baseCubeGenerator.mat = mat;
        baseCubeGenerator.gapLength = gapLength;
        Vector3 baseCubeScale = new Vector3(width, height, depth);
        baseCubeGenerator.size = baseCubeScale;
        baseCubeGenerator.Init();

        if(windowType == WindowType.Line || windowType == WindowType.OpenLine)
        {
            float windowGapLength = (windowType == WindowType.Line) ? windowSize : gapLength;
            int nbWindows = (int)Mathf.Round(height / windowGap) - 1;
            for (int i = 1; i <= nbWindows; i++)
            {
                GameObject windowCube = new GameObject("window(Building)-" + i);
                windowCube.transform.parent = this.transform;
                windowCube.transform.position = this.transform.position + i * (height / (nbWindows + 1)) * this.transform.up;
                CubeGenerator windowCubeGenerator = windowCube.AddComponent<CubeGenerator>();
                windowCubeGenerator.mat = mat;
                windowCubeGenerator.gapLength = windowGapLength;
                Vector3 windowCubeScale = new Vector3(width + gapLength, windowSize, depth * 0.75f);
                windowCubeGenerator.size = windowCubeScale;
                windowCubeGenerator.Init();
            }

            if (bothSide)
            {
                for (int i = 1; i <= nbWindows; i++)
                {
                    GameObject windowCube = new GameObject("window(Building)-" + (i+nbWindows));
                    windowCube.transform.parent = this.transform;
                    windowCube.transform.position = this.transform.position + i * (height / (nbWindows + 1)) * this.transform.up;
                    CubeGenerator windowCubeGenerator = windowCube.AddComponent<CubeGenerator>();
                    windowCubeGenerator.mat = mat;
                    windowCubeGenerator.gapLength = windowGapLength;
                    Vector3 windowCubeScale = new Vector3(width * 0.75f, windowSize, depth + gapLength);
                    windowCubeGenerator.size = windowCubeScale;
                    windowCubeGenerator.Init();
                }
            }
        }
        else if(windowType == WindowType.Square || windowType == WindowType.OpenSquare)
        {
            float windowGapLength = (windowType == WindowType.Square) ? windowSize : gapLength;
            int nbWindowsHeight = (int)Mathf.Round(height / windowGap) - 1;
            int nbWindowsDepth = (int)Mathf.Round(depth / windowGap) - 1;
            for (int i = 1; i <= nbWindowsHeight; i++)
            {
                for(int j = 1; j <= nbWindowsDepth; j++)
                {
                    GameObject windowCube = new GameObject("window(Building)-" + i + "/" + j);
                    windowCube.transform.parent = this.transform;
                    windowCube.transform.position = this.transform.position + i * (height / (nbWindowsHeight + 1)) * this.transform.up + (j * (depth / (nbWindowsDepth + 1)) - depth/2) * this.transform.forward;
                    CubeGenerator windowCubeGenerator = windowCube.AddComponent<CubeGenerator>();
                    windowCubeGenerator.mat = mat;
                    windowCubeGenerator.gapLength = windowGapLength;
                    Vector3 windowCubeScale = new Vector3(width + gapLength, windowSize, windowSize);
                    windowCubeGenerator.size = windowCubeScale;
                    windowCubeGenerator.Init();
                }
            }

            if (bothSide)
            {
                int nbWindowsWidth = (int)Mathf.Round(width / windowGap) - 1;
                for (int i = 1; i <= nbWindowsHeight; i++)
                {
                    for (int j = 1; j <= nbWindowsWidth; j++)
                    {
                        GameObject windowCube = new GameObject("window(Building)-" + i + "/" + j);
                        windowCube.transform.parent = this.transform;
                        windowCube.transform.position = this.transform.position + i * (height / (nbWindowsHeight + 1)) * this.transform.up + (j * (width / (nbWindowsWidth + 1)) - width / 2) * this.transform.right;
                        CubeGenerator windowCubeGenerator = windowCube.AddComponent<CubeGenerator>();
                        windowCubeGenerator.mat = mat;
                        windowCubeGenerator.gapLength = windowGapLength;
                        Vector3 windowCubeScale = new Vector3(windowSize, windowSize, depth + gapLength);
                        windowCubeGenerator.size = windowCubeScale;
                        windowCubeGenerator.Init();
                    }
                }
            }
        }
        else if(windowType == WindowType.None)
        {
            //Nothing
        }
        
    }


    public void BuildAlterTowerBuilding()
    {
        GameObject baseCube = new GameObject("base(Building)");
        baseCube.transform.parent = this.transform;
        baseCube.transform.position = this.transform.position + height * 0.5f * this.transform.up;
        CubeGenerator baseCubeGenerator = baseCube.AddComponent<CubeGenerator>();
        baseCubeGenerator.mat = mat;
        baseCubeGenerator.gapLength = gapLength;
        Vector3 baseCubeScale = new Vector3(width * 0.25f , height, depth * 0.25f);
        baseCubeGenerator.size = baseCubeScale;
        baseCubeGenerator.Init();

        float alterLen = 0.5f * height / nbAlter;
        for (int i = 0; i < nbAlter; i++)
        {
            GameObject windowCube = new GameObject("alter(Building)-" + i);
            windowCube.transform.parent = this.transform;
            windowCube.transform.position = this.transform.position + ((2 * i + 1) * alterLen - 0.5f * alterLen) * this.transform.up;
            CubeGenerator windowCubeGenerator = windowCube.AddComponent<CubeGenerator>();
            windowCubeGenerator.mat = mat;
            windowCubeGenerator.gapLength = gapLength;

            float ratioPos = ((2.0f * i + 1.0f) - 0.5f) / (nbAlter * 2.0f);
            Vector3 windowCubeScale = Vector3.zero;

            switch (alterFunction)
            {
                case FunctionShape.Linear:
                    windowCubeScale = new Vector3(((1.0f - ratioPos) * 0.75f + 0.25f)* width, alterLen, ((1.0f - ratioPos) * 0.75f + 0.25f) * depth);
                    break;
                case FunctionShape.ZigZag:
                    if (i % 2 == 0)
                    {
                        windowCubeScale = new Vector3(width, alterLen, depth);
                    }
                    else
                    {
                        windowCubeScale = new Vector3(0.4f * width, alterLen, 0.4f * depth);
                    }
                    break;
                case FunctionShape.SquareRoot:
                    windowCubeScale = new Vector3((Mathf.Sqrt(1.0f - ratioPos) * 0.75f + 0.25f) * width, alterLen, (Mathf.Sqrt(1.0f - ratioPos) * 0.75f + 0.25f) * depth);
                    break;
                case FunctionShape.Square:
                    windowCubeScale = new Vector3(((1.0f - ratioPos) * (1.0f - ratioPos) * 0.75f + 0.25f) * width, alterLen, ((1.0f - ratioPos) * (1.0f - ratioPos) * 0.75f + 0.25f) * depth);
                    break;
            }

            windowCubeGenerator.size = windowCubeScale;
            windowCubeGenerator.Init();
        }
    }
}
