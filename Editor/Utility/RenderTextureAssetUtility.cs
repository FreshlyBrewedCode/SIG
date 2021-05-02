using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CreateRenderTextureMenu
{
    [MenuItem("Assets/Create/sRGB RenderTexture")]
    public static void CreateSRGBRenderTexture()
    {
        var path = Path.Combine(GetProjectWindowPath(), "sRGBRenderTexture.renderTexture");
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        
        var texture = new RenderTexture(512, 512, 0, GraphicsFormat.R8G8B8A8_SRGB, 0);
        AssetDatabase.CreateAsset(texture, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static string GetProjectWindowPath()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            break;
        }

        return path;
    }

    [MenuItem("Assets/Export RenderTexture")]
    public static void ExportRenderTexture()
    {
        if (Selection.activeObject is RenderTexture tex)
        {
            var path = EditorUtility.SaveFilePanel("Export RenderTexture", GetProjectWindowPath(),
                tex.name, "png");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllBytes(path, new UTexture(tex).ConvertTo(UTextureFormat.PNG).RawTexture);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
    
    [MenuItem("Assets/Export RenderTexture", validate = true)]
    public static bool ValidateExportRenderTexture()
    {
        return Selection.activeObject is RenderTexture;
    }
}
