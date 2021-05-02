using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;

public abstract class MathNode : SIGNode
{
    public const string MATH = "Math";
        
    [Output("Out")]
    public UVector output;
    
    public abstract UVector Result { get; }

    protected override void Process(SIGProcessingContext context)
    {
        output = Result;
    }
}

public abstract class MultiVectorNode : MathNode
{
    [Input("In", allowMultiple = true)] protected IEnumerable<UVector> input;
    
    [CustomPortBehavior(nameof(input))]
    IEnumerable<PortData> GetPortsForInputs(List<SerializableEdge> edges)
    {
        yield return new PortData{ displayName = "In ", displayType = typeof(UVector), acceptMultipleEdges = true};
    }
    
    [CustomPortInput(nameof(input),typeof(UVector), allowCast = true)]
    public void GetInputs(List<SerializableEdge> edges)
    {
        input = edges.Select(e => (UVector)e.passThroughBuffer);
    }
}

[System.Serializable]
public abstract class DualInputMathNode : MathNode
{
    [Input("A"), SerializeField, ShowAsDrawer]
    public UVector a;
    
    [Input("B"), SerializeField, ShowAsDrawer]
    public UVector b;
}

[System.Serializable, NodeMenuItem(MathNode.MATH + "/Combine")]
public class CombineNode : MathNode
{
    [Input("x"), SerializeField, ShowAsDrawer] public float x;
    [Input("y"), SerializeField, ShowAsDrawer] public float y;
    [Input("z"), SerializeField, ShowAsDrawer] public float z;
    [Input("w"), SerializeField, ShowAsDrawer] public float w;
    
    public override UVector Result => new UVector(x, y, z, w);
}

[System.Serializable, NodeMenuItem(MathNode.MATH + "/Split")]
public class SplitNode : SIGNode
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public UVector input;
    
    [Output("x")] public float x;
    [Output("y")] public float y;
    [Output("z")] public float z;
    [Output("w")] public float w;

    protected override void Process(SIGProcessingContext context)
    {
        x = input.x;
        y = input.y;
        z = input.z;
        w = input.w;
    }
}

[System.Serializable, NodeMenuItem(MATH + "/Add")]
public class AddNode : DualInputMathNode
{
    public override UVector Result => a + b;
}

[System.Serializable, NodeMenuItem(MATH + "/Subtract")]
public class SubtractNode : DualInputMathNode
{
    public override UVector Result => a - b;
}

[System.Serializable, NodeMenuItem(MATH + "/Multiply")]
public class MultiplyNode : DualInputMathNode
{
    public override UVector Result => a * b;
}

[System.Serializable, NodeMenuItem(MATH + "/Divide")]
public class DivideNode : DualInputMathNode
{
    public override UVector Result => a / b;
}

[System.Serializable, NodeMenuItem(MATH + "/Pow")]
public class PowNode : MathNode
{
    [Input("A"), SerializeField, ShowAsDrawer]
    public UVector a;
    
    [Input("Power"), SerializeField, ShowAsDrawer]
    public float p;
    
    public override UVector Result => a.ScalarOp(v => Mathf.Pow(v, p));
}
