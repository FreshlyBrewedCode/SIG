using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using System.Reflection;

[System.Serializable]
public abstract class SIGNode : BaseNode
{
    protected SIGProcessingContext context;

    public override string name
    {
        get
        {
            var menuAttr = GetType().GetCustomAttribute(typeof(NodeMenuItemAttribute), false) as NodeMenuItemAttribute;
            if (menuAttr != null)
            {
                var start = menuAttr.menuTitle.LastIndexOf('/') + 1;
                var length = menuAttr.menuTitle.Length - start;
                return menuAttr.menuTitle.Substring(start, length);
            }

            return base.name;
        }
    }

    public SIGProcessingContext Context
    {
        get => context;
        set => context = value;
    }

    public SIGRandom Random => context.Random;
    
    protected override void Process()
    {
        if(context.HasError) return;
        context.Profiler.OnBeginNodeProcessing(this);
        this.Process(context);
        context.Profiler.OnFinishNodeProcessing(this);
    }

    protected virtual void Process(SIGProcessingContext context)
    {
        
    }

    protected bool AssertInput(object input, string message)
    {
        return Assert(input != null, message);
    }
    
    protected bool Assert(bool assertion, string message)
    {
        if (!assertion)
        {
            Error(message);
        }

        return assertion;
    }

    public void Error(string message)
    {
        AddMessage(message, NodeMessageType.Error);
        context.Error();
    }
    
    protected bool IsConnected(string fieldName) => GetPort(fieldName, "").GetEdges().Count > 0;
}

public class SIGProcessingContext
{
    public SIGProcessor processor;
    public SIGProfiler profiler;
    
    protected bool error = false;
    public bool HasError => error;

    protected SIGRandom random;
    public SIGRandom Random => random;

    public SIGProfiler Profiler => profiler;
    
    public event System.Action onReset;
    
    public SIGRenderer Renderer => processor.GetSIGComponent<SIGRenderer>(this);

    public SIGProcessingContext()
    {
        random = new UnityRandom();
        profiler = new SIGProfiler();
    }
    
    public void Error()
    {
        error = true;
    }

    public void Reset()
    {
        onReset?.Invoke();
    } 
}
