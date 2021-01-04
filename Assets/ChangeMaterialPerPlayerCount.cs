using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialPerPlayerCount : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] mrA_meshRenderers = new MeshRenderer[0];
    [SerializeField] private Material m_red;
    [SerializeField] private Material m_green;
    private int i_numOfPlayers;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            i_numOfPlayers++;
        UpdateLights();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            i_numOfPlayers--;
        UpdateLights();
    }

    private void UpdateLights()
    {
        for (int i = 0; i < mrA_meshRenderers.Length; i++)
        {
            mrA_meshRenderers[i].material = m_red;

            if (i_numOfPlayers > i)
                mrA_meshRenderers[i].material = m_green;
        }
    }
}
