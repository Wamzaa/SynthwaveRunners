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

    public SerializedProperty initialRoadRotation_prop;

    public SerializedProperty material_prop;

    private void OnEnable()
    {
        meshMode_prop = serializedObject.FindProperty("mode");
        roadMode_prop = serializedObject.FindProperty("roadMode");
        controlPointsList_prop = serializedObject.FindProperty("controlPointsList");
        resolution_prop = serializedObject.FindProperty("resolution");
        pointResolution_prop = serializedObject.FindProperty("pointResolution");
        initialRoadRotation_prop = serializedObject.FindProperty("initialRoadRotation");
        material_prop = serializedObject.FindProperty("mat");
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
                EditorGUILayout.PropertyField(initialRoadRotation_prop);
                break;
        }

        EditorGUILayout.PropertyField(material_prop);

        serializedObject.ApplyModifiedProperties();
    }

}
