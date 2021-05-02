using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public class SIGMiscellaneousNode : SIGNode
{
    public const string MISCELLANEOUS = "Miscellaneous";
}

[System.Serializable, NodeMenuItem(MISCELLANEOUS + "/Texture Preview")]
public class TexturePreviewNode : SIGMiscellaneousNode
{
    [SerializeField, ShowAsDrawer, Input("Texture")]
    public Texture texture;

    [Output("Pass Through")] public Texture passThrough;

    protected override void Process(SIGProcessingContext context)
    {
        passThrough = texture;
    }
}
