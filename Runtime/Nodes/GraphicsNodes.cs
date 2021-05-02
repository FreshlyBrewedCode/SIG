using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Rendering;


public abstract class SIGGraphicsNode : SIGNode
{
    public const string GRAPHICS = "Graphics";
}

[System.Serializable, NodeMenuItem(GRAPHICS + "/Blit")]
public class BlitNode : SIGGraphicsNode
{
    [SerializeField, ShowAsDrawer, Input("Src")]
    public Texture source;
    
    [SerializeField, ShowAsDrawer, Input("Dest")]
    public RenderTexture destination;
    
    [SerializeField, ShowAsDrawer, Input("Material")]
    public Material material;

    [Output("Dest")] 
    public RenderTexture output;

    protected override void Process(SIGProcessingContext context)
    {
        var hasInputs =
            AssertInput(source, "Missing Source") &&
            AssertInput(destination, "Missing Destination");
        
        if(!hasInputs) return;
        
        if(material != null)
            Graphics.Blit(source, destination, material);
        else
            Graphics.Blit(source, destination);

        output = destination;
    }
}

public abstract class ReplacementShaderNode : SIGRendererNode
{
    public abstract Shader ReplacementShader { get; }

    protected override void Process(SIGProcessingContext context)
    {
        if(!Assert(ReplacementShader != null, "Replacement shader required")) return;
        
        base.Process(context);
        Target.ResetReplacementShader();
    }

    protected override RenderTexture Render()
    {
        Target.SetReplacementShader(ReplacementShader);
        return base.Render();
    }
}

[System.Serializable, NodeMenuItem(SIGGraphicsNode.GRAPHICS + "/Render Replacement")]
public class ReplacementShaderRendererNode : ReplacementShaderNode
{
    [SerializeField, Input("Shader")]
    public Shader replacementShader;

    public override Shader ReplacementShader => replacementShader;
}

[System.Serializable, NodeMenuItem(SIGGraphicsNode.GRAPHICS + "/Render Bitmask")]
public class BitmaskRendererNode : ReplacementShaderNode
{
    private static readonly int ReplacementColorId = Shader.PropertyToID("_ReplacementColor");
    private static Shader bitmaskShader;
    protected static Shader BitmaskShader
    {
        get
        {
            if(bitmaskShader == null) bitmaskShader = Shader.Find("Hidden/SIG/Bitmask");
            return bitmaskShader;
        }
    } 
    
    [SerializeField, Input("Color")]
    public Color color = Color.white;
    
    [SerializeField, Input("Background")]
    public Color background;


    public override Shader ReplacementShader => BitmaskShader;

    protected override void Process()
    {
        using (new CameraContext(Context.Renderer.Camera, new CameraContext.CameraProps
        {
            backgroundColor = background,
            clearFlags = CameraClearFlags.Color,
            allowMSAA = false
        }))
        {
            Shader.SetGlobalColor(ReplacementColorId, color);
            base.Process();
        }
    }
}
