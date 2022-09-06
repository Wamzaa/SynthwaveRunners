using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshBuildingGenerator)), CanEditMultipleObjects]
public class MeshBuildingGeneratorEditor : Editor
{
    public SerializedProperty type_prop;

    public SerializedProperty height_prop;
    public SerializedProperty width_prop;
    public SerializedProperty depth_prop;
    public SerializedProperty radius_prop;
    public SerializedProperty resolution_prop;
    public SerializedProperty hasCylinderLines_prop;

    public SerializedProperty windowType_prop;
    public SerializedProperty windowSize_prop;
    public SerializedProperty windowGap_prop;
    public SerializedProperty bothSide_prop;

    public SerializedProperty alterFunction_prop;
    public SerializedProperty nbAlter_prop;
    public SerializedProperty nbPlane_prop;
    public SerializedProperty isConvex_prop;
    public SerializedProperty bonesWidth_prop;
    public SerializedProperty edgeReduction_prop;
    public SerializedProperty offsetReduction_prop;
    public SerializedProperty levelRadius_prop;
    public SerializedProperty levelHeight_prop;
    public SerializedProperty nbSpiral_prop;
    public SerializedProperty nbLap_prop;

    public SerializedProperty gapLength_prop;
    public SerializedProperty triMat_prop;
    public SerializedProperty squaMat_prop;



