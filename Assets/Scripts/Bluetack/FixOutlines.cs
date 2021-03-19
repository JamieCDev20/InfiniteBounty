using Knife.HDRPOutline.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixOutlines : MonoBehaviour
{

    private OutlineObject o;
    [SerializeField] private Color c_onColour = new Color(1, 0, 0, 1);
    [SerializeField] private Color c_offColour = new Color(1, 0, 0, 0);

    private void Start()
    {
        o = GetComponent<OutlineObject>();
    }

    private void OnBecameInvisible()
    {
        if (o != null)
        {
            //o.enabled = false;
            o.Color = c_onColour;
        }
    }

    private void OnBecameVisible()
    {
        if (o != null)
            //o.Color = c_offColour;
            o.enabled = true;
    }

    private void OnDisable()
    {
        if (o != null)
            o.enabled = false;
        else
        {
            o = GetComponent<OutlineObject>();
            o.enabled = false;
        }
    }

}
