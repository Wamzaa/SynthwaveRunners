using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator)), CanEditMultipleObjects]
public class MeshGeneratorEditor : Editor
{
    public SerializedProperty meshMode_prop;

    public SerializedProperty roadMode_prop; 

    public SerializedProperty controlPointsList_prop;
    public SerializedProperty resolution_prop;
    public SerializedProperty pointResolution_prop;
    public SerializedProperty widthResolution_prop;

    public SerializedProperty initialRoadRotation_prop;

    public SerializedProperty material_prop;
    public SerializedProperty edgeMaterial_prop;
    
    public SerializedProperty withRoadLight_prop;
    public SerializedProperty roadLightScale_prop;
    public SerializedProperty roadLightInter_prop;

    public SerializedProperty showGizmos_prop;


    private void OnEnable()
    {
        meshMode_prop = serializedObject.FindProperty("mode");
        roadMode_prop = serializedObject.FindProperty("roadMode");
        controlPointsList_prop = serializedObject.FindProperty("controlPointsList");
        resolution_prop = serializedObject.FindProperty("resolution");
        pointResolution_prop = serializedObject.FindProperty("pointResolution");
        widthResolution_prop = serializedObject.FindProperty("widthResolution");
        initialRoadRotation_prop = serializedObject.FindProperty("initialRoadRotation");
        material_prop = serializedObject.FindProperty("mat");
        edgeMaterial_prop = serializedObject.FindProperty("edgeMat");
        withRoadLight_prop = serializedObject.FindProperty("withRoadLight");
        roadLightScale_prop = serializedObject.FindProperty("roadLightScale");
        roadLightInter_prop = serializedObject.FindProperty("roadLightInter");
        showGizmos_prop = serializedObject.FindProperty("showGizmos");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(meshMode_prop);
        MeshGenerator.GeneratorMode mode = (MeshGenerator.GeneratorMode)meshMode_prop.enumValueIndex;

        

        switch (mode)
        {
            case MeshGenerator.GeneratorMode.Tunnel:
                EditorGUILayout.PropertyField(controlPointsList_prop);
                EditorGUILayout.PropertyField(resolution_prop);
                EditorGUILayout.PropertyField(pointResolution_prop);
                break;

            case MeshGenerator.GeneratorMode.Road:
                EditorGUILayout.PropertyField(roadMode_prop); 
                EditorGUILayout.PropertyField(controlPointsList_prop);
                EditorGUILayout.PropertyField(resolution_prop);
                EditorGUILayout.PropertyField(pointResolution_prop);

                MeshGenerator.RoadMode rMode = (MeshGenerator.RoadMode)roadMode_prop.enumValueIndex;
                switch (rMode)
                {
                    case MeshGenerator.RoadMode.TornadoRoad:
                        EditorGUILayout.PropertyField(initialRoadRotation_prop);
                        break;
                    case MeshGenerator.RoadMode.FlatRoad:
                        EditorGUILayout.PropertyField(initialRoadRotation_prop);
                        break;
                    case MeshGenerator.RoadMode.FlexibleRoad:
                        EditorGUILayout.PropertyField(widthResolution_prop);
                        break;
                }

                EditorGUILayout.PropertyField(withRoadLight_prop);
                bool roadLightMode = (bool)withRoadLight_prop.boolValue;
                if (roadLightMode)
                {
                    EditorGUILayout.PropertyField(roadLightScale_prop);
                    EditorGUILayout.PropertyField(roadLightInter_prop);
                }
                break;
        }

        EditorGUILayout.PropertyField(material_prop);
        EditorGUILayout.PropertyField(edgeMaterial_prop);

        EditorGUILayout.PropertyField(showGizmos_prop);

        serializedObject.ApplyModifiedProperties();
    }

}
