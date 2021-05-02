using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[NodeCustomEditor(typeof(ImageKeypointsNode))]
public class ImageKeypointsNodeView : BaseNodeView
{
    private ImageKeypointsNode Target => nodeTarget as ImageKeypointsNode;
    private Image textureImage;
    private IMGUIContainer previewContainer;

    public override void Enable()
    {
        base.Enable();
        previewContainer = new IMGUIContainer(OnDrawPreview);

        Target.onProcessed += TargetOnonProcessed;

        controlsContainer.Add(previewContainer);
    }

    private void OnDrawPreview()
    {
        if (Target.image != null)
        {
            var img = Target.image.UnityTexture;
            EditorGUI.DrawPreviewTexture(new Rect(0, 0, img.width, img.height), img);

            GUI.Label(new Rect(2, 2, img.width, 20), $"Keypoints: {Target.imageKeypoints.Count}");

            using (new Handles.DrawingScope(Color.white))
            {
                foreach (var keypoint in Target.imageKeypoints)
                {
                    Handles.color = Color.HSVToRGB(keypoint.Keypoint.Id / (float) Target.imageKeypoints.Count, 1f, 1f);
                    Handles.DrawSolidDisc(new Vector2(keypoint.X, keypoint.Y), Vector3.forward, 3f);
                }
            }
        }
    }


    private void TargetOnonProcessed()
    {
        if (Target.image != null)
        {
            previewContainer.style.width = Target.image.Width;
            previewContainer.style.height = Target.image.Height;
        }
    }
}