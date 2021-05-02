using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Python.Runtime;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

// [ScriptedImporter(VERSION, "py")]
// public class PythonImporter : ScriptedImporter
// {
//     private const int VERSION = 1;
//     private const string PYTHON_FOLDER = "Python";
//     
//     public override void OnImportAsset(AssetImportContext ctx)
//     {
//         if(!ctx.assetPath.EndsWith(".sig.py")) return;
//         
//         if (ctx.mainObject is PythonAsset asset)
//         {
//             asset.Refresh(ctx.assetPath);
//             return;
//         }
//         
//         var pythonAsset = ScriptableObject.CreateInstance<PythonAsset>();
//         pythonAsset.pythonPath = ctx.assetPath;
//         pythonAsset.name = ctx.assetPath.Split('/').Last().Split('.').First();
//         ctx.AddObjectToAsset(nameof(PythonAsset), pythonAsset);
//         pythonAsset.Refresh(ctx.assetPath);
//         
//         var textAsset = new TextAsset(File.ReadAllText(ctx.assetPath));
//         ctx.AddObjectToAsset(nameof(TextAsset), textAsset);
//         ctx.SetMainObject(textAsset);
//         
//         if (ctx.mainObject.name.EndsWith(".sig"))
//         {
//             ctx.mainObject.name = ctx.mainObject.name.Substring(0, ctx.mainObject.name.Length - ".sig".Length);
//         }
//     }
// }
