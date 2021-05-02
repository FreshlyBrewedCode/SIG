using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using UnityEngine.UIElements;

public class SIGGraphView : BaseGraphView
{
    // public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    // {
    //     evt.menu.AppendSeparator();
		  //
    //     // Add the "Hello World" menu item which print Hello World in the console
    //     evt.menu.AppendAction("Hello World",
    //         (e) => Debug.Log("Hello World"),
    //         DropdownMenu.MenuAction.AlwaysEnabled
    //     );
    //
    //     base.BuildContextualMenu(evt);
    // }

    public SIGGraphView(EditorWindow window) : base(window)
    {
        
    }
    
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        BuildStackNodeContextualMenu(evt);
        base.BuildContextualMenu(evt);
    }
    
    protected void BuildStackNodeContextualMenu(ContextualMenuPopulateEvent evt)
    {
        Vector2 position = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        evt.menu.AppendAction("New Stack", (e) => AddStackNode(new BaseStackNode(position)), DropdownMenuAction.AlwaysEnabled);
    }
}
