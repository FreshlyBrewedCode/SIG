using System;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Rotator : MonoBehaviour
{
    public Vector3 limitsLow;
    public Vector3 limitsHigh;

    [SerializeField, HideInInspector] protected Quaternion cachedRotation;
    [SerializeField, HideInInspector] protected bool hasCache = false;
    
    public void Cache()
    {
        cachedRotation = transform.localRotation;
        hasCache = true;
    }

    public void Reset()
    {
        transform.localRotation = cachedRotation;
    }

    public void RandomRotation(SIGRandom random)
    {
        if(!hasCache) Cache();
        Reset();
        
        transform.localEulerAngles += new Vector3(
            random.Range(limitsLow.x, limitsHigh.x),
            random.Range(limitsLow.y, limitsHigh.y),
            random.Range(limitsLow.z, limitsHigh.z)
        );
    }
    
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        Handles.color = Handles.xAxisColor;
        Handles.DrawSolidArc(transform.position, transform.right, transform.forward, - limitsLow.x + limitsHigh.x , 0.5f);
        Handles.color = Handles.yAxisColor;
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, limitsHigh.y - limitsLow.y, 0.5f);
        Handles.color = Handles.zAxisColor;
        Handles.DrawSolidArc(transform.position, transform.forward, transform.up, limitsHigh.z - limitsLow.z, 0.5f);
        #endif
    }
}