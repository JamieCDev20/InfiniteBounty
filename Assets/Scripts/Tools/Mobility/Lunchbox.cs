﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Lunchbox : MobilityTool
{
    [Header("Lunchbox Stats")]
    [SerializeField] private GameObject go_sandwichPrefab;
    private float f_coolDown;
    [SerializeField] private Vector3 v_lidOpenRotation;
    [SerializeField] private GameObject go_lidObject;
    private bool b_isOpen;

    private void Update()
    {
        if (b_isOpen)
        {
            f_coolDown -= Time.deltaTime;
            if (f_coolDown < 0) CloseLid();
        }
    }

    private void CloseLid()
    {
        go_lidObject.transform.localEulerAngles = Vector3.zero;
        b_isOpen = false;
    }

    public override void Use()
    {
        base.Use();

        for (int i = 0; i < PhotonNetwork.CountOfPlayers; i++)
        {
            GameObject _go_sandwich = Instantiate(go_sandwichPrefab, transform.position, Quaternion.identity);
            _go_sandwich.GetComponent<Rigidbody>().AddForce(transform.forward + new Vector3(0, -60 + (i * 30), 0), ForceMode.Impulse);
        }
        go_lidObject.transform.localEulerAngles = v_lidOpenRotation;

        f_coolDown = f_timeBetweenUsage;
        b_isOpen = true;
    }

}
