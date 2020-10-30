using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VisualisePath : MonoBehaviour
{

    [SerializeField] private LineRenderer lr;
    [SerializeField] private Transform t;
    [SerializeField] private GoapAgent agent;

    private void Update()
    {
        DrawPath();
    }

    private void DrawPath()
    {
        if (agent.Path().corners.Length < 2)
            return;

        lr.positionCount = agent.Path().corners.Length;

        for (int i = 0; i < agent.Path().corners.Length; i++)
        {
            lr.SetPosition(i, agent.Path().corners[i]);
        }

    }
}
