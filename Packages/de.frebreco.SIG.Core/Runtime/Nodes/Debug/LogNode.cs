using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable, NodeMenuItem("Debug/Log", typeof(SIGGraph))]
public class LogNode : SIGNode
{
    [Input("Message")] public object message;

    protected override void Process(SIGProcessingContext context)
    {
        AddMessage(message.ToString(), NodeMessageType.Info);
        Debug.Log(message);
    }
}

[System.Serializable, NodeMenuItem("Debug/Log JSON", typeof(SIGGraph))]
public class LogJsonNode : SIGNode
{
    [Input("Message")] public object message;

    protected bool ShouldIndent
    {
        get
        {
            if (message is IList) return false;
            return true;
        }
    }
    
    protected override void Process(SIGProcessingContext context)
    {
        string msg = JsonConvert.SerializeObject(message, ShouldIndent ? Formatting.Indented : Formatting.None);
        AddMessage(msg, NodeMessageType.Info);
        Debug.Log(msg);
    }
}
