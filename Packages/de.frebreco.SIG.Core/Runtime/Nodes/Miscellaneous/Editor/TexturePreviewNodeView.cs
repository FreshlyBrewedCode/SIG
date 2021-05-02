using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UIElements;

[NodeCustomEditor(typeof(TexturePreviewNode))]
public class TexturePreviewNodeView : BaseNodeView
{
    private TexturePreviewNode Target => nodeTarget as TexturePreviewNode;
    private Image textureImage;
    
    public override void Enable()
    {
        base.Enable();
        textureImage = new Image();
        textureImage.image = Target.texture;
        
        Target.onProcessed += TargetOnonProcessed;
        
        controlsContainer.Add(textureImage);
        textureImage.MarkDirtyRepaint();
    }

    
    private void TargetOnonProcessed()
    {
        textureImage.image = Target.texture;
        textureImage.MarkDirtyRepaint();
    }
}
