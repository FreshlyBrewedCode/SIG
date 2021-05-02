using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUVector
{
    UVector Value { get; set; }
}

public enum UVectorType
{
    Undefined = 0, Int, Float, Vector2, Vector3, Vector4, Quaternion
}

[System.Serializable]
public struct UVector : IUVector
{
    [SerializeField]
    public float x, y, z, w;
    
    [SerializeField]
    public UVectorType type;
    
    public UVector Value
    {
        get => this;
        set => this = value;
    }
    
    public UVector(int x) { this.x = x; this.y = 0; this.z = 0; this.w = 0; type = UVectorType.Int; }
    public UVector(float x) { this.x = x; this.y = 0; this.z = 0; this.w = 0; type = UVectorType.Float; }
    public UVector(float x, float y) { this.x = x; this.y = y; this.z = 0; this.w = 0; type = UVectorType.Vector2; }
    public UVector(float x, float y, float z) { this.x = x; this.y = y; this.z = z; this.w = 0; type = UVectorType.Vector3; }
    public UVector(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; type = UVectorType.Vector4; }
    public UVector(Vector2 v) { this.x = v.x; this.y = v.y; this.z = 0; this.w = 0; type = UVectorType.Vector2; }
    public UVector(Vector3 v) { this.x = v.x; this.y = v.y; this.z = v.z; this.w = 0; type = UVectorType.Vector3; }
    public UVector(Vector4 v) { this.x = v.x; this.y = v.y; this.z = v.z; this.w = v.w; type = UVectorType.Vector4; }
    public UVector(Quaternion q) { this.x = q.x; this.y = q.y; this.z = q.z; this.w = q.w; type = UVectorType.Quaternion; }

    public float AsFloat => x;
    public int AsInt => (int) x;
    public Vector2 AsVector2 => new Vector2(x, y);
    public Vector3 AsVector3 => new Vector3(x, y, z);
    public Vector4 AsVector4 => new Vector4(x, y, z, w);
    public Quaternion AsQuaternion => new Quaternion(x, y, z, w);
    public Vector3 AsEulerAngles => AsQuaternion.eulerAngles;

    public bool IsVector => (int) type > 2;
    public bool IsPureVector => IsVector && type != UVectorType.Quaternion;
    public bool IsScalar => (int) type <= 2 && type != UVectorType.Undefined;
    
    public delegate float ScalarOperation(float v);
    public UVector ScalarOp(ScalarOperation op)
    {
        return type switch
        {
            UVectorType.Undefined => new UVector(op(x), op(y), op(z), op(w)),
            UVectorType.Int => new UVector(op(x)),
            UVectorType.Float => new UVector(op(x)),
            UVectorType.Vector2 => new UVector(op(x), op(y)),
            UVectorType.Vector3 => new UVector(op(x), op(y), op(z)),
            UVectorType.Vector4 => new UVector(op(x), op(y), op(z), op(w)),
            UVectorType.Quaternion => new UVector(op(x), op(y), op(z), op(w)),
            _ => throw new Exception("Unknown UVector type")
        };
    }
    
    public static implicit operator float(UVector v) => v.AsFloat;
    public static explicit operator UVector(float v) => new UVector(v);
    
    public static implicit operator int(UVector v) => v.AsInt;
    public static explicit operator UVector(int v) => new UVector(v);
    
    public static implicit operator Vector2(UVector v) => v.AsVector2;
    public static explicit operator UVector(Vector2 v) => new UVector(v);
    
    public static implicit operator Vector3(UVector v) => v.AsVector3;
    public static explicit operator UVector(Vector3 v) => new UVector(v);
    
    public static implicit operator Vector4(UVector v) => v.AsVector4;
    public static explicit operator UVector(Vector4 v) => new UVector(v);
    
    public static implicit operator Quaternion(UVector v) => v.AsQuaternion;
    public static explicit operator UVector(Quaternion v) => new UVector(v);
    
