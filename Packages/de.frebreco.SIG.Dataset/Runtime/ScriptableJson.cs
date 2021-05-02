using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ScriptableJson : ScriptableObject
{
    public abstract string ToJson(bool pretty = false);
    public abstract byte[] EncodedJson { get; }

    public abstract void LoadJson(string json);
}

public abstract class ScriptableJson<T> : ScriptableJson
    where T : struct
{
    [SerializeField] public T data;

    public override string ToJson(bool pretty = false) => JsonConverter.ToJson(data, pretty);
    public override byte[] EncodedJson => ToJson().EncodeUTF8();

    public override void LoadJson(string json)
    {
        data = JsonConverter.FromJson<T>(json);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableJson), true)]
public class ScriptableJsonEditor : Editor
{
    protected ScriptableJson Target => target as ScriptableJson;
    protected string json;
    protected bool editMode = false;

    private void OnEnable()
    {
        json = Target.ToJson(true);
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck() && !editMode) json = Target.ToJson(true);

        if(GUILayout.Button(editMode ? "Save" : "Edit Raw Json", GUILayout.Width(150)))
        {
            if(editMode) Target.LoadJson(json);
            editMode = !editMode;
        }
        
        using (new EditorGUI.DisabledScope(!editMode))
        {
            json = EditorGUILayout.TextArea(json);
        }
    }
}
#endif
