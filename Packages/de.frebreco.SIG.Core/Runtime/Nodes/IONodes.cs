using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GraphProcessor;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class IONode : SIGNode
{
    public const string IO = "IO";

    public override Color color => new Color(0.91f, 0.66f, 0.22f);
}

public abstract class PathIONode : IONode
{
    [SerializeField, Input("Directory")]
    public string directory = "./";
    
    [SerializeField, Input("File")]
    public string file;

    public string AbsoluteDirectory =>
        directory.StartsWith(".") ? System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, directory)) : directory;
    public string Path => System.IO.Path.Combine(AbsoluteDirectory, file);
}

public abstract class SaveFileNode : PathIONode
{
    protected abstract byte[] Data { get; }
    protected virtual bool HasData => true;
    
    protected override void Process(SIGProcessingContext context)
    {
        if(!HasData) return;
        
        var data = Data;
        if(!Assert(data != null && data.Length > 0, "Empty data can not be saved")) return;

        using (var fs = new FileStream(Path, FileMode.OpenOrCreate))
        {
            fs.Write(data, 0, data.Length);
        }
        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif
    }
}

[System.Serializable, NodeMenuItem(IO + "/Save Image")]
public class SaveImageNode : SaveFileNode
{
    [Input("Texture")]
    public Texture texture;
    
    public enum TextureFileFormat
    {
        PNG, JPG, EXR, TGA
    }

    [SerializeField]
    public TextureFileFormat format;

    protected override bool HasData => IsConnected(nameof(texture));

    protected override byte[] Data
    {
        get
        {
            switch (texture)
            {
                case Texture2D tex2D:
                    return Encode(tex2D);
                case RenderTexture renderTex:
                {
                    Texture2D temp = renderTex.ToTexture2D();
                    var data = Encode(temp);
                    Object.DestroyImmediate(temp);

                    return data;
                }
                default:
                    return null;
            }
        }
    }

    protected byte[] Encode(Texture2D tex) => format switch
    {
        TextureFileFormat.PNG => tex.EncodeToPNG(),
        TextureFileFormat.JPG => tex.EncodeToJPG(),
        TextureFileFormat.EXR => tex.EncodeToEXR(),
        TextureFileFormat.TGA => tex.EncodeToTGA(),
        _ => throw new FormatException()
    };
}
