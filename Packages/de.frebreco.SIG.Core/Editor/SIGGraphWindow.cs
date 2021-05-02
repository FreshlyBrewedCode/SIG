using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class SIGGraphWindow : BaseGraphWindow
{
    // Add the window in the editor menu
    [MenuItem("SIG/Graph Window")]
    public static SIGGraphWindow Open()
    {
        var graphWindow = GetWindow<SIGGraphWindow>();
        graphWindow.Show();

        return graphWindow;
    }

    [OnOpenAsset]
    public static bool OnOpenGraph(int instanceID, int line)
    {
        if (Selection.activeObject is SIGGraph graph)
        {
            var graphWindow = CreateWindow<SIGGraphWindow>();
            graphWindow.InitializeGraph(graph);
            graphWindow.Show();
            
            return true;
        }

        return false;
    }
    
    protected override void InitializeWindow(BaseGraph graph)
    {
        // Set the window title
        titleContent = new GUIContent("SIG Graph");

        // Here you can use the default BaseGraphView or a custom one (see section below)
        var graphView = new SIGGraphView(this);

        // Add toolbar
        graphView.Add(new SIGToolbarView(graphView));
        
        
        // Add minimap
        // graphView.Add(new MiniMapView(graphView));
        // graphView.Add(new GridBackground());
        
        rootView.Add(graphView);
    }

    protected override void InitializeGraphView(BaseGraphView view)
    {
        base.InitializeGraphView(view);
        
        view.OpenPinned<ExposedParameterView>();
    }
    
}
