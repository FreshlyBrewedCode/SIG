using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SIGComponentNode<T> : SIGNode where T : SIGComponent
{
    public const string COMPONENTS = "Components";

    protected T Target => context.processor.GetSIGComponent<T>(context);
    
    protected override void Process(SIGProcessingContext context)
    {
        base.Process(context);
    }
}
