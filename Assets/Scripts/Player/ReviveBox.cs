﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveBox : MonoBehaviour, IInteractible
{

    private PlayerHealth ph_health;

    private void Start()
    {
        ph_health = GetComponentInParent<PlayerHealth>();
    }

    public void Interacted()
    {
    }

    public void Interacted(Transform interactor)
    {
        interactor.GetComponent<PhotonView>().RPC("DoRevive", RpcTarget.All);
        ph_health.photonView.RPC("GetRevived", RpcTarget.All);
    }

}
