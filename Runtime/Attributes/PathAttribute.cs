using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class PathAttribute : PropertyAttribute
{
    public PathPanel panelType;
    public string title = "Select Path";
    public string defaultName = "";
    public string extensions = "";

    public PathAttribute()
    {
        panelType = PathPanel.OpenFile;
    }

    public PathAttribute(PathPanel type)
    {
        panelType = type;
    }
}

public enum PathPanel
{
    OpenFile, OpenFolder, SaveFile, SaveFolder
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PathAttribute))]
public class PropertyAttributePropertyDrawer : PropertyDrawer 
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        if (_property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(_position, _property, _label);
        }
        else
        {
            
            var rects = _position.AbsSplitX(-25f);
            EditorGUI.PropertyField(
                rects[0].Padding(0, 0, 0, 1f), 
                _property, _label);
            if (GUI.Button(rects[1], "...", "minibutton"))
            {
                _property.stringValue = ShowPanel(_property.stringValue);
                _property.serializedObject.ApplyModifiedProperties();
            }
        }
    }

    public string ShowPanel(string path)
    {
        var attr = attribute as PathAttribute;

        var basePath = string.IsNullOrEmpty(path) ? Application.dataPath : path;
        var newPath = attr.panelType switch
        {
            PathPanel.OpenFile => EditorUtility.OpenFilePanel(attr.title, basePath, attr.extensions),
            PathPanel.OpenFolder => EditorUtility.OpenFolderPanel(attr.title, basePath, attr.defaultName),
            PathPanel.SaveFile => EditorUtility.SaveFilePanel(attr.title, basePath, attr.defaultName, attr.extensions),
            PathPanel.SaveFolder => EditorUtility.SaveFolderPanel(attr.title, basePath, attr.defaultName),
            _ => throw new InvalidEnumArgumentException()
        };

        if (string.IsNullOrEmpty(newPath)) return path;
        return newPath;
    }
}
#endif