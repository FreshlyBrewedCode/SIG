using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface that provides access to a Keypoint. Should be implemented by any keypoint <see cref="MonoBehaviour"/> components.
/// </summary>
public interface ISIGKeypoint
{
    Keypoint Keypoint { get; }
    Vector3 Position { get; }
}

/// <summary>
/// A basic <see cref="MonoBehaviour"/> component that returns a simple Keypoint with an id.
/// </summary>
public class SIGKeypoint : MonoBehaviour, ISIGKeypoint
{
    [SerializeField] protected int id;
    public Keypoint Keypoint => new Keypoint(id);
    public Vector3 Position => transform.position;
}

/// <summary>
/// A class containing meta information about a keypoint.
/// </summary>
[System.Serializable]
public class Keypoint
{
    [SerializeField] protected int id;
    public int Id => id;

    public Keypoint(int id)
    {
        this.id = id;
    }
}

/// <summary>
/// A wrapper around a Keypoint that contains the x and y position of the keypoint in an image.
/// </summary>
public abstract class ImageKeypoint
{
    protected float x, y;
    public virtual float X => x;
    public virtual float Y => y;
    public abstract Keypoint Keypoint { get; }

    protected ImageKeypoint(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

/// <summary>
/// <inheritdoc cref="ImageKeypoint"/>
/// </summary>
/// <typeparam name="TKeypoint">The type of keypoint that is wrapped by the ImageKeypoint.</typeparam>
public class ImageKeypoint<TKeypoint> : ImageKeypoint where TKeypoint : Keypoint
{
    protected TKeypoint keypoint;
    public override Keypoint Keypoint => keypoint;

    public ImageKeypoint(int x, int y, TKeypoint keypoint) : base(x, y)
    {
        this.keypoint = keypoint;
    }
}
