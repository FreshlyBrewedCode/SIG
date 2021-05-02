using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SIGRenderer : SIGComponent
{
    [SerializeField, LayerDropdown] protected int renderLayer;
    
    protected Camera camera;
    public Camera Camera => camera;
    protected RenderTexture tempTexture;

    protected Queue<int> sceneLayerStore;
    
    public override void Initialize(SIGProcessingContext context)
    {
        base.Initialize(context);
        
        SetCamera(GetOrCreateComponent<Camera>());
    }

    public override void Finalize()
    {
        if(tempTexture != null) RenderTexture.ReleaseTemporary(tempTexture);
    }
    
    public RenderTexture Render(int width, int height)
    {
        UseTemporaryTexture(width, height);
        return Render(tempTexture);
    }

    public RenderTexture Render(RenderScene scene, int width, int height)
    {
        UseTemporaryTexture(width, height);
        return Render(scene, tempTexture);
    }

    public RenderTexture Render(RenderScene scene, RenderTexture texture)
    {
        PrepareScene(scene);
        var tex = Render(texture);
        RestoreScene(scene);

        return tex;
    }

    public void PrepareScene(RenderScene scene)
    {
        if(sceneLayerStore == null) sceneLayerStore = new Queue<int>(scene.Count);

        foreach (var gameObject in scene)
        {
            PrepareGameObject(gameObject);
        }
    }

    protected void PrepareGameObject(GameObject gameObject)
    {
        sceneLayerStore.Enqueue(gameObject.layer);
        gameObject.layer = renderLayer;

        if (gameObject.transform.childCount > 0)
        {
            foreach (Transform child in gameObject.transform)
            {
                PrepareGameObject(child.gameObject);
            }
        }
    }

    public void RestoreScene(RenderScene scene)
    {
        foreach (var gameObject in scene)
        {
            RestoreGameObject(gameObject);
        }
    }
    
    protected void RestoreGameObject(GameObject gameObject)
    {
        gameObject.layer = sceneLayerStore.Dequeue();

        if (gameObject.transform.childCount > 0)
        {
            foreach (Transform child in gameObject.transform)
            {
                RestoreGameObject(child.gameObject);
            }
        }
    }
    
    public RenderTexture Render(RenderTexture texture)
    {
        camera.targetTexture = texture;
        camera.Render();
        camera.targetTexture = null;

        return texture;
    }

    public void SetCamera(Camera cam)
    {
        camera = cam;
        cam.enabled = false;
        cam.cullingMask = 1 << renderLayer;
    }

    public void SetReplacementShader(Shader replacement, string tag = "")
    {
        if(camera == null) return;
        camera.SetReplacementShader(replacement, tag);
    }
    
    public void ResetReplacementShader()
    {
        if(camera == null) return;
        camera.ResetReplacementShader();
    }
    
    protected void UseTemporaryTexture(int width, int height)
    {
        if (tempTexture == null || (tempTexture.width != width || tempTexture.height != height))
        {
            if(tempTexture != null) RenderTexture.ReleaseTemporary(tempTexture);
            tempTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.Default);
        }
    }
}

public class CameraContext : IDisposable
{
    protected Camera cam;
    protected CameraProps cachedCamProps;

    public struct CameraProps
    {
        public CameraClearFlags? clearFlags;
        public Color? backgroundColor;
        public bool? allowMSAA;
        public float? aspect;

        public static CameraProps FromCamera(Camera cam)
        {
            return new CameraProps
            {
                clearFlags = cam.clearFlags,
                backgroundColor = cam.backgroundColor,
                allowMSAA = cam.allowMSAA,
                aspect = cam.aspect,
            };
        }

        public void Apply(Camera cam)
        {
            cam.clearFlags = clearFlags ?? cam.clearFlags;
            cam.backgroundColor = backgroundColor ?? cam.backgroundColor;
            cam.allowMSAA = allowMSAA ?? cam.allowMSAA;
            if (aspect.HasValue) cam.aspect = aspect.Value;
        }
    }
    
    public CameraContext(Camera camera, CameraProps props)
    {
        cam = camera;
        
        cachedCamProps = CameraProps.FromCamera(cam);
        props.Apply(cam);
    }
    
    public void Dispose()
    {
        cachedCamProps.Apply(cam);
    }
}
