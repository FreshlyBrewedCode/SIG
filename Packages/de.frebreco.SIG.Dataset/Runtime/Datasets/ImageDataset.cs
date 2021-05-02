using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IImageDataset
{
    UTextureFormat ImageFormat { get; }
    int ImageSize { get; }
}

public abstract class ImageDataset<TItem, TAnnotation> : SIGDataset<TItem, TAnnotation>, IImageDataset
    where TAnnotation : IAnnotation
    where TItem : ImageDatasetItem<TAnnotation>
{
    [SerializeField] protected UTextureFormat imageFormat = UTextureFormat.PNG;
    public virtual UTextureFormat ImageFormat => imageFormat;

    [SerializeField] protected int imageSize = 512;
    public virtual int ImageSize => imageSize;
}

public abstract class ImageDatasetItem<TAnnotation> : IDatasetItem<TAnnotation> 
    where TAnnotation : IAnnotation
{
    protected int id;
    public virtual int Id => id;

    public virtual string ItemIdentifier => Id.ToString("D5");

    protected TAnnotation annotation;
    public virtual TAnnotation Annotation => annotation;
    
    public abstract UTexture Image { get; }

    public virtual string ImageSubDirectory => "images";

    protected string fullImagePath;
    public string FullImagePath => fullImagePath;

    public virtual void Add<TItem>(SIGDataset<TItem, TAnnotation> dataset, int id) where TItem : IDatasetItem<TAnnotation>
    {
        this.id = id;
        if (dataset is IImageDataset imageDataset)
        {
            var img = Image.ConvertTo(imageDataset.ImageFormat);
            fullImagePath = dataset.SaveDatasetFile(this, img.RawTexture, img.FileExtension, ImageSubDirectory);
        }
    }

    public virtual void Annotate(TAnnotation annotation)
    {
        this.annotation = annotation;
    }
}

public abstract class RenderTextureDatasetItem<TAnnotation> : ImageDatasetItem<TAnnotation> where TAnnotation : IAnnotation
{
    protected RenderTexture texture;
    public override UTexture Image => new UTexture(texture);

    public RenderTextureDatasetItem(RenderTexture texture)
    {
        this.texture = texture;
    }
}
