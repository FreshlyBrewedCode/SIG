using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;

[System.Serializable]
public abstract class SelectNode : MultiInputOperationNode
{
    public const string SELECT = SCENE_OPERATIONS + "/Select/Select ";

    public override Color color => new Color(0.71f, 0.71f, 0.71f);

    protected bool AssertIndex(int index, string message)
    {
        return Assert(index >= 0 && index < input.Count, message);
    }

    protected bool InputIsEmpty => input == null || input.Count <= 0;
}

[System.Serializable, NodeMenuItem(SELECT + "Index")]
public class SelectIndexNode : SelectNode
{
    [SerializeField, ShowAsDrawer, Input("Index")]
    public int index;

    protected override RenderScene Apply(SIGProcessingContext context)
    {
        if (InputIsEmpty) return RenderScene.Empty;
        
        int i = Mathf.RoundToInt(index);
        if (!AssertIndex(i, "Index is out of range")) return null;

        return new RenderObject(input[i]);
    }
}

[System.Serializable, NodeMenuItem(SELECT + "Range")]
public class SelectRangeNode : SelectNode
{
    [SerializeField, ShowAsDrawer, Input("Start")]
    public float start;
    
    [SerializeField, ShowAsDrawer, Input("Count")]
    public float count;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        if (InputIsEmpty) return RenderScene.Empty;
        
        int s = Mathf.RoundToInt(start);
        int c = Mathf.RoundToInt(count);
        
        if (!AssertIndex(s, "Start is out of range")) return null;
        if(!Assert(c >= 0, "Count can not be less than 0")) return null;
        if (!AssertIndex(s + c - 1, "End is out of range")) return null;
        
        var objects = new GameObject[c];
        for (int i = s; i < s + c; i++)
        {
            objects[i - s] = input[i];
        }
        
        return new RenderObjectCollection(objects);
    }
}

[System.Serializable, NodeMenuItem(SELECT + "Random")]
public class SelectRandomNode : SelectNode
{
    [SerializeField, ShowAsDrawer, Input("Count")]
    public float count = 1f;
    
    [SerializeField, Setting("Select Unique")]
    protected bool selectUnique = true;
    
    protected override RenderScene Apply(SIGProcessingContext context)
    {
        if (InputIsEmpty) return RenderScene.Empty;
        
        int c = Mathf.RoundToInt(count);
        
        if(!Assert(c >= 0, "Count can not be less than 0")) return RenderScene.Empty;
        if(!Assert(c <= input.Count || !selectUnique, "Input has not enough unique elements")) return input;
        
        // Single random
        if(c == 1) return new RenderObject(input[Random.Range(0, input.Count)]);

        GameObject[] gameObjects = new GameObject[c];
        
        // Unique random with count
        if (selectUnique)
        {
            List<int> used = new List<int>(c);
            for (int i = 0; i < c; i++)
            {
                var random = Random.Range(0, input.Count);;
                while (used.Contains(random)) random = (random + 1) % c;
                
                gameObjects[i] = input[random];
                used.Add(random);
            }
            return new RenderObjectCollection(gameObjects);
        }
        
        // Random with count
        for (int i = 0; i < c; i++) gameObjects[i] = input[Random.Range(0, input.Count)];
        
        return new RenderObjectCollection(gameObjects);
    }
}

[System.Serializable, NodeMenuItem("Drawer Test")]
public class DrawerTestNode : BaseNode
{
    [SerializeField, Input("Quaternion")]
    public Quaternion value1;
    
    [SerializeField, ShowAsDrawer, Input("Drawer float")]
    public float value2;
}

[System.Serializable, NodeMenuItem("Drawer Test Inherited")]
public class InheritedDrawerNode : DrawerTestNode
{
    [SerializeField, Input("Drawer Vector3")]
    public Vector3 value3;

    protected override void Enable()
    {
        Debug.Log(GetNodeFields().Aggregate("", (s, f) => s += f.Name + 
                                                               $"({f.CustomAttributes.Aggregate("", (s, a) => s += a.AttributeType.Name + ", ")}), "));
    }
}

