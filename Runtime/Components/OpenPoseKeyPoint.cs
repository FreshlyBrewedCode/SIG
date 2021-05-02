using System;
using UnityEngine;
using UnityEngine.Serialization;

public class OpenPoseKeyPoint : MonoBehaviour, ISIGKeypoint
{
    public int id;
    [FormerlySerializedAs("cocoKeypoint")] public CocoKeypointType type = CocoKeypointType.None;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(type != CocoKeypointType.None)
            Gizmos.DrawSphere(transform.position, 0.3f * transform.lossyScale.magnitude);
    }

    public Keypoint Keypoint => new CocoKeypoint(id, type);
    public Vector3 Position => transform.position;
}

public enum CocoKeypointType
{
    None = -1, Nose = 0, LeftEye, RightEye, LeftEar, RightEar, LeftShoulder, RightShoulder, LeftElbow, RightElbow, LeftWrist, RightWrist, LeftHip, RightHip, LeftKnee, RightKnee, LeftAnkle, RightAnkle
}

[System.Serializable]
public class CocoKeypoint : Keypoint
{
    [SerializeField] protected CocoKeypointType type;
    public CocoKeypointType Type => type;

    public CocoKeypoint(int id, CocoKeypointType type) : base(id)
    {
        this.type = type;
    }
}