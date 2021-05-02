using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = MENU_PATH + "/Generic Graph Processing Task")]
public class GenericGraphProcessingTask : SIGTask<SIGGraph, SIGProcessingContext>
{
    protected override SIGProcessingContext CreateContext()
    {
        return new SIGProcessingContext();
    }
}
