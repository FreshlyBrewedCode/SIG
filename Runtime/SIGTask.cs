using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SIGTask : ScriptableObject
{
    public const string MENU_PATH = "SIG/Tasks";

    public delegate void OnTaskCompletedCallback<T>(T task, bool success) where T : SIGTask;
    public event OnTaskCompletedCallback<SIGTask> onCompleted;

    public abstract SIGProcessingContext Context { get; }
    public abstract SIGGraph Graph { get; }

    public virtual void OnPreProcess(SIGProcessingContext context) {}

    public virtual void OnPostProcess(SIGProcessingContext context)
    {
        if(context.HasError) Debug.LogError("Error while processing graph. Open the graph for more details.");
    }

    public virtual void Run(SIGGraphProcessor processor, SIGProcessingContext context)
    {
        OnPreProcess(context);
        processor.Run(context);
        OnPostProcess(context);
        Complete();
    }

    protected virtual void Complete(bool success = true)
    {
        onCompleted?.Invoke(this, success);
    }
}

public abstract class SIGTask<TGraph, TContext> : SIGTask 
    where TGraph : SIGGraph
    where TContext : SIGProcessingContext
{
    [SerializeField] protected TGraph graph;
    public override SIGGraph Graph => graph;

    protected TContext context;
    public override SIGProcessingContext Context
    {
        get
        {
            if (context == null) context = CreateContext();
            return context;
        }
    }
    
    protected abstract TContext CreateContext();

    public override void Run(SIGGraphProcessor processor, SIGProcessingContext context)
    {
        if(!(context is TContext ctx)) throw new Exception($"Task can only run in a context of type {typeof(TContext).Name}");
        Run(processor, ctx);
        this.context = null;
    }
    
    public override void OnPreProcess(SIGProcessingContext context)
    {
        OnPreProcess(context as TContext);
    }
    
    public override void OnPostProcess(SIGProcessingContext context)
    {
        OnPostProcess(context as TContext);
    }
    
    protected virtual void Run(SIGGraphProcessor processor, TContext context)
    {
        base.Run(processor, context);
    }
    
    protected virtual void OnPreProcess(TContext context) {}
    protected virtual void OnPostProcess(TContext context) {}
    
}

