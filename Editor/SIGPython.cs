using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Python.Runtime;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Scripting.Python;
using UnityEngine;
using Object = UnityEngine.Object;
public class SIGPython
{
    const string WORKER_NAME = "de.frebreco.sigpython.clients.worker";
    private const string CLIENT_PATH = "Packages/de.frebreco.SIG.Python/Python/client.py";
    
    public bool HasWorker => PythonRunner.IsClientConnected(WORKER_NAME);
    public bool IsInitialized => HasWorker;
    
    private static IEnumerator WaitForWorkerAndInitialize()
    {
        yield return PythonRunner.WaitForConnection(WORKER_NAME);
        Services.Init(FindPythonNodes());
    }

    public static List<string> FindPythonNodes()
    {
        List<string> nodes = new List<string>();
        
        foreach (var asset in AssetDatabase.FindAssets($"t:{nameof(SIGPythonModule)}"))
        {
            // var pythonAsset = AssetDatabase.LoadAssetAtPath<SIGPythonModule>(AssetDatabase.GUIDToAssetPath(asset));
            var path = AssetDatabase.GUIDToAssetPath(asset);
            
            nodes.Add(Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) 
                      + path);
        }

        return nodes;
    }
    
    public static dynamic CallService(string service, params object[] args)
    {
        return PythonRunner.CallServiceOnClient(WORKER_NAME, service, args);
    }

    public static dynamic CallServiceWithLock(string service, params object[] args)
    {
        dynamic output;
        using (Py.GIL())
        {
            output = CallService(service, args);
        }

        return output;
    }

    public static class Services
    {
        private const string INIT = "init";
        private const string PROCESS_NODE = "process_node";
        
        public static dynamic Init(List<string> nodes) => CallService(INIT, nodes);
        
        public static Dictionary<string, PyObject> ProcessNode(string node, PyDict inputs)
        {
            var output = CallServiceWithLock(PROCESS_NODE, node, inputs);
            Dictionary<string, PyObject> result = new Dictionary<string, PyObject>();
            foreach (var o in output)
            {
                result.Add((string)o, output[o]);
            }

            return result;
        }
    }
    
    [MenuItem("SIG/Start & Initialize Python Worker", priority = 50)]
    public static void InitializeWorker()
    {
        // The client here spawned could also be launched from the command line.
        PythonRunner.SpawnClient(CLIENT_PATH,
            wantLogging: true);
        EditorCoroutineUtility.StartCoroutineOwnerless(WaitForWorkerAndInitialize());
    }

    [MenuItem("SIG/Only Initialize Python Worker", priority = 50)]
    public static void OnlyInitializeWorker()
    {
        Services.Init(FindPythonNodes());
    }

}

public static class PythonRuntimeExtensions
{
    public static PyDict ToPyDict<K, V>(this Dictionary<K, V> dict)
    {
        var pyDict = new PyDict();
        foreach (K key in dict.Keys)
        {
            pyDict.SetItem(key.ToPython(), dict[key].ToPython());
        }
    
        return pyDict;
    }

    public static T GetOutput<T>(this Dictionary<string, PyObject> outputs, string output, T defaultValue = default)
    {
        if (outputs.TryGetValue(output, out var value)) return value.As<T>();
        return defaultValue;
    }
}