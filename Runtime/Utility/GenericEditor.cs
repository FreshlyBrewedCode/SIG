using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class Editor<T> : Editor where T : Object
{
    protected T Target => target as T;
}

#endif