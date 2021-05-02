using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class SIGDataset<TItem, TAnnotation> : ScriptableObject 
    where TAnnotation : IAnnotation 
    where TItem : IDatasetItem<TAnnotation>
{
    public const string MENU_PATH = "SIG/Datasets";

    [SerializeField, HideInInspector] protected List<TItem> items = new List<TItem>();
    public ReadOnlyCollection<TItem> Items => new ReadOnlyCollection<TItem>(items);
    public int Size => items.Count;
    
    [SerializeField, Path(PathPanel.SaveFolder, title = "Select Dataset Path")] protected string datasetPath;
    public virtual string DatasetPath => datasetPath;
    public abstract string AnnotationFileName { get; }
    
    public virtual int AddItem(TItem item)
    {
        var id = GetNextId();
        item.Add(this, id);
        items.Add(item);
        return id;
    }

    public virtual void AddAnnotation(int id, TAnnotation annotation)
    {
        GetItem(id).Annotate(annotation);
    }
    
    // Simple count based id is fine as long as we don't remove/rearrange items in the list
    public virtual int GetNextId() => items.Count;
    public virtual TItem GetItem(int id) => items[id];

    public virtual void Clear()
    {
        items.Clear();
    }
    
    // Save an external dataset file to the dataset path
    public virtual string SaveDatasetFile(byte[] data, string fileName, string subDirectory = "")
    {
        string path = Path.Combine(DatasetPath, subDirectory);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        path = Path.Combine(path, fileName);
        
        // Save file
        using (var fs = new FileStream(path, FileMode.OpenOrCreate))
        {
            fs.Write(data, 0, data.Length);
        }

        return path;
    }
    
    // String data variant
    public string SaveDatasetFile(string content, string fileName, string subDirectory = "") => SaveDatasetFile(content.EncodeUTF8(), fileName, subDirectory);
    
    // Save an external dataset file that contains the item identifier in the file name
    public string SaveDatasetFile(IDatasetItem<TAnnotation> source, byte[] data, string fileIdAndExtension,
        string subDirectory = "") =>
        SaveDatasetFile(data, source.ItemIdentifier + fileIdAndExtension, subDirectory);
    
    // String data variant
    public string SaveDatasetFile(IDatasetItem<TAnnotation> source, string content, string fileIdAndExtension,
        string subDirectory = "") => SaveDatasetFile(source, content.EncodeUTF8(), fileIdAndExtension, subDirectory);

    public virtual byte[] AggregateAnnotations()
    {
        if (Size <= 0) return new byte[0];
        return items[0].Annotation.EncodeMultiple((IEnumerable<IAnnotation>)Annotations);
    }

    public string SaveDatasetAnnotations(string subDirectory = "")
        => SaveDatasetFile(AggregateAnnotations(), AnnotationFileName, subDirectory);

    public virtual IEnumerable<TAnnotation> Annotations => items.Select(i => i.Annotation);
}

public interface IDatasetItem<TAnnotation> where TAnnotation : IAnnotation
{
    int Id { get; }
    string ItemIdentifier { get; }
    TAnnotation Annotation { get; }

    void Add<TItem>(SIGDataset<TItem, TAnnotation> dataset, int id) where TItem : IDatasetItem<TAnnotation>;
    void Annotate(TAnnotation annotation);
}

public interface IAnnotation
{
    byte[] Encode();
    byte[] EncodeMultiple(IEnumerable<IAnnotation> annotations);
}

public static class DataConversionExtensions
{
    public static byte[] EncodeUTF8(this string content)
    {
        int length = Encoding.UTF8.GetByteCount(content);
        byte[] data = new byte[length];
        Encoding.UTF8.GetBytes(content, 0, content.Length, data, 0);
        return data;
    }
}