    private void OnEnable()
    {
        type_prop = serializedObject.FindProperty("type");

        height_prop = serializedObject.FindProperty("height");
        width_prop = serializedObject.FindProperty("width");
        depth_prop = serializedObject.FindProperty("depth");
        radius_prop = serializedObject.FindProperty("radius");
        resolution_prop = serializedObject.FindProperty("resolution");
        hasCylinderLines_prop = serializedObject.FindProperty("hasCylinderLines");

        windowType_prop = serializedObject.FindProperty("windowType");
        windowSize_prop = serializedObject.FindProperty("windowSize");
        windowGap_prop = serializedObject.FindProperty("windowGap");
        bothSide_prop = serializedObject.FindProperty("bothSide");

        alterFunction_prop = serializedObject.FindProperty("alterFunction");
        nbAlter_prop = serializedObject.FindProperty("nbAlter");
        nbPlane_prop = serializedObject.FindProperty("nbPlane");
        isConvex_prop = serializedObject.FindProperty("isConvex");
        bonesWidth_prop = serializedObject.FindProperty("bonesWidth");
        edgeReduction_prop = serializedObject.FindProperty("edgeReduction");
        offsetReduction_prop = serializedObject.FindProperty("offsetReduction");
        levelRadius_prop = serializedObject.FindProperty("levelRadius");
        levelHeight_prop = serializedObject.FindProperty("levelHeight");
        nbSpiral_prop = serializedObject.FindProperty("nbSpiral");
        nbLap_prop = serializedObject.FindProperty("nbLap");

        gapLength_prop = serializedObject.FindProperty("gapLength");
        triMat_prop = serializedObject.FindProperty("triMat");
        squaMat_prop = serializedObject.FindProperty("squaMat");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(type_prop);

        MeshBuildingGenerator.BuildingType type = (MeshBuildingGenerator.BuildingType)type_prop.enumValueIndex;

        switch (type)
        {
            case MeshBuildingGenerator.BuildingType.SimpleBloc:
                EditorGUILayout.PropertyField(height_prop);
                EditorGUILayout.PropertyField(width_prop);
                EditorGUILayout.PropertyField(depth_prop);
                EditorGUILayout.PropertyField(bothSide_prop);
                break;
            case MeshBuildingGenerator.BuildingType.CircularBloc:
                EditorGUILayout.PropertyField(height_prop);
                EditorGUILayout.PropertyField(radius_prop);
                EditorGUILayout.PropertyField(resolution_prop);
                EditorGUILayout.PropertyField(hasCylinderLines_prop);
                break;
            case MeshBuildingGenerator.BuildingType.AlterTower:
                EditorGUILayout.PropertyField(height_prop);
                EditorGUILayout.PropertyField(width_prop);
                EditorGUILayout.PropertyField(depth_prop);

                EditorGUILayout.PropertyField(alterFunction_prop);
                EditorGUILayout.PropertyField(nbAlter_prop);
                break;
            case MeshBuildingGenerator.BuildingType.CircularAlterTower:
                EditorGUILayout.PropertyField(height_prop);
                EditorGUILayout.PropertyField(radius_prop);
                EditorGUILayout.PropertyField(resolution_prop);
                EditorGUILayout.PropertyField(hasCylinderLines_prop);

                EditorGUILayout.PropertyField(alterFunction_prop);
                EditorGUILayout.PropertyField(nbAlter_prop);
                break;
            case MeshBuildingGenerator.BuildingType.BubbleTemple:
                EditorGUILayout.PropertyField(radius_prop);
                EditorGUILayout.PropertyField(resolution_prop);
                EditorGUILayout.PropertyField(hasCylinderLines_prop);

                EditorGUILayout.PropertyField(nbPlane_prop);
                break;
            case MeshBuildingGenerator.BuildingType.SkelTower:
                EditorGUILayout.PropertyField(height_prop);
                EditorGUILayout.PropertyField(radius_prop);
                EditorGUILayout.PropertyField(resolution_prop);

                EditorGUILayout.PropertyField(nbPlane_prop);
                EditorGUILayout.PropertyField(isConvex_prop);
                EditorGUILayout.PropertyField(bonesWidth_prop);
                break;
            case MeshBuildingGenerator.BuildingType.LevelSkyscrapper:
                EditorGUILayout.PropertyField(height_prop);
                EditorGUILayout.PropertyField(width_prop);
                EditorGUILayout.PropertyField(depth_prop);

                EditorGUILayout.PropertyField(edgeReduction_prop);
                EditorGUILayout.PropertyField(offsetReduction_prop);
                break;
            case MeshBuildingGenerator.BuildingType.EllipticLevelSkyScrapper:
                EditorGUILayout.PropertyField(height_prop);
                EditorGUILayout.PropertyField(width_prop);
                EditorGUILayout.PropertyField(depth_prop);
                EditorGUILayout.PropertyField(resolution_prop);

                EditorGUILayout.PropertyField(edgeReduction_prop);
                EditorGUILayout.PropertyField(offsetReduction_prop);
                break;
            case MeshBuildingGenerator.BuildingType.CircularSpiralTower:
                EditorGUILayout.PropertyField(height_prop);
                EditorGUILayout.PropertyField(radius_prop);
                EditorGUILayout.PropertyField(resolution_prop);
                EditorGUILayout.PropertyField(hasCylinderLines_prop);

                EditorGUILayout.PropertyField(levelRadius_prop);
                EditorGUILayout.PropertyField(levelHeight_prop);
                EditorGUILayout.PropertyField(nbSpiral_prop);
                EditorGUILayout.PropertyField(nbLap_prop);
                break;
            case MeshBuildingGenerator.BuildingType.CubicSpiralTower:
                EditorGUILayout.PropertyField(height_prop);
                EditorGUILayout.PropertyField(radius_prop);

                EditorGUILayout.PropertyField(levelRadius_prop);
                EditorGUILayout.PropertyField(levelHeight_prop);
                EditorGUILayout.PropertyField(nbSpiral_prop);
                EditorGUILayout.PropertyField(nbLap_prop);
                EditorGUILayout.PropertyField(isConvex_prop);
                break;
        }

        if(type == MeshBuildingGenerator.BuildingType.SimpleBloc || type == MeshBuildingGenerator.BuildingType.CircularBloc)
        {
            EditorGUILayout.PropertyField(windowType_prop);
            EditorGUILayout.PropertyField(windowSize_prop);
            EditorGUILayout.PropertyField(windowGap_prop);
        }

        EditorGUILayout.PropertyField(gapLength_prop);
        EditorGUILayout.PropertyField(triMat_prop);
        EditorGUILayout.PropertyField(squaMat_prop);

        serializedObject.ApplyModifiedProperties();
    }
}
