using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum UTextureFormat
{
    Unity, PNG, JPG, EXR, TGA
}

public class UTexture
{
    protected Texture unityTexture;
    protected byte[] rawTexture;
    protected UTextureFormat format;

    public bool IsUnityTexture => format == UTextureFormat.Unity && unityTexture != null;
    public bool IsRenderTexture => unityTexture is RenderTexture;

    public Texture UnityTexture => unityTexture;
    public Texture2D AsTexture2D
    {
        get
        {
            if (!IsUnityTexture) ToUnity();
            
            if (unityTexture is Texture2D tex) return tex;
            if (unityTexture is RenderTexture renderTex)
            {
                unityTexture = renderTex.ToTexture2D();
                return (Texture2D) unityTexture;
            }

            throw new InvalidCastException("Unable to cast unity texture to Texture2D");
        }
    }

    public int Width => unityTexture.width;
    public int Height => unityTexture.height;
    
    public byte[] RawTexture => rawTexture;

    public string FileExtension => format switch
    {
        UTextureFormat.Unity => ".asset",
        UTextureFormat.PNG => ".png",
        UTextureFormat.JPG => ".jpg",
        UTextureFormat.EXR => ".exr",
        UTextureFormat.TGA => ".tga",
        _ => throw new InvalidEnumArgumentException("Unsupported texture format.")
    };

    public UTexture(Texture unityTexture)
    {
        this.unityTexture = unityTexture;
        format = UTextureFormat.Unity;
        rawTexture = null;
    }
    
    public UTexture(byte[] rawTexture, UTextureFormat format)
    {
        this.unityTexture = null;
        this.format = format;
        this.rawTexture = rawTexture;
        
        if(format == UTextureFormat.Unity) 
            throw new InvalidEnumArgumentException("Unity texture format can not be used with rawTexture data");
    }

    public UTexture ConvertTo(UTextureFormat format)
    {
        if(this.format == format) return this;
        ToUnity();
        
        return format switch
        {
            UTextureFormat.Unity => this,
            UTextureFormat.PNG => FromRawBytes(AsTexture2D.EncodeToPNG(), UTextureFormat.PNG),
            UTextureFormat.JPG => FromRawBytes(AsTexture2D.EncodeToJPG(), UTextureFormat.JPG),
            UTextureFormat.EXR => FromRawBytes(AsTexture2D.EncodeToEXR(), UTextureFormat.EXR),
            UTextureFormat.TGA => FromRawBytes(AsTexture2D.EncodeToTGA(), UTextureFormat.TGA),
            _ => throw new InvalidEnumArgumentException("Unsupported texture format.")
        };

        UTexture FromRawBytes(byte[] data, UTextureFormat dataFormat)
        {
            this.rawTexture = data;
            this.format = dataFormat;
            return this;
        }
    }

    public UTexture ToUnity()
    {
        if (IsUnityTexture) return this;

        var tex = new Texture2D(1, 1);
        tex.LoadImage(rawTexture);
        this.unityTexture = tex;
        this.format = UTextureFormat.Unity;
        
        return this;
    }
}

public static class TextureExtensions
{
    public static Texture2D ToTexture2D(this RenderTexture renderTexture)
    {
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
        var active = RenderTexture.active;
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = active;

        tex.hideFlags = HideFlags.DontSave;
        
        return tex;
    }
}
