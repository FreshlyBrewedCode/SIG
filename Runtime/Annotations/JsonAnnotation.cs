using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JsonAnnotation<TData, TDataset> : IAnnotation where TData : struct, IJsonAnnotationData
{
    public abstract TData Data { get; }

    protected TDataset dataset;

    public JsonAnnotation(TDataset dataset)
    {
        this.dataset = dataset;
    }

    public virtual byte[] Encode()
    {
        return JsonConverter.ToJson(Data).EncodeUTF8();
    }

    public virtual byte[] EncodeMultiple(IEnumerable<IAnnotation> annotations)
    {
        TData root = default;
        bool hasRoot = false;
        
        foreach (var annotation in annotations)
        {
            if (annotation is JsonAnnotation<TData, TDataset> json)
            {
                if (!hasRoot)
                {
                    root = json.Data;
                    hasRoot = true;
                }
                else
                {
                    root.Append(json.Data);
                }
            }
        }

        return JsonConverter.ToJson(root).EncodeUTF8();
    }
}

public interface IJsonAnnotationData
{
    IJsonAnnotationData Append(IJsonAnnotationData item);
}
