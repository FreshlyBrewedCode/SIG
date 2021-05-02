using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SIGScene : SIGComponent
{
    
    public override void Initialize(SIGProcessingContext context)
    {
        base.Initialize(context);
        ClearScene();

        this.context.onReset += ClearScene;
    }

    public override void Finalize()
    {
        this.context.onReset -= ClearScene;
        
        base.Finalize();
    }

    public void ClearScene()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    
    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return AddGameObject(GameObject.Instantiate(prefab, position, rotation));
    }

    public GameObject AddGameObject(GameObject gameObject)
    {
        gameObject.transform.SetParent(transform, true);
        return gameObject;
    }

    public GameObject CreateGameObject(string name = "SceneObject")
    {
        return AddGameObject(new GameObject(name));
    }
}

