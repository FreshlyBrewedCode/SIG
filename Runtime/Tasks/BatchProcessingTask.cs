using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

public abstract class BatchProcessingTask<TGraph, TContext> : SIGTask<TGraph, TContext>
    where TGraph : SIGGraph
    where TContext : BatchProcessingContext
{
    public abstract int Count { get; }
    public abstract int BatchSize { get; }

    protected override void Run(SIGGraphProcessor processor, TContext context)
    {
        for (int i = 0; i < context.Count; i += context.BatchSize)
        {
            RunBatch(processor, context);
        }
        Complete();
    }

    protected virtual void RunBatch(SIGGraphProcessor processor, TContext context)
    {
        for (int i = 0; i < context.BatchSize && context.Index < context.Count; i++)
        {
            RunOnce(processor, context);
        }
    }

    protected override void OnPreProcess(TContext context)
    {
        base.OnPreProcess(context);
        context.Reset();
    }

    protected virtual void RunOnce(SIGGraphProcessor processor, TContext context)
    {
        OnPreProcess(context);
        processor.Run(context);
        OnPostProcess(context);
        context.MoveNext();
    }
}

public abstract class AsyncBatchProcessingTask<TGraph, TContext> : BatchProcessingTask<TGraph, TContext>
    where TGraph : SIGGraph
    where TContext : BatchProcessingContext
{
    protected int progressId;
    protected bool shouldCancel = false;
    
    protected override void Run(SIGGraphProcessor processor, TContext context)
    {
#if UNITY_EDITOR
        EditorCoroutineUtility.StartCoroutine(AsyncRun(processor, context), this);
#endif
    }

    protected virtual IEnumerator AsyncRun(SIGGraphProcessor processor, TContext context)
    {
        progressId = Progress.Start(this.name, null);
        Progress.SetPriority(progressId, Progress.Priority.High);
        Progress.Report(progressId, 1, context.Count);
        
        Progress.RegisterCancelCallback(progressId, OnCancel);
        shouldCancel = false;

        yield return null;
        
        for (int i = 0; i < context.Count; i += context.BatchSize)
        {
            if (shouldCancel) break;

            RunBatch(processor, context);
            yield return null;
        }
        
        Progress.Finish(progressId, shouldCancel ? Progress.Status.Canceled : Progress.Status.Succeeded);
        Progress.UnregisterCancelCallback(progressId);
        Complete(!shouldCancel);

        bool OnCancel()
        {
            shouldCancel = true;
            return true;
        }
    }

    protected override void RunOnce(SIGGraphProcessor processor, TContext context)
    {
        base.RunOnce(processor, context);
        Progress.Report(progressId, context.Index + 2, context.Count);
    }
}


public class BatchProcessingContext : SIGProcessingContext
{
    protected int index;
    public int Index => index;
    
    protected int count;
    public int Count => count;

    protected int batchSize;
    public int BatchSize => batchSize;

    public int BatchIndex => index % batchSize;

    public BatchProcessingContext(int count, int batchSize)
    {
        this.index = 0;
        this.count = count;
        this.batchSize = batchSize;
        
        if(count < 0) throw new Exception("Count can not be less than zero.");
        if(batchSize < 1) throw new Exception("Batch size must be at least one.");
    }
    
    public virtual void MoveNext() => index++;
}
