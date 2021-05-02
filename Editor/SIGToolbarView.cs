using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using Status = UnityEngine.UIElements.DropdownMenuAction.Status;

public class SIGToolbarView : ToolbarView
{
    public SIGToolbarView(BaseGraphView graphView) : base(graphView) {}

    protected ToolbarButtonData showParameters;
    protected ToolbarButtonData showInspector;
    
    protected override void AddButtons()
    {
        AddButton("Center", graphView.ResetPositionAndZoom);
        
        bool exposedParamsVisible = graphView.GetPinnedElementStatus<ExposedParameterView>() != Status.Hidden;
        showParameters = AddToggle("Show Parameters", exposedParamsVisible, (v) => graphView.ToggleView<ExposedParameterView>());

        bool inspectorVisible = graphView.GetPinnedElementStatus<SIGNodeInspectorView>() != Status.Hidden;
        showInspector = AddToggle("Show Inspector", inspectorVisible, (v) => graphView.ToggleView<SIGNodeInspectorView>());
        
        AddButton("Show In Project", () => EditorGUIUtility.PingObject(graphView.graph), false);
        AddButton("Process", Process, false);
    }
    
    public override void UpdateButtonStatus()
    {
        if (showParameters != null)
            showParameters.value = graphView.GetPinnedElementStatus<ExposedParameterView>() != Status.Hidden;
        if (showInspector != null)
            showInspector.value = graphView.GetPinnedElementStatus<SIGNodeInspectorView>() != Status.Hidden;
    }

    protected virtual void Process()
    {
        SIGProcessor.Processor.Process(graphView.graph as SIGGraph);
    }
}