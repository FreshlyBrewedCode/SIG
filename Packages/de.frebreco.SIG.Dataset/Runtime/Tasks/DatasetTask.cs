using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DatasetTask<TDataset, TItem, TAnnotation, TContext> : AsyncBatchProcessingTask<SIGGraph, TContext> 
    where TContext : BatchProcessingContext
    where TDataset : SIGDataset<TItem, TAnnotation>
    where TItem : IDatasetItem<TAnnotation>
    where TAnnotation : IAnnotation
{
    [SerializeField] protected TDataset dataset;
    [SerializeField] protected int itemCount;
    [SerializeField] protected int batchSize;
    
    public override int Count => itemCount;
    public override int BatchSize => batchSize;
    public TDataset Dataset => dataset;
    
    protected override IEnumerator AsyncRun(SIGGraphProcessor processor, TContext context)
    {
        dataset.Clear();
        yield return base.AsyncRun(processor, context);
        dataset.SaveDatasetAnnotations();
    }
}

public class DatasetProcessingContext<TDataset, TItem, TAnnotation> : BatchProcessingContext
    where TAnnotation : IAnnotation 
    where TItem : IDatasetItem<TAnnotation>
    where TDataset : SIGDataset<TItem, TAnnotation>
{
    protected TDataset dataset;
    public virtual TDataset Dataset => dataset;

    protected int datasetItemId;
    public int DatasetItemId => datasetItemId;

    public DatasetProcessingContext(TDataset dataset, int count, int batchSize) : base(count, batchSize)
    {
        this.dataset = dataset;
        this.datasetItemId = dataset.GetNextId();
    }

    public override void MoveNext()
    {
        base.MoveNext();
        datasetItemId = dataset.GetNextId();
    }
}

public class ImageDatasetProcessingContext<TDataset, TItem, TAnnotation> : DatasetProcessingContext<TDataset, TItem, TAnnotation>
    where TAnnotation : IAnnotation 
    where TItem : ImageDatasetItem<TAnnotation>
    where TDataset : ImageDataset<TItem, TAnnotation>
{
    protected ImageDatasetProcessingContext(TDataset dataset, int count, int batchSize) : base(dataset, count, batchSize) { }
}
