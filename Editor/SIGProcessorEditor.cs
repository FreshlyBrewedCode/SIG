using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SIGProcessor))]
public class SIGProcessorEditor : Editor
{
    public SIGProcessor Target => target as SIGProcessor;

    protected SerializedProperty graphProperty;
    protected SerializedProperty taskProperty;

    private void OnEnable()
    {
        graphProperty = serializedObject.FindProperty("graph");
        taskProperty = serializedObject.FindProperty("task");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        using (new EditorGUI.DisabledScope(graphProperty.objectReferenceValue == null))
        {
            if (GUILayout.Button("Process"))
            {
                Target.Process(true);
            }
        }
        
        using (new EditorGUI.DisabledScope(taskProperty.objectReferenceValue == null))
        {
            if (GUILayout.Button("Run Task"))
            {
                Target.RunTask(taskProperty.objectReferenceValue as SIGTask);
            }
        }
    }
}
