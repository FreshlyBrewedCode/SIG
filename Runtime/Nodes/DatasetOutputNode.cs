using System;
using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public abstract class DatasetNode<TDataset, TItem, TAnnotation> : SIGNode
    where TAnnotation : IAnnotation 
    where TItem : IDatasetItem<TAnnotation>
    where TDataset : SIGDataset<TItem, TAnnotation>
{
    public const string DATASET = "Dataset";

    protected virtual new DatasetProcessingContext<TDataset, TItem, TAnnotation> Context
    {
        get
        {
            if (base.Context is DatasetProcessingContext<TDataset, TItem, TAnnotation> ctx) return ctx;
            throw new Exception("Dataset nodes can only be used in a dataset processing context.");
        }
    }

    protected virtual TDataset Dataset => Context.Dataset;
    public virtual int DatasetItemId => Context.DatasetItemId;
}

[System.Serializable]
public abstract class DatasetOutputNode<TDataset, TItem, TAnnotation> : DatasetNode<TDataset, TItem, TAnnotation>
    where TAnnotation : IAnnotation 
    where TItem : IDatasetItem<TAnnotation>
    where TDataset : SIGDataset<TItem, TAnnotation>
{
    public abstract TItem Item { get; }
    public abstract TAnnotation Annotation { get; }

    protected override void Process(SIGProcessingContext context)
    {
        if(!Assert(Dataset != null, "No dataset provided") ||
           !Assert(Item != null, "No dataset item provided") ||
           !Assert(Annotation != null, "No annotation provided")) return;

        var id = Dataset.AddItem(Item);
        Dataset.AddAnnotation(id, Annotation);
    }
}

[System.Serializable]
public abstract class ImageDatasetOutputNode<TDataset, TItem, TAnnotation> : DatasetOutputNode<TDataset, TItem, TAnnotation>
    where TAnnotation : IAnnotation 
    where TItem : ImageDatasetItem<TAnnotation>
    where TDataset : ImageDataset<TItem, TAnnotation>
{
    [SerializeField, Input("Image")] public RenderTexture image;
    [Input("Annotation")] public TAnnotation annotation;
    
    public override TAnnotation Annotation => annotation;
}
