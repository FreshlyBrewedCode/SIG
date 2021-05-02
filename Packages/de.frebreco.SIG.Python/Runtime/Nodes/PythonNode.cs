using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphProcessor;
using Python.Runtime;
using UnityEngine;

public abstract class PythonNode : SIGNode
{
    public const string PYTHON = "Python";
    
    protected abstract string PythonNodeClass { get; }

    protected abstract Dictionary<string, object> PullInputs();
    protected abstract void PushOutputs(Dictionary<string, PyObject> outputs);
    
    protected override void Process(SIGProcessingContext context)
    {
#if UNITY_EDITOR
        var inputs = PullInputs();
        PushOutputs(SIGPython.Services.ProcessNode(PythonNodeClass, inputs.ToPyDict()));
#else
        context.Error();
        throw new PlatformNotSupportedException("SIGPython is only available in the Unity editor");
#endif
    }
}

public abstract class ReflectionPythonNode : PythonNode
{
    protected override Dictionary<string, object> PullInputs()
    {
        var inputs = new Dictionary<string, object>();
        foreach (var port in inputPorts)
        {
            inputs.Add(port.fieldName, port.fieldInfo.GetValue(this));
        }
        return inputs;
    }

    protected override void PushOutputs(Dictionary<string, PyObject> outputs)
    {
        foreach (var port in outputPorts)
        {
            if (outputs.ContainsKey(port.fieldName))
            {
                PyObject value = outputs[port.fieldName];
                port.fieldInfo.SetValue(this, value.AsManagedObject(port.fieldInfo.FieldType));
            }
        }
    }
}

[System.Serializable, NodeMenuItem(PYTHON + "/Python Add")]
public class ExmaplePythonNode : ReflectionPythonNode
{
    protected override string PythonNodeClass => "ExampleAddNode";

    [SerializeField, Input("A")]
    public float a;
    
    [SerializeField, Input("B")]
    public float b;
    
    [Output("Out")]
    public float output;
}

// [System.Serializable, NodeMenuItem(PYTHON + "/COCO Annotation")]
// public class CocoAnnotationNode : PythonNode
// {
//     protected override string PythonNodeClass => "CocoAnnotationNode";
//     
//     [NonSerialized, Input("Mask")]
//     public UTexture mask;
//
//     [Output("JSON")]
//     public string json;
//
//     [Output("Bytes")] public byte[] rawTexture;
//     
//     protected override Dictionary<string, object> PullInputs()
//     {
//         rawTexture = mask.ConvertTo(UTextureFormat.PNG).RawTexture;
//         return new Dictionary<string, object>
//         {
//             {"mask_texture", rawTexture.ToList()}
//         };
//     }
//
//     protected override void PushOutputs(Dictionary<string, PyObject> outputs)
//     {
//         json = outputs.GetOutput<string>(nameof(json));
//     }
// }

[System.Serializable, NodeMenuItem(IO + "/Save Bytes")]
public class SaveBytesNode : SaveFileNode
{
    [NonSerialized, Input("Bytes")] public byte[] bytes;
    
    protected override byte[] Data => bytes;
    protected override bool HasData => IsConnected(nameof(bytes));
}
