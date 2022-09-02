using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuildingGenerator : MonoBehaviour
{
    [Header("--- Type ---")]
    public BuildingType type;
    public enum BuildingType { SimpleBloc, CircularBloc, AlterTower, CircularAlterTower, BubbleTemple};


    [Header("--- Primary Settings ---")]
    public float height;
    public float width;
    public float depth;
    [Header("--- Primary Settings ---")]
    public float radius;
    public int resolution;
    public bool hasCylinderLines;

    [Header("--- Window Settings ---")]
    public WindowType windowType;
    public enum WindowType { None, Line, Square, OpenLine, OpenSquare };

    public float windowSize;
    public float windowGap;
    public bool bothSide;


    [Header("--- Specific Settings ---")]
    // Alter Tower Settings
    public FunctionShape alterFunction;
    public enum FunctionShape { Linear, ZigZag, SquareRoot, Square, Keel };

    public int nbAlter;

    [Header("--- Specific Settings ---")]
    //Bubble Temple Settings
    public int nbPlane; 


    [Header("--- Material & Mat. Settings ---")]
    public float gapLength;
    public Material triMat;
    public Material squaMat;

    void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        Quaternion rot = this.transform.rotation;
        this.transform.rotation = Quaternion.identity;

        if (type == BuildingType.SimpleBloc)
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
        else if (type == BuildingType.BubbleTemple)
        {
            BuildBubbleTempleBuilding();
        }

        this.transform.rotation = rot;
    }

    public void BuildSimpleBlocBuilding()
    {
        Vector3 baseCubeScale = new Vector3(width, height, depth);
        CubeGenerator.SetupCubeMesh("base(Building)", this.transform, this.transform.position + height * 0.5f * Vector3.up, baseCubeScale, gapLength, squaMat);
        
        if(windowType == WindowType.Line || windowType == WindowType.OpenLine)
        {
            float windowGapLength = (windowType == WindowType.Line) ? windowSize : gapLength;
            int nbWindows = (int)Mathf.Round(height / windowGap) - 1;
            for (int i = 1; i <= nbWindows; i++)
            {
                Vector3 windowCubeScale = new Vector3(width + gapLength, windowSize, depth * 0.75f);
                CubeGenerator.SetupCubeMesh("window(Building)-" + i, this.transform, this.transform.position + i * (height / (nbWindows + 1)) * Vector3.up, windowCubeScale, windowGapLength, squaMat);
                
            }

            if (bothSide)
            {
                for (int i = 1; i <= nbWindows; i++)
                {
                    Vector3 windowCubeScale = new Vector3(width * 0.75f, windowSize, depth + gapLength);
                    CubeGenerator.SetupCubeMesh("window(Building)-" + (i + nbWindows), this.transform, this.transform.position + i * (height / (nbWindows + 1)) * Vector3.up, windowCubeScale, windowGapLength, squaMat);
                    
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
                    CubeGenerator.SetupCubeMesh("window(Building)-" + i + "/" + j, this.transform, this.transform.position + i * (height / (nbWindowsHeight + 1)) * Vector3.up + (j * (depth / (nbWindowsDepth + 1)) - depth / 2) * Vector3.forward, windowCubeScale, windowGapLength, squaMat);

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
                        CubeGenerator.SetupCubeMesh("window(Building)-" + i + "/" + j, this.transform, this.transform.position + i * (height / (nbWindowsHeight + 1)) * Vector3.up + (j * (width / (nbWindowsWidth + 1)) - width / 2) * Vector3.right, windowCubeScale, windowGapLength, squaMat);

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
        CylinderGenerator.SetupCylinderMesh("base(Building)", this.transform, this.transform.position + height * 0.5f * Vector3.up, radius, height, resolution, hasCylinderLines, gapLength, triMat, squaMat);

        if (windowType == WindowType.Line || windowType == WindowType.OpenLine)
        {
            float windowGapLength = (windowType == WindowType.Line) ? windowSize : gapLength;
            int nbWindows = (int)Mathf.Round(height / windowGap) - 1;
            for (int i = 1; i <= nbWindows; i++)
            {
                CylinderGenerator.SetupCylinderMesh("window(Building)-" + i, this.transform, this.transform.position + i * (height / (nbWindows + 1)) * Vector3.up, radius + gapLength, windowSize, resolution, hasCylinderLines, windowGapLength, triMat, squaMat);
            }
        }
        else if (windowType == WindowType.Square || windowType == WindowType.OpenSquare)
        {
            float windowGapLength = (windowType == WindowType.Square) ? windowSize : gapLength;
            int nbWindowsHeight = (int)Mathf.Round(height / windowGap) - 1;
            int nbWindowsSide = (int)Mathf.Round(2 * Mathf.PI * radius / windowGap);
            for (int i = 1; i <= nbWindowsHeight; i++)
            {
                for (int j = 1; j <= nbWindowsSide; j++)
                {
                    Vector3 windowCubeScale = new Vector3(windowSize, windowSize, gapLength);
                    Transform cubeTr = CubeGenerator.SetupCubeMesh("window(Building)-" + i + "/" + j, this.transform, this.transform.position + i * (height / (nbWindowsHeight + 1)) * Vector3.up + (radius * Mathf.Cos(j * 2 * Mathf.PI / nbWindowsSide) * Vector3.right + radius * Mathf.Sin(j * 2 * Mathf.PI / nbWindowsSide) * Vector3.forward), windowCubeScale, windowGapLength, squaMat);
                    cubeTr.LookAt(this.transform.position + i * (height / (nbWindowsHeight + 1)) * Vector3.up);
                }
            }
        }
        else if (windowType == WindowType.None)
        {
            //Nothing
        }

    }

    public void BuildAlterTowerBuilding()
    {
        Vector3 baseCubeScale = new Vector3(width * 0.25f, height, depth * 0.25f);
        CubeGenerator.SetupCubeMesh("base(Building)", this.transform, this.transform.position + height * 0.5f * Vector3.up, baseCubeScale, gapLength, squaMat);
        

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
                        windowCubeScale = new Vector3(0.6f * width, alterLen, 0.6f * depth);
                    }
                    break;
                case FunctionShape.SquareRoot:
                    windowCubeScale = new Vector3((Mathf.Sqrt(1.0f - ratioPos) * 0.75f + 0.25f) * width, alterLen, (Mathf.Sqrt(1.0f - ratioPos) * 0.75f + 0.25f) * depth);
                    break;
                case FunctionShape.Square:
                    windowCubeScale = new Vector3(((1.0f - ratioPos) * (1.0f - ratioPos) * 0.75f + 0.25f) * width, alterLen, ((1.0f - ratioPos) * (1.0f - ratioPos) * 0.75f + 0.25f) * depth);
                    break;
                case FunctionShape.Keel:
                    if(ratioPos < 2.0f / 3.0f)
                    {
                        windowCubeScale = new Vector3(((1.0f - 1.5f * ratioPos) * (1.0f - 1.5f * ratioPos) * 0.75f + 0.25f) * width, alterLen, ((1.0f - 1.5f * ratioPos) * (1.0f - 1.5f * ratioPos) * 0.75f + 0.25f) * depth);
                    }
                    else
                    {
                        windowCubeScale = new Vector3(((3.0f * ratioPos - 2.0f) * (3.0f * ratioPos - 2.0f) * 0.75f + 0.25f) * width, alterLen, ((3.0f * ratioPos - 2.0f) * (3.0f * ratioPos - 2.0f) * 0.75f + 0.25f) * depth);
                    }
                    break;
            }

            CubeGenerator.SetupCubeMesh("alter(Building)-" + i, this.transform, this.transform.position + ((2 * i + 1) * alterLen - 0.5f * alterLen) * Vector3.up, windowCubeScale, gapLength, squaMat);
        }
    }

    public void BuildCircularAlterTowerBuilding()
    {
        CylinderGenerator.SetupCylinderMesh("base(Building)", this.transform, this.transform.position + height * 0.5f * Vector3.up, 0.15f * radius, height, resolution, hasCylinderLines, gapLength, triMat, squaMat);

        float alterLen = 0.5f * height / nbAlter;
        for (int i = 0; i < nbAlter; i++)
        {
            float windowRadius = 0.0f;
            float ratioPos = ((2.0f * i + 1.0f) - 0.5f) / (nbAlter * 2.0f);

            switch (alterFunction)
            {
                case FunctionShape.Linear:
                    windowRadius = ((1.0f - ratioPos) * 0.85f + 0.15f) * radius;
                    break;
                case FunctionShape.ZigZag:
                    if (i % 2 == 0)
                    {
                        windowRadius = radius;
                    }
                    else
                    {
                        windowRadius = 0.6f * radius;
                    }
                    break;
                case FunctionShape.SquareRoot:
                    windowRadius = (Mathf.Sqrt(1.0f - ratioPos) * 0.85f + 0.15f) * radius;
                    break;
                case FunctionShape.Square:
                    windowRadius = ((1.0f - ratioPos) * (1.0f - ratioPos) * 0.85f + 0.15f) * radius;
                    break;
                case FunctionShape.Keel:
                    if (ratioPos < 2.0f / 3.0f)
                    {
                        windowRadius = ((1.0f - 1.5f * ratioPos) * (1.0f - 1.5f * ratioPos) * 0.85f + 0.15f) * radius;
                    }
                    else
                    {
                        windowRadius = ((3.0f * ratioPos - 2.0f) * (3.0f * ratioPos - 2.0f) * 0.85f + 0.15f) * radius;
                    }
                    break;
            }

            CylinderGenerator.SetupCylinderMesh("alter(Building)-" + i, this.transform, this.transform.position + ((2 * i + 1) * alterLen + 0.5f * alterLen) * Vector3.up, windowRadius, alterLen, resolution, hasCylinderLines, gapLength, triMat, squaMat);
            if(alterFunction == FunctionShape.Keel)
            {
                IcosphereGenerator.SetupIcoSphereMesh("head(keelTower)", this.transform, this.transform.position + (height + radius) * Vector3.up, radius, 1, gapLength, triMat);
            }
        }
    }

    public void BuildBubbleTempleBuilding()
    {
        IcosphereGenerator.SetupIcoSphereMesh("bubble", this.transform, this.transform.position + 0.5f * radius * Vector3.up, radius, 2, gapLength, triMat);

        for(int i=0; i<nbPlane; i++)
        {
            float posY = 1.5f * (i * radius / nbPlane + radius / (6.0f * nbPlane));
            CylinderGenerator.SetupCylinderMesh("plane " + i, this.transform, this.transform.position + posY * Vector3.up, 0.1f * radius + radius * Mathf.Cos(Mathf.Asin((1.5f * i/(float)nbPlane) + 1.5f/(6.0f * nbPlane) - 0.5f)), radius / (3.0f * nbPlane), resolution, hasCylinderLines, gapLength, triMat, squaMat);
        }
    }

}
