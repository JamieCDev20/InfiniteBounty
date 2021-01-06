using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnObjectWhenAroundPlayers : MonoBehaviour
{

    [SerializeField] private float f_radius;
    [SerializeField] private GameObject go_screenObject;

    private void OnTriggerEnter(Collider other)
    {
        CheckForPlayers();
    }

    private void OnTriggerExit(Collider other)
    {
        CheckForPlayers();
    }


    private void CheckForPlayers()
    {
        Collider[] _cA = Physics.OverlapSphere(transform.position, f_radius);
        for (int i = 0; i < _cA.Length; i++)
            if (_cA[i].tag == "Player")
            {
                EnableScreen();
                return;
            }

        DisableScreen();
    }

    private void EnableScreen()
    {
        go_screenObject.SetActive(true);
    }
    private void DisableScreen()
    {
        go_screenObject.SetActive(false);
    }
}
