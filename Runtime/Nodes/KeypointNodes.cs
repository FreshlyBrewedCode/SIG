using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public abstract class KeypointNode : SIGNode
{
    public const string KEYPOINTS = "Keypoints";
}

[System.Serializable, NodeMenuItem(KEYPOINTS + "/Image Keypoints")]
public class ImageKeypointsNode : KeypointNode
{
    [Input("Scene")] public RenderScene scene;
    [Input("Camera")] public RenderScene camera;
    [Input("Image")] public UTexture image;
    [SerializeField, Input("Culling")] public bool culling = true;
    
    [Output("Out")] public List<ImageKeypoint> imageKeypoints;

    protected override void Process(SIGProcessingContext context)
    {
        if(!Assert(IsConnected(nameof(camera)), "A camera is required")) return;
        if(!Assert(IsConnected(nameof(image)), "An image is required")) return;
        
        var cam = camera.GetComponent<Camera>();
        if(!Assert(cam != null, "A camera is required")) return;
        
        
        var keypoints = scene.GetComponents<ISIGKeypoint>(true);
        imageKeypoints = new List<ImageKeypoint>();
        
        using (new CameraContext(cam, new CameraContext.CameraProps
        {
            aspect = image.Width / image.Height
        }))
        {
            foreach (var keypoint in keypoints)
            {
                var point = cam.WorldToViewportPoint(keypoint.Position);

                if (culling)
                {
                    if(point.x < 0 || point.x > 1 || point.y < 0 || point.y > 1) continue;
                }
                
                imageKeypoints.Add(new ImageKeypoint<Keypoint>(
                    Mathf.RoundToInt(point.x * image.Width),
                    Mathf.RoundToInt(image.Height - point.y * image.Height),
                    keypoint.Keypoint
                ));
            }
        }
    }
}
