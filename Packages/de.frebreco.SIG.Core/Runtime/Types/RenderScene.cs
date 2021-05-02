using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class RenderScene : IList<GameObject>
{
    public abstract IEnumerator<GameObject> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public abstract void Add(GameObject item);
    public abstract void Clear();
    public abstract bool Contains(GameObject item);
    public abstract void CopyTo(GameObject[] array, int arrayIndex);
    public abstract bool Remove(GameObject item);
    public abstract int Count { get; }
    public bool IsReadOnly => true;
    public abstract int IndexOf(GameObject item);
    public abstract void Insert(int index, GameObject item);
    public abstract void RemoveAt(int index);
    public abstract GameObject this[int index] { get; set; }

    public bool IsEmpty => Count <= 0;
    public static RenderScene Empty => new RenderObject(null);

    public virtual T GetComponent<T>()
    {
        return this.Select(g => g.GetComponent<T>()).FirstOrDefault();
    }

    public virtual IEnumerable<T> GetComponents<T>(bool includeChildren = false)
    {
        if(includeChildren)
            return this.Select(g => g.GetComponentsInChildren<T>()).SelectMany(g => g);
        return this.Select(g => g.GetComponent<T>());
    }
    
    public static RenderScene Merge(RenderScene a, RenderScene b)
    {
        if (a == null && b == null) return RenderScene.Empty;
        if (a == null || a.Count <= 0) return b;
        if (b == null || b.Count <= 0) return a;
        
        var collection = new RenderObjectCollection(a.Count + b.Count);
        a.CopyTo(collection, 0);
        b.CopyTo(collection, a.Count);

        return collection;
    }
    
    public static RenderScene Merge(IEnumerable<RenderScene> scenes)
    {
        if (scenes == null) return Empty;
        var count = scenes.Sum(s => s?.Count ?? 0);
        if (count <= 0) return Empty;
        
        var collection = new RenderObjectCollection(count);
        var index = 0;
        foreach (var scene in scenes)
        {
            if(scene == null) continue;
            
            scene.CopyTo(collection, index);
            index += scene.Count;
        }

        return collection;
    }

    public override string ToString()
    {
        return Count <= 0 ? "Empty" : this.Aggregate("", (s, g) => s += $"{g.name}, ").TrimEnd(' ', ',');
    }

    public static implicit operator int(RenderScene scene) => scene.Count;

    public static RenderScene FromList(IList<GameObject> list)
    {
        if (list.Count <= 0) return RenderScene.Empty;
        if(list.Count == 1) return new RenderObject(list[0]);
        return new RenderObjectCollection(list.ToArray());
    }
}

public class RenderObject : RenderScene
{
    protected GameObject gameObject;

    public RenderObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    
    // Cast from/to GameObject
    public static explicit operator RenderObject(GameObject gameObject) => new RenderObject(gameObject);
    public static implicit operator GameObject(RenderObject obj) => obj.gameObject;
    
    public override IEnumerator<GameObject> GetEnumerator()
    {
        if(gameObject == null) yield break;
        yield return gameObject;
    }

    public override bool Contains(GameObject item) => item == gameObject;
    public override void CopyTo(GameObject[] array, int arrayIndex)
    {
        if(gameObject != null) array[arrayIndex] = gameObject;
    }

    public override int Count => gameObject == null ? 0 : 1;
    public override int IndexOf(GameObject item) => item == gameObject ? 0 : -1;
    
    public override GameObject this[int index]
    {
        get => index == 0 && Count > 0 ? gameObject : throw new IndexOutOfRangeException();
        set => throw new ReadOnlyException("RenderObject is readonly");
    }
    
    public override void Add(GameObject item) => throw new ReadOnlyException("RenderObject is readonly");
    public override void Clear() => throw new ReadOnlyException("RenderObject is readonly");
    public override bool Remove(GameObject item) => throw new ReadOnlyException("RenderObject is readonly");
    public override void Insert(int index, GameObject item) => throw new ReadOnlyException("RenderObject is readonly");
    public override void RemoveAt(int index) => throw new ReadOnlyException("RenderObject is readonly");
}

public class RenderObjectCollection : RenderScene
{
    protected GameObject[] gameObjects;
    protected IList<GameObject> GameObjects => gameObjects;

    public RenderObjectCollection(GameObject[] gameObjects)
    {
        this.gameObjects = gameObjects;
    }
    
    public RenderObjectCollection(int size)
    {
        this.gameObjects = new GameObject[size];
    }
    
    // Cast from/to GameObject array
    public static explicit operator RenderObjectCollection(GameObject[] gameObjects) => new RenderObjectCollection(gameObjects);
    public static implicit operator GameObject[](RenderObjectCollection collection) => collection.gameObjects;
    
    
    public override IEnumerator<GameObject> GetEnumerator() => GameObjects.GetEnumerator();
    public override bool Contains(GameObject item) => GameObjects.Contains(item);
    public override void CopyTo(GameObject[] array, int arrayIndex) => GameObjects.CopyTo(array, arrayIndex);
    public override int Count => GameObjects.Count;
    public override int IndexOf(GameObject item) => GameObjects.IndexOf(item);
    
    public override GameObject this[int index]
    {
        get => gameObjects[index];
        set => throw new ReadOnlyException("RenderObjectCollection is readonly");
    }
    
    public override void Add(GameObject item) => throw new ReadOnlyException("RenderObjectCollection is readonly");
    public override void Clear() => throw new ReadOnlyException("RenderObjectCollection is readonly");
    public override bool Remove(GameObject item) => throw new ReadOnlyException("RenderObjectCollection is readonly");
    public override void Insert(int index, GameObject item) => throw new ReadOnlyException("RenderObjectCollection is readonly");
    public override void RemoveAt(int index) => throw new ReadOnlyException("RenderObjectCollection is readonly");
}
