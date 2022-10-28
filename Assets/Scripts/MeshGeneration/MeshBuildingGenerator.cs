using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuildingGenerator : MonoBehaviour
{
    [Header("--- Primary Settings ---")]
    public BuildingType type;
    public enum BuildingType { SimpleBloc, CircularBloc, AlterTower, CircularAlterTower, BubbleTemple, SkelTower, LevelSkyscrapper, EllipticLevelSkyScrapper, CircularSpiralTower, CubicSpiralTower};

    public float height;
    public float width;
    public float depth;
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
    public enum FunctionShape { Linear, ZigZag, SquareRoot, Square, Keel, LinSin};

    public int nbAlter;

    [Header("--- Specific Settings ---")]
    //Bubble Temple & Skel Tower Settings
    public int nbPlane;

    //Skel Tower Settings
    public bool isConvex;
    public float bonesWidth;

    [Header("--- Specific Settings ---")]
    //Level Skyscrapper Settings
    public float edgeReduction;
    public Vector2 offsetReduction;

    [Header("--- Specific Settings ---")]
    //Spiral Tower Settings
    public float levelRadius;
    public float levelHeight;
    public int nbSpiral;
    public float nbLap;

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
        else if(type == BuildingType.SkelTower)
        {
            BuildSkelTowerBuilding();
        }
        else if (type == BuildingType.LevelSkyscrapper)
        {
            BuildLevelSkyscrapperBuilding();
        }
        else if(type == BuildingType.EllipticLevelSkyScrapper)
        {
            BuildEllipticLevelSkyscrapperBuilding();
        }
        else if (type == BuildingType.CircularSpiralTower)
        {
            BuildCircularSpiralTowerBuilding();
        }
        else if (type == BuildingType.CubicSpiralTower)
        {
            BuildCubicSpiralTowerBuilding();
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
        Vector3 baseCubeScale = new Vector3(width * 0.15f, height, depth * 0.15f);
        CubeGenerator.SetupCubeMesh("base(Building)", this.transform, this.transform.position + height * 0.5f * Vector3.up, baseCubeScale, gapLength, squaMat);
        

        float alterLen = 0.5f * height / nbAlter;
        for (int i = 0; i < nbAlter; i++)
        {
            float ratioPos = ((2.0f * i + 1.0f) - 0.5f) / (nbAlter * 2.0f);
            Vector3 windowCubeScale = Vector3.zero;

            switch (alterFunction)
            {
                case FunctionShape.Linear:
                    windowCubeScale = new Vector3(((1.0f - ratioPos) * 0.85f + 0.15f)* width, alterLen, ((1.0f - ratioPos) * 0.85f + 0.15f) * depth);
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
                    windowCubeScale = new Vector3((Mathf.Sqrt(1.0f - ratioPos) * 0.85f + 0.15f) * width, alterLen, (Mathf.Sqrt(1.0f - ratioPos) * 0.85f + 0.15f) * depth);
                    break;
                case FunctionShape.Square:
                    windowCubeScale = new Vector3(((1.0f - ratioPos) * (1.0f - ratioPos) * 0.85f + 0.15f) * width, alterLen, ((1.0f - ratioPos) * (1.0f - ratioPos) * 0.85f + 0.15f) * depth);
                    break;
                case FunctionShape.Keel:
                    if(ratioPos < 2.0f / 3.0f)
                    {
                        windowCubeScale = new Vector3(((1.0f - 1.5f * ratioPos) * (1.0f - 1.5f * ratioPos) * 0.85f + 0.15f) * width, alterLen, ((1.0f - 1.5f * ratioPos) * (1.0f - 1.5f * ratioPos) * 0.85f + 0.15f) * depth);
                    }
                    else
                    {
                        windowCubeScale = new Vector3(((3.0f * ratioPos - 2.0f) * (3.0f * ratioPos - 2.0f) * 0.85f + 0.15f) * width, alterLen, ((3.0f * ratioPos - 2.0f) * (3.0f * ratioPos - 2.0f) * 0.85f + 0.15f) * depth);
                    }
                    break;
                case FunctionShape.LinSin:
                    windowCubeScale = new Vector3(((1.0f - ratioPos + 0.2f * Mathf.Sin(2 * Mathf.PI * ratioPos * height / (0.5f * width + 0.5f * depth))) * 0.85f + 0.15f) * width, alterLen, ((1.0f - ratioPos + +0.2f * Mathf.Sin(2 * Mathf.PI * ratioPos * height / (0.5f * width + 0.5f * depth))) * 0.85f + 0.15f) * depth);
                    break;
            }

            CubeGenerator.SetupCubeMesh("alter(Building)-" + i, this.transform, this.transform.position + ((2 * i + 1) * alterLen - 0.5f * alterLen) * Vector3.up, windowCubeScale, gapLength, squaMat);
            if (alterFunction == FunctionShape.Keel)
            {
                IcosphereGenerator.SetupIcoSphereMesh("head(keelTower)", this.transform, this.transform.position + (height + (0.25f * width + 0.25f * depth)) * Vector3.up, (0.25f * width + 0.25f * depth), 1, gapLength, triMat);
            }
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
                case FunctionShape.LinSin:
                    windowRadius = ((1.0f - ratioPos + +0.2f * Mathf.Sin(2 * Mathf.PI * ratioPos * height / (1.0f*radius))) * 0.85f + 0.15f) * radius;
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

    public void BuildSkelTowerBuilding()
    {
        Vector3 boneSize = new Vector3(bonesWidth, height, bonesWidth);
        for(int i =0; i<resolution; i++)
        {
            Transform bone = CubeGenerator.SetupCubeMesh("bone-" + i, this.transform, this.transform.position + (height / 2) * Vector3.up + radius * Mathf.Cos(2 * Mathf.PI * i / resolution) * Vector3.right + radius * Mathf.Sin(2 * Mathf.PI * i / resolution) * Vector3.forward, boneSize, gapLength, squaMat);
            bone.LookAt(this.transform.position + (height / 2) * Vector3.up);
            if (!isConvex)
            {
                bone.Rotate(Vector3.up, 45.0f);
            }
        }

        for(int j = 1; j<=nbPlane; j++)
        {
            CylinderGenerator.SetupCylinderMesh("plane-" + j, this.transform, this.transform.position + (j * height / nbPlane) * Vector3.up, radius, bonesWidth, resolution, false, gapLength, triMat, squaMat);
        }
    }

    public void BuildLevelSkyscrapperBuilding()
    {
        bool isMinus = true;
        bool loopActive = true;
        Vector3 currentPosition = this.transform.position + (height / 2) * Vector3.up;
        Vector3 currentSize = new Vector3(width, height, depth);
        CubeGenerator.SetupCubeMesh("baseLevel", this.transform, currentPosition, currentSize, gapLength, squaMat);
        while (loopActive)
        {
            if (isMinus)
            {
                currentPosition.x = currentPosition.x - offsetReduction.x / 2;
                currentPosition.y = currentPosition.y + currentSize.y + edgeReduction / 2;
                currentPosition.z = currentPosition.z - offsetReduction.y / 2;
            }
            else
            {
                currentPosition.x = currentPosition.x + offsetReduction.x / 2;
                currentPosition.y = currentPosition.y + currentSize.y + edgeReduction / 2;
                currentPosition.z = currentPosition.z + offsetReduction.y / 2;
            }
            isMinus = !isMinus;

            currentSize.x = currentSize.x - edgeReduction - offsetReduction.x;
            currentSize.y = currentSize.y + edgeReduction;
            currentSize.z = currentSize.z - edgeReduction - offsetReduction.y;

            if (currentSize.x > 0 && currentSize.z > 0)
            {
                Transform level = CubeGenerator.SetupCubeMesh("level", this.transform, currentPosition, currentSize, gapLength, squaMat);

                int nbWindows = (int)Mathf.Round(currentSize.y / (2 * edgeReduction)) - 1;
                for (int i = 1; i <= nbWindows; i++)
                {
                    Vector3 windowCubeScale = new Vector3(currentSize.x + gapLength, edgeReduction, Mathf.Max(currentSize.z - 2 * edgeReduction - 2 * gapLength, 0));
                    CubeGenerator.SetupCubeMesh("window(SkyScrapper)-" + i, level, currentPosition + i * (currentSize.y / (nbWindows + 1)) * Vector3.up - (currentSize.y / 2) * Vector3.up, windowCubeScale, 2 * edgeReduction, squaMat);
                    windowCubeScale = new Vector3(Mathf.Max(currentSize.x - 2 * edgeReduction, 0), edgeReduction, currentSize.z + gapLength);
                    CubeGenerator.SetupCubeMesh("window(SkyScrapper)-" + i, level, currentPosition + i * (currentSize.y / (nbWindows + 1)) * Vector3.up - (currentSize.y / 2) * Vector3.up, windowCubeScale, 2 * edgeReduction, squaMat);
                }
            }
            else
            {
                loopActive = false;
            }
        }
    }

    public void BuildEllipticLevelSkyscrapperBuilding()
    {
        bool isMinus = true;
        bool loopActive = true;
        Vector3 currentPosition = this.transform.position + (height / 2) * Vector3.up;
        Vector3 currentSize = new Vector3(width, height, depth);
        EllipseGenerator.SetupEllipseMesh("baseLevel", this.transform, currentPosition, currentSize.y, currentSize.x, currentSize.z, resolution, false, gapLength, triMat, squaMat);
        while (loopActive)
        {
            if (isMinus)
            {
                currentPosition.x = currentPosition.x - offsetReduction.x / 2;
                currentPosition.y = currentPosition.y + currentSize.y + edgeReduction / 2;
                currentPosition.z = currentPosition.z - offsetReduction.y / 2;
            }
            else
            {
                currentPosition.x = currentPosition.x + offsetReduction.x / 2;
                currentPosition.y = currentPosition.y + currentSize.y + edgeReduction / 2;
                currentPosition.z = currentPosition.z + offsetReduction.y / 2;
            }
            isMinus = !isMinus;

            currentSize.x = currentSize.x - edgeReduction - offsetReduction.x;
            currentSize.y = currentSize.y + edgeReduction;
            currentSize.z = currentSize.z - edgeReduction - offsetReduction.y;

            if (currentSize.x > 0 && currentSize.z > 0)
            {
                Transform level = EllipseGenerator.SetupEllipseMesh("level", this.transform, currentPosition, currentSize.y, currentSize.x, currentSize.z, resolution, false, gapLength, triMat, squaMat);

                int nbWindows = (int)Mathf.Round(currentSize.y / (2 * edgeReduction)) - 1;
                for (int i = 1; i <= nbWindows; i++)
                {
                    EllipseGenerator.SetupEllipseMesh("window(SkyScrapper)-" + i, level, currentPosition + i * (currentSize.y / (nbWindows + 1)) * Vector3.up - (currentSize.y / 2) * Vector3.up, edgeReduction, currentSize.x + gapLength, currentSize.z + gapLength, resolution, false, 2 * edgeReduction, triMat, squaMat);
                }
            }
            else
            {
                loopActive = false;
            }
        }
    }

    public void BuildCircularSpiralTowerBuilding()
    {
        float nbLevel = Mathf.Ceil(height / levelHeight);
        float angleStep = 2 * Mathf.PI * nbLap / nbLevel;
        for(int j=0; j<nbSpiral; j++)
        {
            float offAngle = 2 * Mathf.PI * j / ((float)nbSpiral);
            for (int i = 0; i < nbLevel; i++)
            {
                CylinderGenerator.SetupCylinderMesh("level" + i + "/" + j, this.transform, this.transform.position + radius * (Mathf.Cos(i * angleStep + offAngle) * Vector3.right + Mathf.Sin(i * angleStep + offAngle) * Vector3.forward) + (i + 0.5f) * levelHeight * Vector3.up, levelRadius, levelHeight, resolution, hasCylinderLines, gapLength, triMat, squaMat);
            }
        }
    }

    public void BuildCubicSpiralTowerBuilding()
    {
        float nbLevel = Mathf.Ceil(height / levelHeight);
        float angleStep = 2 * Mathf.PI * nbLap / nbLevel;
        for (int j = 0; j < nbSpiral; j++)
        {
            float offAngle = 2 * Mathf.PI * j / ((float)nbSpiral);
            for (int i = 0; i < nbLevel; i++)
            {
                Transform cube = CubeGenerator.SetupCubeMesh("level" + i + "/" + j, this.transform, this.transform.position + radius * (Mathf.Cos(i * angleStep + offAngle) * Vector3.right + Mathf.Sin(i * angleStep + offAngle) * Vector3.forward) + (i + 0.5f) * levelHeight * Vector3.up, new Vector3(2*levelRadius, levelHeight, 2*levelRadius), gapLength, squaMat);
                cube.LookAt(this.transform.position + (i + 0.5f) * levelHeight * Vector3.up);
                if(!isConvex)
                {
                    cube.Rotate(Vector3.up, 45.0f);
                }
            }
        }
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (type == BuildingType.SimpleBloc)
        {
            DrawRotatedWireCube(width, height, depth);
        }
        else if (type == BuildingType.CircularBloc)
        {
            DrawRotatedWireCube(2 * radius, height, 2 * radius);
        }
        else if (type == BuildingType.AlterTower)
        {
            DrawRotatedWireCube(width, height, depth);
        }
        else if (type == BuildingType.CircularAlterTower)
        {
            DrawRotatedWireCube(2 * radius, height, 2 * radius);
        }
        else if (type == BuildingType.BubbleTemple)
        {
            DrawRotatedWireCube(2 * radius, 2 * radius, 2 * radius);
        }
        else if (type == BuildingType.SkelTower)
        {
            DrawRotatedWireCube(2 * radius, height, 2 * radius);
        }
        else if (type == BuildingType.LevelSkyscrapper)
        {
            DrawRotatedWireCube(width, 10 * height, depth);
        }
        else if (type == BuildingType.EllipticLevelSkyScrapper)
        {
            DrawRotatedWireCube(width, 10 * height, depth);
        }
        else if (type == BuildingType.CircularSpiralTower)
        {
            DrawRotatedWireCube(2 * radius, height, 2 * radius);
        }
        else if (type == BuildingType.CubicSpiralTower)
        {
            DrawRotatedWireCube(2 * radius, height, 2 * radius);
        }
    }

    public void DrawRotatedWireCube(float width, float height, float depth)
    {
        Vector3 pos = this.transform.position;
        Vector3 x = (width/2) * this.transform.right;
        Vector3 y = height * this.transform.up;
        Vector3 z = (depth/2) * this.transform.forward;

        Gizmos.DrawLine(pos - x - z, pos + x - z);
        Gizmos.DrawLine(pos + x - z, pos + x + z);
        Gizmos.DrawLine(pos + x + z, pos - x + z);
        Gizmos.DrawLine(pos - x + z, pos - x - z);

        Gizmos.DrawLine(pos - x - z, pos - x - z + y);
        Gizmos.DrawLine(pos + x - z, pos + x - z + y);
        Gizmos.DrawLine(pos + x + z, pos + x + z + y);
        Gizmos.DrawLine(pos - x + z, pos - x + z + y);

        Gizmos.DrawLine(pos - x - z + y, pos + x - z + y);
        Gizmos.DrawLine(pos + x - z + y, pos + x + z + y);
        Gizmos.DrawLine(pos + x + z + y, pos - x + z + y);
        Gizmos.DrawLine(pos - x + z + y, pos - x - z + y);
    }

}