    public static UVector operator+(UVector a, UVector b) => new UVector(a.AsVector4 + b.AsVector4);
    public static UVector operator-(UVector a, UVector b) => new UVector(a.AsVector4 - b.AsVector4);
    public static UVector operator *(UVector a, UVector b)
    {
        // Two vectors
        if (a.IsVector && b.IsVector)
        {
            var v = new UVector(UnityEngine.Vector4.Scale(a.AsVector4, b.AsVector4));
            v.type = (UVectorType) Mathf.Max((int) a.type, (int) b.type);
            return v;
        }
        
        // One vector, one scalar
        if (a.IsVector && b.IsScalar) return a.ScalarOp(v => v * b.AsFloat);
        if(b.IsVector && a.IsScalar) return b.ScalarOp(v => v * a.AsFloat);
        
        // Two scalars
        return new UVector(a.AsFloat * b.AsFloat);
    }
    public static UVector operator*(UVector a, float b) => new UVector(a.AsVector4 * b);
    public static UVector operator*(UVector a, int b) => new UVector(a.AsVector4 * b);
    public static UVector operator /(UVector a, UVector b)
    {
        // Two vectors (weired fix to prevent division by zero)
        if (a.IsVector && b.IsVector) return new UVector(
            a.x / b.x != 0 ? b.x : 1,
            a.y / b.y != 0 ? b.y : 1,
            a.z / b.z != 0 ? b.z : 1,
            a.w / b.w != 0 ? b.w : 1);
        
        // One vector, one scalar
        if (a.IsVector && b.IsScalar) return a.ScalarOp(v => v / b.AsFloat);
        if(b.IsVector && a.IsScalar) return b.ScalarOp(v => v / a.AsFloat);
        
        // Two scalars
        return new UVector(a.AsFloat / b.AsFloat);
    }

    public static UVector Int => new UVector(0);
    public static UVector Float => new UVector(0f);
    public static UVector Vector2 => new UVector(UnityEngine.Vector2.zero);
    public static UVector Vector3 => new UVector(UnityEngine.Vector3.zero);
    public static UVector Vector4 => new UVector(UnityEngine.Vector4.zero);
    public static UVector Quaternion => new UVector(UnityEngine.Quaternion.identity);
    
    public override string ToString()
    {
        return AsVector4 + " : " + type;
    }
}

[System.Serializable]
public struct URotation : IUVector
{
    public Quaternion quaternion;
    public Vector3 eulerAngles
    {
        get => quaternion.eulerAngles;
        set => quaternion = Quaternion.Euler(value);
    }
    
    public UVector Value
    {
        get => new UVector(quaternion);
        set
        {
            quaternion = value;
        }
    }

    public URotation(Vector3 eulerAngles)
    {
        quaternion = Quaternion.Euler(eulerAngles);
    }

    public URotation(Quaternion quaternion)
    {
        this.quaternion = quaternion;
    }

    public URotation(UVector vector)
    {
        if (vector.type == UVectorType.Quaternion || vector.type == UVectorType.Vector4)
            quaternion = vector.AsQuaternion;
        else
            quaternion = Quaternion.Euler(vector.AsVector3);
    }
    
    public static implicit operator Vector3(URotation v) => v.eulerAngles;
    public static explicit operator URotation(Vector3 v) => new URotation(v);
    
    public static implicit operator Vector4(URotation v) => v.Value.AsVector4;
    public static explicit operator URotation(Vector4 v) => new URotation(new Quaternion(v.x, v.y, v.z, v.w));
    
    public static implicit operator Quaternion(URotation v) => v.quaternion;
    public static explicit operator URotation(Quaternion v) => new URotation(v);
}

[Serializable]
public abstract class BaseUVector : IUVector
{
    [SerializeField]
    protected UVector vector;
    public virtual UVector Value
    {
        get => vector;
        set => vector = value;
    }
}

public abstract class BaseUVector<T> : BaseUVector
{
    public abstract new T Value { get; set; }
}

[Serializable]
public class UInt : BaseUVector<int>
{
    public override int Value
    {
        get => vector.AsInt;
        set => vector = (UVector)value;
    }
}

[Serializable]
public class UFloat : BaseUVector<float>
{
    public override float Value
    {
        get => vector.AsFloat;
        set => vector = (UVector)value;
    }
}

[Serializable]
public class UVector2 : BaseUVector<Vector2>
{
    public override Vector2 Value
    {
        get => vector.AsVector2;
        set => vector = (UVector)value;
    }
}

[Serializable]
public class UVector3 : BaseUVector<Vector3>
{
    public override Vector3 Value
    {
        get => vector.AsVector3;
        set => vector = (UVector)value;
    }
}

[Serializable]
public class UVector4 : BaseUVector<Vector4>
{
    public override Vector4 Value
    {
        get => vector.AsVector4;
        set => vector = (UVector)value;
    }
}

[Serializable]
public class UQuaternion : BaseUVector<Quaternion>
{
    public override Quaternion Value
    {
        get => vector.AsQuaternion;
        set => vector = (UVector)value;
    }
}
