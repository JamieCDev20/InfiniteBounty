﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerLevelSpawnController : MonoBehaviour
{

    private PlayerInputManager pim;
    [SerializeField] private GameObject go_impactEffects;
    [SerializeField] private float f_fireForce = 10;
    private PhotonView view;
    private bool b_hasDid;
    [SerializeField] private float f_timeToWaitBeforeActivating;
    [SerializeField] private float f_timeToPlayParticle = 1;
    [SerializeField] private GameObject go_cameraParent;


    public void SetupPlayer(GameObject _go_playerToSetup)
    {
        go_cameraParent.SetActive(true);

        _go_playerToSetup.GetComponentInChildren<Animator>().SetTrigger("LevelStart");

        pim = _go_playerToSetup.GetComponent<PlayerInputManager>();
        pim.b_shouldPassInputs = false;
        pim.GetCamera().enabled = false;
        pim.GetCamera().transform.localEulerAngles = new Vector3(5, 0, 0);

        _go_playerToSetup.transform.position = transform.position;

        _go_playerToSetup.transform.forward = transform.forward;

        Invoke("PlayerImpact", f_timeToWaitBeforeActivating);
        Invoke("PlayParticle", f_timeToPlayParticle);
    }

    private void PlayParticle()
    {
        go_impactEffects.SetActive(true);
    }

    public void PlayerImpact()
    {
        pim.b_shouldPassInputs = true;
        pim.GetCamera().enabled = true;

        go_cameraParent.SetActive(false);
    }


}