using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;

[System.Serializable]
public abstract class SceneOperationNode : SIGComponentNode<SIGScene>
{
    public const string SCENE_OPERATIONS = "Operations";

    [Output("Scene")] public RenderScene output;
    
    protected override void Process(SIGProcessingContext context)
    {
        output = Apply(context);
        if(debug) AddMessage("Output: " + output.ToString(), NodeMessageType.Info);
        else RemoveMessageContains("Output: ");
    }

    protected abstract RenderScene Apply(SIGProcessingContext context);
}

[System.Serializable]
public abstract class MultiInputOperationNode : SceneOperationNode
{
    [Input("In", allowMultiple = true)] protected RenderScene input;
    
    [CustomPortBehavior(nameof(input))]
    IEnumerable<PortData> GetPortsForInputs(List<SerializableEdge> edges)
    {
        yield return new PortData{ displayName = "In ", displayType = typeof(RenderScene), acceptMultipleEdges = true};
    }

    [CustomPortInput(nameof(input), typeof(RenderScene), allowCast = true)]
    public void GetInputs(List< SerializableEdge > edges)
    {
        input = RenderScene.Merge(edges.Select(e => (RenderScene)e.passThroughBuffer));
    }
}

[System.Serializable, NodeMenuItem(SCENE_OPERATIONS + "/Instantiate")]
public class InstantiateNode : SceneOperationNode
{
    [SerializeField, ShowAsDrawer, Input("Prefab")]
    public GameObject prefab;

    [SerializeField, ShowAsDrawer, Input("Count")]
    public int count = 1;

    protected SIGScene scene;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        if (!Assert(count >= 0, "Count can not be less than 0")) return null; 
        if(count == 0) return RenderScene.Empty;

        scene = Target;
        if (count == 1) return (RenderObject) Instance;

        var instances = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            instances[i] = Instance;
        }
        
        return new RenderObjectCollection(instances);
    }
    
    protected GameObject Instance => 
        prefab == null 
        ? scene.CreateGameObject() 
        : scene.Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
}

[System.Serializable, NodeMenuItem(SCENE_OPERATIONS + "/Merge")]
public class MergeNode : SceneOperationNode
{
    [Input("A")]
    public RenderScene scene1;
    
    [Input("B")]
    public RenderScene scene2;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        if(scene1 == null && scene2 == null) return RenderScene.Empty;
        
        return RenderScene.Merge(scene1, scene2);
    }
}

[System.Serializable, NodeMenuItem(SCENE_OPERATIONS + "/Multi Merge")]
public class MultiMergeNode : MultiInputOperationNode
{
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        return input;
    }
}

[System.Serializable, NodeMenuItem(SCENE_OPERATIONS + "/Include")]
public class IncludeNode : MergeNode
{
    [SerializeField, Input("?")]
    public bool merge;

    protected override RenderScene Apply(SIGProcessingContext context)
    {
        if(scene1 == null && scene2 == null) return RenderScene.Empty;
        
        return merge ? RenderScene.Merge(scene1, scene2) : scene1;
    }
}

[System.Serializable, NodeMenuItem(SCENE_OPERATIONS + "/Count")]
public class CountNode : MultiInputOperationNode
{
    [Output("Count")] public int count;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        count = input.Count;
        return input;
    }
}

[System.Serializable, NodeMenuItem(SCENE_OPERATIONS + "/Find")]
public class FindNode : MultiInputOperationNode
{
    [SerializeField, Input("Name")] public string searchName;
    [SerializeField, Input("Recurse")] public bool recurse = false;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        List<GameObject> results = new List<GameObject>();
        
        foreach (var go in input)
        {
            var result = Find(go.transform, searchName, recurse);
            if(result != null) results.Add(result.gameObject);
        }
        
        return RenderScene.FromList(results);
    }

    protected Transform Find(Transform parent, string path, bool recurse = false)
    {
        var result = parent.Find(path);
        if (result == null && recurse)
        {
            foreach (Transform child in parent)
            {
                result = Find(child, path, true);
                if(result != null) break;
            }
        }

        return result;
    }
}





