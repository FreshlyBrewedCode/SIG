using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SIGComponent : MonoBehaviour
{
    protected SIGProcessingContext context;
    protected SIGProcessor Processor => context.processor;

    public virtual void Initialize(SIGProcessingContext context)
    {
        this.context = context;
    }
    
    public virtual void Finalize() {}
    
    protected T GetOrCreateComponent<T>() where T : Component
    {
        var comp = GetComponent<T>();
        if (comp == null) comp = gameObject.AddComponent<T>();

        return comp;
    }
}
