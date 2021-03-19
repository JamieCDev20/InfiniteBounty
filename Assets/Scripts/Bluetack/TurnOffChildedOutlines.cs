using Knife.HDRPOutline.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffChildedOutlines : MonoBehaviour
{
    
    private void Start()
    {
        foreach (OutlineObject o in GetComponentsInChildren<OutlineObject>())
        {
            o.enabled = false;
        }
        foreach (FixOutlines f in GetComponentsInChildren<FixOutlines>())
        {
            f.enabled = false;
        }
    }

}
