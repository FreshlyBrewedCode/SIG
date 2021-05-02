using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public abstract class RandomNode : SIGNode
{
    public const string RANDOM = "Random";
    public override Color color => new Color(0.21f, 0.71f, 0.65f);
}

public abstract class RandomUVectorNode : RandomNode
{
    [Output("Out")] public UVector output;

    protected abstract UVector GetValue(SIGProcessingContext context);

    protected override void Process(SIGProcessingContext context)
    {
        output = GetValue(context);
    }
}

[System.Serializable, NodeMenuItem(RANDOM + "/Random Value")]
public class RandomValueNode : RandomUVectorNode
{
    protected override UVector GetValue(SIGProcessingContext context)
    {
        return (UVector)Random.RandomValue;
    }
}

[System.Serializable, NodeMenuItem(RANDOM + "/Random Range")]
public class RandomRangeNode : RandomUVectorNode
{
    [SerializeField, Input("Start")] public float start;
    [SerializeField, Input("End")] public float end;
    
    protected override UVector GetValue(SIGProcessingContext context)
    {
        return (UVector)Random.Range(start, end);
    }
}
