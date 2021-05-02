using System;
using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public class TextureConverter : ITypeAdapter
{
   // UTexture
   public static UTexture TextureToUTexture(Texture texture) => new UTexture(texture);
   public static Texture UTextureToTexture(UTexture texture) => texture.ToUnity().UnityTexture;
   public static UTexture Texture2DToUTexture(Texture2D texture) => new UTexture(texture);
   public static Texture2D UTextureToTexture2D(UTexture texture) => texture.AsTexture2D;
   
   // Unity Textures
   public static Texture2D TextureToTexture2D(Texture texture)
   {
      return texture switch
      {
         Texture2D tex2d => tex2d,
         RenderTexture renderTex => renderTex.ToTexture2D(),
         _ => throw new InvalidCastException("Unable to cast texture to Texture2D")
      };
   }

   public static Texture Texture2DToTexture(Texture2D tex) => tex;
}
