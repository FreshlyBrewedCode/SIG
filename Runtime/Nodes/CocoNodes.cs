using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;

[System.Serializable, NodeMenuItem(DATASET + "/Coco Dataset")]
public class CocoDatasetOutputNode : ImageDatasetOutputNode<CocoDataset, CocoDatasetItem, CocoAnnotation>
{
    public new CocoProcessingContext Context
    {
        get
        {
            if (base.Context is CocoProcessingContext cocoContext) return cocoContext;
            throw new Exception("Coco dataset node can only be used in a Coco processing context.");
        }
    }
    
    public override CocoDatasetItem Item => new CocoDatasetItem(image);
}

[System.Serializable, NodeMenuItem(ANNOTATION + "/Coco Annotation")]
public class CocoAnnotationNode : AnnotationNode<CocoAnnotation, CocoDataset, CocoDatasetItem>
{
    private const string PYTHON_NODE = "CocoAnnotationNode";
    
    [Input("Mask")] public UTexture mask;
    [NonSerialized, Input("Keypoints")] public List<ImageKeypoint> keypoints;
    
    public override CocoAnnotation Annotation => new CocoAnnotation(Dataset);

    protected override void Process(SIGProcessingContext context)
    {
        if(!Assert(mask != null, "A bit mask is required")) return;
        if(keypoints == null) keypoints = new List<ImageKeypoint>();
        
#if UNITY_EDITOR
        var rawTexture = mask.ConvertTo(UTextureFormat.PNG).RawTexture;
        
        var pythonOutput = SIGPython.Services.ProcessNode(PYTHON_NODE, new Dictionary<string, object>
        {
            {"mask_texture", rawTexture.ToList()},
            {"id", DatasetItemId},
            {"category_id", Dataset.Category.id},
            {"image_id", DatasetItemId},
            {"is_crowd", false},
            {"keypoints", JsonConverter.ToJson(CocoAnnotation.GetCocoKeypoints(keypoints))}
        }.ToPyDict());

        annotation = new CocoAnnotation(Dataset)
        {
            imageAnnotation = JsonConverter.FromJson<CocoJsonAnnotationData.Annotation>(pythonOutput.GetOutput<string>("json"))
        };
#else
        Error("SIGPython is not available at runtime");
        return;
#endif
    }
}

[System.Serializable, NodeMenuItem(KEYPOINTS + "/Coco Keypoints")]
public class CocoKeypointNode : ImageKeypointsNode
{
    [NonSerialized, Output("Coco Out")] public List<int> cocoKeypoints;

    protected override void Process(SIGProcessingContext context)
    {
        base.Process(context);
        cocoKeypoints = CocoAnnotation.GetCocoKeypoints(imageKeypoints);
    }
}



