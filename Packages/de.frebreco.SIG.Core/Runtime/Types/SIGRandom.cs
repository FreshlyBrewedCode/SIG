using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom interface to access RNG in SIG.
/// </summary>
public abstract class SIGRandom
{
    public abstract float RandomValue { get; }
    public abstract void Seed(int seed);
    public abstract float Range(float start, float end);
    public abstract int Range(int start, int end);
}

public class UnityRandom : SIGRandom
{
    public override float RandomValue => Random.value;
    public override void Seed(int seed) => Random.InitState(seed);
    public override float Range(float start, float end) => Random.Range(start, end);
    public override int Range(int start, int end) => Random.Range(start, end);
}
