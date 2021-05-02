using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(menuName = MENU_PATH + "/Coco Dataset Task")]
public class CocoDatasetTask : DatasetTask<CocoDataset, CocoDatasetItem, CocoAnnotation, CocoProcessingContext>
{
    protected List<Dictionary<string, long>> profilerResults;
    protected Stopwatch totalStopwatch;

    protected override void Run(SIGGraphProcessor processor, CocoProcessingContext context)
    {
        totalStopwatch = Stopwatch.StartNew();
        base.Run(processor, context);
    }

    protected override CocoProcessingContext CreateContext()
    {
        var context = new CocoProcessingContext(Dataset, Count, BatchSize);
        context.profiler = new StopWatchProfiler();
        profilerResults = new List<Dictionary<string, long>>();
        return context;
    }

    protected override void RunOnce(SIGGraphProcessor processor, CocoProcessingContext context)
    {
        context.Profiler.Reset(); 
        base.RunOnce(processor, context);
        if(context.Profiler is StopWatchProfiler profiler) profilerResults.Add(profiler.FlatResults);
    }

    protected override void Complete(bool success = true)
    {
        base.Complete(success);
        Dataset.SaveDatasetFile(JsonConverter.ToJson(profilerResults, true), "profiler_results.json");
        totalStopwatch.Stop();
        Debug.Log("Total time: " + totalStopwatch.ElapsedMilliseconds);
    }
}

public class CocoProcessingContext : ImageDatasetProcessingContext<CocoDataset, CocoDatasetItem, CocoAnnotation>
{
    public CocoProcessingContext(CocoDataset dataset, int count, int batchSize) : base(dataset, count, batchSize) {}
}
