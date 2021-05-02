using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

public class CocoAnnotation : JsonAnnotation<CocoJsonAnnotationData, CocoDataset>
{
    public CocoAnnotation(CocoDataset dataset) : base(dataset)
    {

    }
 
    public CocoJsonAnnotationData.Image image;
    public CocoJsonAnnotationData.Annotation imageAnnotation;

    public override CocoJsonAnnotationData Data
    {
        get
        {
            var root = dataset.BaseAnnotation;
            root.images.Add(image);
            root.annotations.Add(imageAnnotation);
            return root;
        }
    }

    public void Annotate(CocoDatasetItem item)
    {
        image = new CocoJsonAnnotationData.Image()
        {
            id = item.Id, 
            width = item.Image.Width,
            height = item.Image.Height, 
            file_name = item.FullImagePath,
            license = dataset.License.id,
            date_captured = DateTime.Now,
            coco_url = "",
            flickr_url = ""
        };
    }
    
    public override byte[] EncodeMultiple(IEnumerable<IAnnotation> annotations)
    {
        var root = dataset.BaseAnnotation;
        
        foreach (var annotation in annotations)
        {
            if (annotation is CocoAnnotation coco)
            {
                root.images.Add(coco.image);
                root.annotations.Add(coco.imageAnnotation);
            }
        }

        return JsonConverter.ToJson(root).EncodeUTF8();
    }

    public static List<int> GetCocoKeypoints(List<ImageKeypoint> keypoints)
    {
        var cocoPoints = new List<int>();

        int cocoIndex = 0;
        
        foreach (var keypoint in keypoints
            .Where(k => k.Keypoint is CocoKeypoint c && c.Type != CocoKeypointType.None)
            .OrderBy(k => (k.Keypoint as CocoKeypoint).Type))
        {
            var cocoKeypoint = keypoint.Keypoint as CocoKeypoint;
            
            // Keypoint is not included
            if ((int) cocoKeypoint.Type != cocoIndex)
            {
                cocoPoints.Add(0); 
                cocoPoints.Add(0); 
                cocoPoints.Add(0);
            }
            else
            {
                cocoPoints.Add(Mathf.RoundToInt(keypoint.X));
                cocoPoints.Add(Mathf.RoundToInt(keypoint.Y));
                cocoPoints.Add(2); // Always use "labeled and visible" for now
            }

            cocoIndex++;
        }

        return cocoPoints;
    }
}

public class ScriptableCocoJson<T> : ScriptableJson<T> where T : struct
{
    public const string MENU_PATH = "SIG/Annotations/Coco";
}

[System.Serializable]
public struct CocoJsonAnnotationData : IJsonAnnotationData
{
    public Info info;
    public List<Image> images;
    public List<Annotation> annotations;
    public List<Category> categories;
    public List<License> licenses;
    
    public IJsonAnnotationData Append(IJsonAnnotationData item)
    {
        if (item is CocoJsonAnnotationData json)
        {
            images.AddRange(json.images);
            annotations.AddRange(json.annotations);
        }

        return this;
    }

    // Json Regex
    // "(\w*)": (\w*), => public $2 $1;\n

    [System.Serializable]
    public struct Info
    {
        public int year;
        public string version;
        public string description;
        public string contributor;
        public string url;
        public DateTime date_created;
    }

    [System.Serializable]
    public struct Image
    {
        public int id;
        public int width;
        public int height;
        public string file_name;
        public int license;
        public string flickr_url;
        public string coco_url;
        public DateTime date_captured;
    }

    [System.Serializable]
    public struct Annotation
    {
        public int id;
        public int image_id;
        public int category_id;
        public List<int[]> segmentation;
        public float area;
        public int[] bbox;
        public int iscrowd;
        public List<int> keypoints;
        public int num_keypoints;
    }

    [System.Serializable]
    public struct License
    {
        public int id;
        public string name;
        public string url;
    }

    [System.Serializable]
    public struct Category
    {
        public int id;
        public string name;
        public string supercategory;
        public List<string> keypoints;
        public List<SkeletonChain> skeleton;
    }
    
    [System.Serializable, JsonConverter(typeof(SkeletonChainJsonConverter))]
    public struct SkeletonChain
    {
        public int a, b;
    }
    
    public class SkeletonChainJsonConverter : JsonConverter<SkeletonChain>
    {
        public override void WriteJson(JsonWriter writer, SkeletonChain value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new[]{value.a, value.b});
        }

        public override SkeletonChain ReadJson(JsonReader reader, Type objectType, SkeletonChain existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var chain = serializer.Deserialize<int[]>(reader);
            if (chain == null || chain.Length != 2) return existingValue;
            
            return new SkeletonChain()
            {
                a = chain[0], b = chain[1]
            };
        }
    }
}