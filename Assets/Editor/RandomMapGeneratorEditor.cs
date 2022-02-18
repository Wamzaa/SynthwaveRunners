using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomMapGenerator)), CanEditMultipleObjects]
public class RandomMapGeneratorEditor : Editor
{
    public SerializedProperty seeded_prop;
    public SerializedProperty seed_prop;

    //temporary
    public SerializedProperty mat_prop;

    private void OnEnable()
    {
        seeded_prop = serializedObject.FindProperty("seeded");
        seed_prop = serializedObject.FindProperty("seed");
        mat_prop = serializedObject.FindProperty("mat");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(seeded_prop);
        bool seeded = (bool)seeded_prop.boolValue;

        if (seeded)
        {
            EditorGUILayout.PropertyField(seed_prop);
            EditorGUILayout.PropertyField(mat_prop);
        }
        else
        {
            EditorGUILayout.PropertyField(mat_prop);
        }

        serializedObject.ApplyModifiedProperties();
    }

}
