using System;
using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseUVectorField : VisualElement, INotifyValueChanged<UVector>
{
    public abstract void SetValueWithoutNotify(UVector newValue);
    public abstract UVector value { get; set; }
}

public abstract class BaseUVectorField<TField, TType> : BaseUVectorField  where TField : VisualElement, INotifyValueChanged<TType>
{
    public TField vectorField;
    protected UVector rawValue;
    
    protected BaseUVectorField(TField vectorField) : base()
    {
        this.vectorField = vectorField;
        // vectorField = string.IsNullOrEmpty(label) ? new Vector4Field() : new Vector4Field(label);
        vectorField.RegisterValueChangedCallback(evt => value = ToUVector(evt.newValue)); 
        Add(vectorField);
    }

    protected abstract TType FromUVector(UVector value);
    protected abstract UVector ToUVector(TType value);
    
    public override void SetValueWithoutNotify(UVector newValue)
    {
        rawValue = newValue;
        vectorField.SetValueWithoutNotify(FromUVector(newValue));
    }
    
    
    public override UVector value
    {
        get => rawValue;
        set
        {
            using (ChangeEvent<UVector> pooled = ChangeEvent<UVector>.GetPooled(rawValue, value))
            {
                pooled.target = this;
                SetValueWithoutNotify(value);
                SendEvent(pooled);
            }
        }
    }
}

public class UVectorFloatField : BaseUVectorField<FloatField, float>
{
    public UVectorFloatField(string label = null) : base(new FloatField(label)) { }
    protected override float FromUVector(UVector value) => value.AsFloat;
    protected override UVector ToUVector(float value) => new UVector(value);
}

public class UVector4Field : BaseUVectorField<Vector4Field, Vector4>
{
    public UVector4Field(string label = null) : base(new Vector4Field(label)) { }
    protected override Vector4 FromUVector(UVector value) => value.AsVector4;
    protected override UVector ToUVector(Vector4 value) => new UVector(value);
}

public class UVector3Field : BaseUVectorField<Vector3Field, Vector3>
{
    public UVector3Field(string label = null) : base(new Vector3Field(label)) { }
    protected override Vector3 FromUVector(UVector value) => value.AsVector3;
    protected override UVector ToUVector(Vector3 value) => new UVector(value);
}

public class UVector2Field : BaseUVectorField<Vector2Field, Vector2>
{
    public UVector2Field(string label = null) : base(new Vector2Field(label)) { }
    protected override Vector2 FromUVector(UVector value) => value.AsVector2;
    protected override UVector ToUVector(Vector2 value) => new UVector(value);
}

[FieldDrawer(typeof(UVector))]
public class UVectorField : BaseUVectorField<Vector4Field, Vector4>
{

    public UVectorField(string label = null) : base(new Vector4Field(label)) { }

    protected override Vector4 FromUVector(UVector value) => value.AsVector4;
    protected override UVector ToUVector(Vector4 value) => new UVector(value);
    
    // public void SetValueWithoutNotify(UVector newValue)
    // {
    //     if (uvectorField.value.type != newValue.type)
    //     {
    //         uvectorField.RemoveFromHierarchy();
    //         uvectorField = GetField(newValue.type);
    //         Add(uvectorField);
    //     }
    //
    //     uvectorField.SetValueWithoutNotify(newValue);
    // }

    protected BaseUVectorField GetField(UVectorType type) => type switch
    {
        UVectorType.Undefined => new UVector4Field(),
        UVectorType.Int => new UVectorFloatField(),
        UVectorType.Float => new UVectorFloatField(),
        UVectorType.Vector2 => new UVector2Field(),
        UVectorType.Vector3 => new UVector3Field(),
        UVectorType.Vector4 => new UVector4Field(),
        UVectorType.Quaternion => new UVector4Field(),
        _ => new UVector4Field()
    };
    
    // protected int GetFieldCount(UVectorType type) => type switch
    // {
    //     UVectorType.Undefined => 4,
    //     UVectorType.Int => 1,
    //     UVectorType.Float => 1,
    //     UVectorType.Vector2 => 2,
    //     UVectorType.Vector3 => 3,
    //     UVectorType.Vector4 => 4,
    //     UVectorType.Quaternion => 4,
    //     _ => 4
    // };

    // protected void SetFieldCount(int count)
    // {
    //     for (int i = 0; i < 4; i++)
    //     {
    //         uvectorField.vectorField[0][i].style.visibility = i < count
    //             ? new StyleEnum<Visibility>(Visibility.Visible)
    //             : new StyleEnum<Visibility>(Visibility.Hidden);
    //     }
    // }
}

