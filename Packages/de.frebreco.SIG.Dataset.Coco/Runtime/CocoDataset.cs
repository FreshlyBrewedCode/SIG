using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = MENU_PATH + "/Coco Dataset")]
public class CocoDataset : ImageDataset<CocoDatasetItem, CocoAnnotation>
{
    [SerializeField] protected CocoJsonAnnotationData.Info info;
    [SerializeField] protected CocoCategory category;
    [SerializeField] protected CocoLicense license;

    public CocoJsonAnnotationData.Category Category => category.data;
    public CocoJsonAnnotationData.License License => license.data;
    
    public CocoJsonAnnotationData BaseAnnotation
    {
        get
        {
            return new CocoJsonAnnotationData
            {
                info = info,
                categories = new List<CocoJsonAnnotationData.Category>
                {
                    category.data
                },
                licenses = new List<CocoJsonAnnotationData.License>
                {
                    license.data
                },
                annotations = new List<CocoJsonAnnotationData.Annotation>(),
                images = new List<CocoJsonAnnotationData.Image>()
            };
        }
    }

    public override string AnnotationFileName => "annotations.json";
}

public class CocoDatasetItem : RenderTextureDatasetItem<CocoAnnotation>
{
    public CocoDatasetItem(RenderTexture texture) : base(texture) { }

    public override void Annotate(CocoAnnotation annotation)
    {
        base.Annotate(annotation);
        annotation.Annotate(this);
    }
}
