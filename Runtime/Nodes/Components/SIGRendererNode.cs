using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[System.Serializable, NodeMenuItem(COMPONENTS + "/Renderer")]
public class SIGRendererNode : SIGComponentNode<SIGRenderer>
{
    [Input("Scene")] public RenderScene scene;
    
    [Input("Camera")] public RenderScene camera = RenderScene.Empty;
    
    [SerializeField, ShowAsDrawer, Input("Resolution")]
    public Vector2 resolution;
    
    [SerializeField, ShowAsDrawer, Input("Target")]
    public RenderTexture targetTexture;
    
    [Output("Image")] public RenderTexture image;
    
    protected override void Process(SIGProcessingContext context)
    {
        base.Process(context);

        if (resolution.x < 1 || resolution.y < 1 || resolution.x % 1f != 0f || resolution.y % 1f != 0f)
        {
            AddMessage("Invalid resolution", NodeMessageType.Error);
            context.Error();
            return;
        }

        if (!camera.IsEmpty)
        {
            var cam = camera.GetComponent<Camera>();
            Assert(cam != null, "Camera input does not contain a camera component");
            Target.SetCamera(cam);
        }

        image = Render();
    }

    protected virtual RenderTexture Render()
    {
        if(targetTexture == null)
            return Target.Render(scene, (int)resolution.x, (int)resolution.y);
        else
            return Target.Render(scene, targetTexture);
    }
}
