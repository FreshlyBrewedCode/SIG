using System;
using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class SIGProcessor : MonoBehaviour
{
    protected static SIGProcessor currentProcessor;
    public static SIGProcessor Processor => currentProcessor;
    
    [SerializeField] protected SIGGraph graph;
    [SerializeField] protected SIGTask task;
    
    protected SIGGraphProcessor graphProcessor;
    protected SIGGraphProcessor GraphProcessor
    {
        get
        {
            if (graphProcessor == null) InitializeGraphProcessor();
            return graphProcessor;
        }
    }

    protected Dictionary<System.Type, SIGComponent> components = new Dictionary<Type, SIGComponent>();
    
    private void OnEnable()
    {
        currentProcessor = this;
    }

    protected void InitializeGraphProcessor()
    {
        if (task != null) graph = task.Graph;
        
        Debug.Assert(graph != null, "Unable to initialize processor without graph.");
        graph.UpdateComputeOrder();
        graphProcessor = new SIGGraphProcessor(graph);
        graphProcessor.UpdateComputeOrder();
    }
    
    public void Process(bool initialize = true)
    {
        var context = PreProcess(initialize);

        if (task != null)
        {
            task.onCompleted += OnTaskCompleted;
            task.Run(GraphProcessor, context);
        }
        else
        {
            GraphProcessor.Run(context);
            if(context.HasError) Debug.LogError("Error while processing graph. Open the graph for more details.");
            
            PostProcess(context);
        }

        void OnTaskCompleted(SIGTask task, bool success)
        {
            PostProcess(context);
            task.onCompleted -= OnTaskCompleted;
        }
    }

    public void Process(SIGGraph graph)
    {
        this.graph = graph;
        var task = this.task;
        this.task = null;
        
        Process(true);

        this.task = task;
    }

    public void RunTask(SIGTask task)
    {
        this.task = task;
        Process(true);
    }
    
    public T GetSIGComponent<T>(SIGProcessingContext context) where T : SIGComponent
    {
        var type = typeof(T);

        if (components.ContainsKey(type)) return components[type] as T;
        
        var component = FindObjectOfType<T>();
        if (component == null) component = AddSIGComponent<T>(context);
        else
        {
            component.Initialize(context);
            components[type] = component;
        }
        
        return component;
    }

    public T AddSIGComponent<T>(SIGProcessingContext context) where T : SIGComponent
    {
        var type = typeof(T);
        Debug.Assert(!components.ContainsKey(type), "There should only be one instance of a SIGComponent");
        
        var go = new GameObject(type.Name);
        var component = go.AddComponent<T>();
        
        component.Initialize(context);
        components.Add(type, component);
        
        return component;
    }

    public void FindComponents()
    {
        foreach (var component in FindObjectsOfType<SIGComponent>())
        {
            var type = component.GetType();
            Debug.Assert(!components.ContainsKey(type), "There should only be one instance of a SIGComponent");
            components[type] = component;
        }
    }

    public void InitializeComponents(SIGProcessingContext context)
    {
        foreach (var component in components.Values)
        {
            component.Initialize(context);
        }
    }

    protected virtual SIGProcessingContext PreProcess(bool initialize)
    {
        if(initialize) InitializeGraphProcessor();
        FindComponents();

        var context = CreateContext();
        InitializeComponents(context);

        return context;
    }

    protected virtual void PostProcess(SIGProcessingContext context)
    {
        foreach (var component in components.Values)
        {
            component.Finalize();
        }
        components.Clear();
    }
    
    protected virtual SIGProcessingContext CreateContext()
    {
        if (task == null)
            return new SIGProcessingContext()
            {
                processor = this
            };

        var context = task.Context;
        context.processor = this;
        
        return context;
    }
}
