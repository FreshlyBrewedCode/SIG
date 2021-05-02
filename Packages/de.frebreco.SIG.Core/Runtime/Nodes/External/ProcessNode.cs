using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GraphProcessor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UIElements;
#endif

public abstract class ExternalNode : SIGNode
{
    public const string EXTERNAL = "External";
}

public abstract class ProcessNode : ExternalNode
{
    protected abstract string ExecutablePath { get; }
    protected abstract string Arguments { get; }

    [SerializeField][HideInInspector]
    protected string output;
    public string Output => output;

    public event System.Action<string> onOutputChanged; 
    
    protected override void Process(SIGProcessingContext context)
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = ExecutablePath;
        start.Arguments = Arguments;
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        using(Process process = System.Diagnostics.Process.Start(start))
        {
            using(StreamReader reader = process.StandardOutput)
            {
                output = reader.ReadToEnd();
                onOutputChanged?.Invoke(output);
            }
        }
    }
}

[System.Serializable, NodeMenuItem(EXTERNAL + "/Run Process")]
public class RunProcessNode : ProcessNode
{
    [SerializeField, Input("Exe")]
    public string executable;
    protected override string ExecutablePath => executable;

    [SerializeField, Input("Args")]
    public string arguments;
    protected override string Arguments => arguments;

    [Output("Out")]
    public string nodeOutput;

    protected override void Process(SIGProcessingContext context)
    {
        base.Process(context);
        nodeOutput = output;
    }
}

#if UNITY_EDITOR
[NodeCustomEditor(typeof(ProcessNode))]
public class ProcessNodeView : BaseNodeView
{
    protected TextField outputTextField;
    protected Button expandButton;
    protected ProcessNode Target => nodeTarget as ProcessNode;
    protected bool outputExpanded = false;

    protected string ExpandedLabel => outputExpanded ? "Collapse" : "Expand";
    
    public override void Enable()
    {
        base.Enable();
        
        expandButton = new Button(ToogleExpand);
        expandButton.text = ExpandedLabel;
        controlsContainer.Add(expandButton);
        outputTextField = new TextField(int.MaxValue, true, false, '*');
        outputTextField.isReadOnly = true;
        outputTextField.value = Target.Output;
        controlsContainer.Add(outputTextField);

        Target.onOutputChanged += output => outputTextField.value = output;
        SetExpanded(false);
    }

    public void SetExpanded(bool expanded)
    {
        outputExpanded = expanded;
        expandButton.text = ExpandedLabel;

        if (outputExpanded)
        {
            outputTextField.style.maxWidth = new StyleLength(StyleKeyword.Auto);
            outputTextField.style.maxHeight = new StyleLength(StyleKeyword.Auto);
        }
        else 
        {
            outputTextField.style.maxWidth = new StyleLength(200);
            outputTextField.style.maxHeight = new StyleLength(30);
        }
    }
    
    public void ToogleExpand()
    {
        SetExpanded(!outputExpanded);
    }
}
#endif
