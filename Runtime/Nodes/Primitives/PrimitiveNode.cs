using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEditor;
using UnityEngine;

[System.Serializable, NodeMenuItem("Primitives/Float")]
public abstract class PrimitiveNode<T> : SIGNode
{
    public const string PRIMITIVE_CATEGORY = "Primitives";
    public override Color color => new Color(0.27f, 0.74f, 0.84f);

    [Output("Out")]
    public T output;

    protected abstract T Input { get; }

    public override string name => ObjectNames.NicifyVariableName(typeof(T).Name).Replace(" ", "");

    protected override void Process() => output = Input;
}

[System.Serializable, NodeMenuItem(PRIMITIVE_CATEGORY + "/Bool")]
public class BoolNode : PrimitiveNode<bool>
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public bool input;
    protected override bool Input => input;
}

[System.Serializable, NodeMenuItem(PRIMITIVE_CATEGORY + "/Float")]
public class FloatNode : PrimitiveNode<float>
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public float input;
    protected override float Input => input;

    public override string name => "Float";
}

[System.Serializable, NodeMenuItem(PRIMITIVE_CATEGORY+ "/String")]
public class StringNode : PrimitiveNode<string> 
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public string input;
    protected override string Input => input;
}

[System.Serializable, NodeMenuItem(PRIMITIVE_CATEGORY+ "/Vector2")]
public class Vector2Node : PrimitiveNode<Vector2> 
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public Vector2 input;
    protected override Vector2 Input => input;
}

[System.Serializable, NodeMenuItem(PRIMITIVE_CATEGORY+ "/Vector3")]
public class Vector3Node : PrimitiveNode<Vector3> 
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public Vector3 input;
    protected override Vector3 Input => input;
}

[System.Serializable, NodeMenuItem(PRIMITIVE_CATEGORY+ "/Vector4")]
public class Vector4Node : PrimitiveNode<Vector4> 
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public Vector4 input;
    protected override Vector4 Input => input;
}

[System.Serializable, NodeMenuItem(PRIMITIVE_CATEGORY+ "/Color")]
public class ColorNode : PrimitiveNode<Color> 
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public Color input;
    protected override Color Input => input;
}

[System.Serializable, NodeMenuItem(PRIMITIVE_CATEGORY+ "/Material")]
public class MaterialNode : PrimitiveNode<Material> 
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public Material input;
    protected override Material Input => input;
}

[System.Serializable, NodeMenuItem(PRIMITIVE_CATEGORY+ "/Texture")]
public class TextureNode : PrimitiveNode<Texture> 
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public Texture input;
    protected override Texture Input => input;
}

[System.Serializable, NodeMenuItem(PRIMITIVE_CATEGORY+ "/Shader")]
public class ShaderNode : PrimitiveNode<Shader> 
{
    [Input("In"), SerializeField, ShowAsDrawer]
    public Shader input;
    protected override Shader Input => input;
}


