using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SIGTask), true)]
public class SIGTaskEditor : Editor<SIGTask>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Run Task")) RunTask();
    }

    protected void RunTask()
    {
        var processor = FindObjectOfType<SIGProcessor>();

        if (processor == null)
        {
            var processorObj = new GameObject("SIGProcessor");
            processor = processorObj.AddComponent<SIGProcessor>();
        }
        
        processor.RunTask(Target);
    }
}
