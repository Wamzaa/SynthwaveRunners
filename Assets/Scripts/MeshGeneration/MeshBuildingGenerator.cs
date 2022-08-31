using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuildingGenerator : MonoBehaviour
{
    public float height;
    public float width;
    public float depth;
    public float radius;

    public int resolution;
    public bool hasCylinderLines;

    public float gapLength;

    public BuildingType type;
    public enum BuildingType { SimpleBloc, CircularBloc, AlterTower, CircularAlterTower };

    public float windowGap;
    public float windowSize;
    public bool bothSide;
    public WindowType windowType;
    public enum WindowType { None, Line, Square, OpenLine, OpenSquare};

    public int nbAlter;
    public FunctionShape alterFunction;
    public enum FunctionShape { Linear, ZigZag, SquareRoot, Square};

    public Material mat;
    public Material mat2;

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
        else if (type == BuildingType.CircularBloc)
        {
            BuildCircularBlocBuilding();
        }
        else if(type == BuildingType.AlterTower)
        {
            BuildAlterTowerBuilding();
        }
        else if (type == BuildingType.CircularAlterTower)
        {
            BuildCircularAlterTowerBuilding();
        }
    }

    public void BuildSimpleBlocBuilding()
    {
        Vector3 baseCubeScale = new Vector3(width, height, depth);
        CubeGenerator.SetupCubeMesh("base(Building)", this.transform, this.transform.position + height * 0.5f * this.transform.up, baseCubeScale, gapLength, mat);
        
        if(windowType == WindowType.Line || windowType == WindowType.OpenLine)
        {
            float windowGapLength = (windowType == WindowType.Line) ? windowSize : gapLength;
            int nbWindows = (int)Mathf.Round(height / windowGap) - 1;
            for (int i = 1; i <= nbWindows; i++)
            {
                Vector3 windowCubeScale = new Vector3(width + gapLength, windowSize, depth * 0.75f);
                CubeGenerator.SetupCubeMesh("window(Building)-" + i, this.transform, this.transform.position + i * (height / (nbWindows + 1)) * this.transform.up, windowCubeScale, windowGapLength, mat);
                
            }

            if (bothSide)
            {
                for (int i = 1; i <= nbWindows; i++)
                {
                    Vector3 windowCubeScale = new Vector3(width * 0.75f, windowSize, depth + gapLength);
                    CubeGenerator.SetupCubeMesh("window(Building)-" + (i + nbWindows), this.transform, this.transform.position + i * (height / (nbWindows + 1)) * this.transform.up, windowCubeScale, windowGapLength, mat);
                    
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
                    Vector3 windowCubeScale = new Vector3(width + gapLength, windowSize, windowSize);
                    CubeGenerator.SetupCubeMesh("window(Building)-" + i + "/" + j, this.transform, this.transform.position + i * (height / (nbWindowsHeight + 1)) * this.transform.up + (j * (depth / (nbWindowsDepth + 1)) - depth / 2) * this.transform.forward, windowCubeScale, windowGapLength, mat);

                }
            }

            if (bothSide)
            {
                int nbWindowsWidth = (int)Mathf.Round(width / windowGap) - 1;
                for (int i = 1; i <= nbWindowsHeight; i++)
                {
                    for (int j = 1; j <= nbWindowsWidth; j++)
                    {
                        Vector3 windowCubeScale = new Vector3(windowSize, windowSize, depth + gapLength);
                        CubeGenerator.SetupCubeMesh("window(Building)-" + i + "/" + j, this.transform, this.transform.position + i * (height / (nbWindowsHeight + 1)) * this.transform.up + (j * (width / (nbWindowsWidth + 1)) - width / 2) * this.transform.right, windowCubeScale, windowGapLength, mat);

                    }
                }
            }
        }
        else if(windowType == WindowType.None)
        {
            //Nothing
        }
        
    }

    public void BuildCircularBlocBuilding()
    {
        CylinderGenerator.SetupCylinderMesh("base(Building)", this.transform, this.transform.position + height * 0.5f * this.transform.up, radius, height, resolution, hasCylinderLines, gapLength, mat, mat2);

        if (windowType == WindowType.Line || windowType == WindowType.OpenLine)
        {
            float windowGapLength = (windowType == WindowType.Line) ? windowSize : gapLength;
            int nbWindows = (int)Mathf.Round(height / windowGap) - 1;
            for (int i = 1; i <= nbWindows; i++)
            {
                CylinderGenerator.SetupCylinderMesh("window(Building)-" + i, this.transform, this.transform.position + i * (height / (nbWindows + 1)) * this.transform.up, radius + gapLength, windowSize, resolution, hasCylinderLines, windowGapLength, mat, mat2);
            }
        }
        /*else if (windowType == WindowType.Square || windowType == WindowType.OpenSquare)
        {
            float windowGapLength = (windowType == WindowType.Square) ? windowSize : gapLength;
            int nbWindowsHeight = (int)Mathf.Round(height / windowGap) - 1;
            int nbWindowsDepth = (int)Mathf.Round(depth / windowGap) - 1;
            for (int i = 1; i <= nbWindowsHeight; i++)
            {
                for (int j = 1; j <= nbWindowsDepth; j++)
                {
                    GameObject windowCube = new GameObject("window(Building)-" + i + "/" + j);
                    windowCube.transform.parent = this.transform;
                    windowCube.transform.position = this.transform.position + i * (height / (nbWindowsHeight + 1)) * this.transform.up + (j * (depth / (nbWindowsDepth + 1)) - depth / 2) * this.transform.forward;
                    CubeGenerator windowCubeGenerator = windowCube.AddComponent<CubeGenerator>();
                    windowCubeGenerator.mat = mat;
                    windowCubeGenerator.gapLength = windowGapLength;
                    Vector3 windowCubeScale = new Vector3(width + gapLength, windowSize, windowSize);
                    windowCubeGenerator.size = windowCubeScale;
                    windowCubeGenerator.Init();
                }
            }
        }*/
        else if (windowType == WindowType.None)
        {
            //Nothing
        }

    }

    public void BuildAlterTowerBuilding()
    {
        Vector3 baseCubeScale = new Vector3(width * 0.25f, height, depth * 0.25f);
        CubeGenerator.SetupCubeMesh("base(Building)", this.transform, this.transform.position + height * 0.5f * this.transform.up, baseCubeScale, gapLength, mat);
        

        float alterLen = 0.5f * height / nbAlter;
        for (int i = 0; i < nbAlter; i++)
        {
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

            CubeGenerator.SetupCubeMesh("alter(Building)-" + i, this.transform, this.transform.position + ((2 * i + 1) * alterLen - 0.5f * alterLen) * this.transform.up, windowCubeScale, gapLength, mat);
        }
    }

    public void BuildCircularAlterTowerBuilding()
    {
        CylinderGenerator.SetupCylinderMesh("base(Building)", this.transform, this.transform.position + height * 0.5f * this.transform.up, 0.25f * radius, height, resolution, hasCylinderLines, gapLength, mat, mat2);

        float alterLen = 0.5f * height / nbAlter;
        for (int i = 0; i < nbAlter; i++)
        {
            float windowRadius = 0.0f;
            float ratioPos = ((2.0f * i + 1.0f) - 0.5f) / (nbAlter * 2.0f);

            switch (alterFunction)
            {
                case FunctionShape.Linear:
                    windowRadius = ((1.0f - ratioPos) * 0.75f + 0.25f) * radius;
                    break;
                case FunctionShape.ZigZag:
                    if (i % 2 == 0)
                    {
                        windowRadius = radius;
                    }
                    else
                    {
                        windowRadius = 0.4f * radius;
                    }
                    break;
                case FunctionShape.SquareRoot:
                    windowRadius = (Mathf.Sqrt(1.0f - ratioPos) * 0.75f + 0.25f) * radius;
                    break;
                case FunctionShape.Square:
                    windowRadius = ((1.0f - ratioPos) * (1.0f - ratioPos) * 0.75f + 0.25f) * radius;
                    break;
            }

            CylinderGenerator.SetupCylinderMesh("alter(Building)-" + i, this.transform, this.transform.position + ((2 * i + 1) * alterLen - 0.5f * alterLen) * this.transform.up, windowRadius, alterLen, resolution, hasCylinderLines, gapLength, mat, mat2);

        }
    }
    }
