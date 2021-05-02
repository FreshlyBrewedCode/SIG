using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SIGProfiler
{
    public virtual void OnBeginGraphProcessing()
    {
    }

    public virtual void OnFinishGraphProcessing()
    {
    }

    public virtual void OnBeginNodeProcessing(SIGNode node)
    {
    }

    public virtual void OnFinishNodeProcessing(SIGNode node)
    {
    }

    public virtual void Reset() {}
}

public class StopWatchProfiler : SIGProfiler
{
    protected Stopwatch graphStopwatch;
    protected Stopwatch nodeStopwatch;
    protected Dictionary<string, List<long>> results;
    public Dictionary<string, List<long>> Results => results;

    public Dictionary<string, long> FlatResults => results.Aggregate(new Dictionary<string, long>(),
        (output, current) =>
        {
            int i = 0;
            foreach (long time in current.Value)
            {
                var name = i > 0 ? $"{current.Key} {i}" : current.Key;
                output.Add(name, time);
                i++;
            }

            return output;
        });
    
    public string JsonResults
    {
        get => JsonConvert.SerializeObject(FlatResults, Formatting.Indented);
    }

    public StopWatchProfiler()
    {
        results = new Dictionary<string, List<long>>();
        Debug.Log($"High frequency: {Stopwatch.IsHighResolution}, Frequency: {Stopwatch.Frequency}");
    }

    public override void OnBeginGraphProcessing()
    {
        graphStopwatch = Stopwatch.StartNew();
    }

    public override void OnFinishGraphProcessing()
    {
        graphStopwatch.Stop();
        AddResult("Total", graphStopwatch.ElapsedTicks);
    }

    public override void OnBeginNodeProcessing(SIGNode node)
    {
        nodeStopwatch = Stopwatch.StartNew();
    }

    public override void OnFinishNodeProcessing(SIGNode node)
    {
        nodeStopwatch.Stop();
        AddResult(node.name, nodeStopwatch.ElapsedTicks);
    }

    protected virtual void AddResult(string name, long time)
    {
        if (!results.ContainsKey(name))
        {
            results.Add(name, new List<long>());
        }

        results[name].Add(time);
    }

    public override void Reset()
    {
        results.Clear();
    }
}