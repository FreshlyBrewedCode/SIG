using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public abstract class UnityComponentNode<T> : MultiInputOperationNode where T : Component
{
    public const string UNITY_COMPONENTS = "Unity Components";

    public override Color color => new Color(0.2f, 0.71f, 0.47f);

    protected T component;

    protected override void Process(SIGProcessingContext context)
    {
        foreach (var gameObject in input)
        {
            component = gameObject.GetComponent<T>();
            if(component != null) break;
        }

        if (component == null)
        {
            if (input.IsEmpty)
            {
                var comp = Target.CreateGameObject(typeof(T).Name);
                input = new RenderObject(comp);
            }
            
            component = input[0].AddComponent<T>();
        }
        
        base.Process(context);
    }

    protected override RenderScene Apply(SIGProcessingContext context)
    {
        return new RenderObject(component.gameObject);
    }
}

[System.Serializable, NodeMenuItem(UNITY_COMPONENTS + "/Camera")]
public class CameraComponentNode : UnityComponentNode<Camera>
{
    [Input("Focal Length"), SerializeField, ShowAsDrawer]
    public float focalLength = 40;
    
    [Input("Clear Color"), SerializeField, ShowAsDrawer]
    public Color clearColor = Color.clear;
    
    [SerializeField] public CameraClearFlags clearFlags = CameraClearFlags.Skybox;

    protected override RenderScene Apply(SIGProcessingContext context)
    {
        component.focalLength = focalLength;
        component.usePhysicalProperties = true;
        component.clearFlags = clearFlags;
        component.backgroundColor = clearColor;
        
        return base.Apply(context);
    }
}

[System.Serializable, NodeMenuItem(UNITY_COMPONENTS + "/Poser")]
public class PoserComponentNode : UnityComponentNode<Poser>
{
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        component.RandomPose(context.Random);
        
        return base.Apply(context);
    }
}
