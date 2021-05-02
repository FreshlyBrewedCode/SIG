using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using Unity.Jobs;
using UnityEngine;

public class SIGGraphProcessor : BaseGraphProcessor
{
    protected List<BaseNode> processList;
    protected SIGProcessingContext context;
    
    /// <summary>
    /// Manage graph scheduling and processing
    /// </summary>
    /// <param name="graph">Graph to be processed</param>
    public SIGGraphProcessor(BaseGraph graph) : base(graph) {}

    public override void UpdateComputeOrder()
    {
        processList = graph.nodes.OrderBy(n => n.computeOrder).ToList();
    }

    /// <summary>
    /// Schedule the graph into the job system
    /// </summary>
    public override void Run()
    {
        int count = processList.Count;

        context.Profiler.OnBeginGraphProcessing();
        for (int i = 0; i < count; i++)
        {
            if (processList[i] is SIGNode node) node.Context = this.context;
            processList[i].OnProcess();
        }
        context.Profiler.OnFinishGraphProcessing();
        
        JobHandle.ScheduleBatchedJobs();
    }

    public virtual void Run(SIGProcessingContext context)
    {
        this.context = context;
        Run();
    }
}
