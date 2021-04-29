﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrooberSquadManager : MonoBehaviour
{

    private static GrooberSquadManager x;

    private Vector3 avgPos;

    private void Start()
    {
        x = this;
        StartCoroutine(CalculatePos());
    }

    IEnumerator CalculatePos()
    {
        Vector3 avg = Vector3.zero;
        while(gameObject != null)
        {
            avg = Vector3.zero;
            int i = 0;
            foreach (GameObject g in TagManager.x.GetTagSet("Groober"))
            {
                avg += g.transform.position;
                i++;
            }

            avg /= i;

            avgPos = avg;

            yield return new WaitForSeconds(0.5f);

        }
    }

    public static Vector3 AverageGrooberPosition()
    {
        return x.avgPos;
    }

}
