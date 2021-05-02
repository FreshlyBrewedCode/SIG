using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Poser : MonoBehaviour
{
    public void RandomPose(SIGRandom random)
    {
        foreach (var rotator in GetComponentsInChildren<Rotator>())
        {
            rotator.RandomRotation(random);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Poser))]
public class PoserEditor : Editor<Poser>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Random Pose")) Target.RandomPose(new UnityRandom());
    }
}
#endif
