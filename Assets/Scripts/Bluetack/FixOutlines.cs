using Knife.HDRPOutline.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixOutlines : MonoBehaviour
{

    private OutlineObject o;

    private void Start()
    {
        o = GetComponent<OutlineObject>();
    }

    private void OnBecameInvisible()
    {
        if (o != null)
            o.enabled = false;
    }

    private void OnBecameVisible()
    {
        if (o != null)
            o.enabled = true;
    }

}
