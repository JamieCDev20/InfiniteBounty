﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosterRotater : MonoBehaviour
{
    private int i_timesMoved;
    [SerializeField] private RectTransform rt_overallParent;
    [SerializeField] private List<RectTransform> rtL_adParents = new List<RectTransform>();

    private void Start()
    {
        InvokeRepeating("MoveAd", 5, 5);
    }

    private void MoveAd()
    {
        RectTransform _rt = rtL_adParents[0];
        rtL_adParents.RemoveAt(0);
        rtL_adParents.Add(_rt);

        for (int i = 0; i < 13; i++)
        {
            Invoke("MoveLerp", i * Time.deltaTime);
        }
        Invoke("UnMove", Time.deltaTime * 14);

        Invoke("SetNewOrder", 1);
    }

    private void MoveLerp()
    {
        rt_overallParent.localPosition -= new Vector3(0, 10, 0);
    }

    private void UnMove()
    {
        rt_overallParent.localPosition += new Vector3(0, 10, 0);
    }

    private void SetNewOrder()
    {
        rtL_adParents[rtL_adParents.Count - 1].localPosition = new Vector3(0, 120 * (rtL_adParents.Count + i_timesMoved), 0);
        i_timesMoved++;
    }


}
