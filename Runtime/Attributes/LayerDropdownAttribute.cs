using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class LayerDropdownAttribute : PropertyAttribute
{
    
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LayerDropdownAttribute))]
public class LayerDropdownPropertyDrawer : PropertyDrawer 
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        if (_property.propertyType != SerializedPropertyType.Integer)
        {
            EditorGUI.PropertyField(_position, _property, _label);
        }
        else
        {
            EditorGUI.BeginProperty(_position, GUIContent.none, _property);
            _property.intValue = EditorGUI.LayerField(_position, _label, _property.intValue);
            EditorGUI.EndProperty();
        }
    }
}
#endif
