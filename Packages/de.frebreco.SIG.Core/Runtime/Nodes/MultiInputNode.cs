using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;

[System.Serializable]
public abstract class MultiInputNode<T> : SIGNode
{
    [Input("In", allowMultiple = true)] protected IEnumerable<T> input;
    
    [CustomPortBehavior(nameof(input))]
    IEnumerable<PortData> GetPortsForInputs(List<SerializableEdge> edges)
    {
        yield return new PortData{ displayName = "In ", displayType = typeof(T), acceptMultipleEdges = true};
    }
    
    [CustomPortInput(nameof(input),typeof(Object), allowCast = true)]
    public void GetInputs(List<SerializableEdge> edges)
    {
        input = edges.Select(e => (T)e.passThroughBuffer);
    }
}
