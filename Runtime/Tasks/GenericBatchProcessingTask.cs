using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = MENU_PATH + "/Generic Batch Processing Task")]
public class GenericBatchProcessingTask : AsyncBatchProcessingTask<SIGGraph, BatchProcessingContext>
{
    [SerializeField, Min(0)] protected int count = 1;
    [SerializeField, Min(1)] protected int batchSize = 1;
    
    protected override BatchProcessingContext CreateContext() => new BatchProcessingContext(Count, BatchSize);

    public override int Count => count;
    public override int BatchSize => batchSize;
}
