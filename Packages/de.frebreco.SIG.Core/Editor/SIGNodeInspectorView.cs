using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SIGNodeInspectorView : PinnedElementView
{
    protected IMGUIContainer imguiContainer;
    protected SIGGraphView graphView;
    
    public SIGNodeInspectorView()
    {
        title = "Node Inspector";
    }

    protected override void Initialize(BaseGraphView graphView)
    {
        this.graphView = graphView as SIGGraphView;

        imguiContainer = new IMGUIContainer();
        imguiContainer.onGUIHandler = OnGUI;
        
        content.Add(imguiContainer);
    }

    private void OnGUI()
    {
        if(graphView.selection.Count != 1) return;

        var nodeView = graphView.selection[0] as BaseNodeView;
        if(nodeView == null) return;
        
        
    }
}
