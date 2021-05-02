using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

[System.Serializable]
public abstract class TransformOperationNode : MultiInputOperationNode
{
    public const string TRANSFORM = SCENE_OPERATIONS + "/Transform";

    public override Color color => new Color(0.85f, 0.21f, 0.21f);
    
    public enum Space
    {
        World, Local
    }

    [SerializeField, HideInInspector]
    public Space space = Space.World;

    protected bool isLocal => space == Space.Local;

    protected Vector3 Position(Transform t) => isLocal ? t.localPosition : t.position;
    protected void SetPosition(Transform t, Vector3 pos)
    {
        if (isLocal) t.localPosition = pos;
        else t.position = pos;
    }
    
    protected Quaternion Rotation(Transform t) => isLocal ? t.localRotation : t.rotation;
    protected void SetRotation(Transform t, Quaternion rot)
    {
        if (isLocal) t.localRotation = rot;
        else t.rotation = rot;
    }
    
    protected Vector3 Scale(Transform t) => isLocal ? t.localScale : t.lossyScale;

}

#if UNITY_EDITOR
[NodeCustomEditor(typeof(TransformOperationNode))]
public class TransformOperationNodeView : BaseNodeView
{
    protected EnumField spaceField;
    protected TransformOperationNode Target => nodeTarget as TransformOperationNode;
    
    public override void Enable()
    {
        base.Enable();
        
        spaceField = new EnumField(Target.space);
        spaceField.RegisterValueChangedCallback(evt => Target.space = (TransformOperationNode.Space) evt.newValue);
        controlsContainer.Add(spaceField);
    }
}
#endif

[System.Serializable, NodeMenuItem(TRANSFORM + "/Set Transform")]
public class SetTransformNode : TransformOperationNode
{
    [Input("P"), SerializeField, ShowAsDrawer]
    public Vector3 positionIn;

    [Input("R"), SerializeField, ShowAsDrawer]
    public Vector3 rotationIn;

    [Input("S"), SerializeField, ShowAsDrawer]
    public Vector3 scaleIn;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        foreach (var gameObject in input)
        {
            var transform = gameObject.transform;
            SetPosition(transform, positionIn);
            SetRotation(transform, Quaternion.Euler(rotationIn));
            transform.localScale = scaleIn;
        }

        return input;
    }
}

[System.Serializable, NodeMenuItem(TRANSFORM + "/Get Transform")]
public class GetTransformNode : TransformOperationNode
{
    [Output("P")]
    public Vector3 positionOut;
    
    [Output("R")]
    public Quaternion rotationOut;
    
    [Output("S")]
    public Vector3 scaleOut;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        if(!Assert(input.Count >= 1, "Scene needs at least one element")) return RenderScene.Empty;
        var transform = input[0].transform;

        positionOut = Position(transform);
        rotationOut = Rotation(transform);
        scaleOut = Scale(transform);

        return input;
    }
}

[System.Serializable, NodeMenuItem(TRANSFORM + "/Offset")]
public class OffsetNode : TransformOperationNode
{
    [SerializeField, ShowAsDrawer, Input("P")]
    public Vector3 offsetPos;
    
    [SerializeField, ShowAsDrawer, Input("R")]
    public Vector3 offsetRot;
    
    [SerializeField, ShowAsDrawer, Input("S")]
    public Vector3 offsetScale;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        int index = 0;
        foreach (var gameObject in input)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < index; i++)
            {
                SetPosition(transform, Position(transform) + offsetPos);
                SetRotation(transform, Quaternion.Euler(Rotation(transform).eulerAngles + offsetRot));
                transform.localScale += offsetScale;
            }
            index++;
        }

        return input;
    }
}

[System.Serializable, NodeMenuItem(TRANSFORM + "/Offset Each")]
public class OffsetEachNode : TransformOperationNode
{
    [SerializeField, ShowAsDrawer, Input("P")]
    public Vector3 offsetPos;
    
    [SerializeField, ShowAsDrawer, Input("R")]
    public Vector3 offsetRot;
    
    [SerializeField, ShowAsDrawer, Input("S")]
    public Vector3 offsetScale;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        foreach (var gameObject in input)
        {
            var transform = gameObject.transform;
            SetPosition(transform, Position(transform) + offsetPos);
            SetRotation(transform, Quaternion.Euler(Rotation(transform).eulerAngles + offsetRot));
            transform.localScale += offsetScale;
        }

        return input;
    }
}

[System.Serializable, NodeMenuItem(TRANSFORM + "/Look At")]
public class LookAtNode : TransformOperationNode
{
    [Input("Target")] 
    public RenderScene target = RenderScene.Empty;

    [SerializeField, Input("Zoom")]
    public float zoom = 0f;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        if (!Assert(!target.IsEmpty, "Target can not be empty")) return input;

        var pos = target.Aggregate(Vector3.zero, (p, g) => p + g.transform.position) / target.Count;

        foreach (var gameObject in input)
        {
            var transform = gameObject.transform;
            transform.LookAt(pos, Vector3.up);
            transform.position += transform.forward * zoom;
        }

        return input;
    }
}
