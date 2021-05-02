using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public abstract class AnnotationNode<TAnnotation, TDataset, TItem> : DatasetNode<TDataset, TItem, TAnnotation>
    where TAnnotation : IAnnotation 
    where TItem : IDatasetItem<TAnnotation>
    where TDataset : SIGDataset<TItem, TAnnotation>
{
    public const string ANNOTATION = "Annotation";
    
    public abstract TAnnotation Annotation { get; }

    [Output("Out")] public TAnnotation annotation;

    protected override void Process(SIGProcessingContext context)
    {
        base.Process(context);
        annotation = Annotation;
    }
}
