using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyPad : MonoBehaviour, IHitable
{
    [SerializeField] private int i_currentHealth = 50;
    private PhotonView view;
    private bool b_isHost;
    [SerializeField] private GameObject go_deathEffects;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        b_isHost = PhotonNetwork.IsMasterClient;
    }

    public void Die()
    {
        gameObject.SetActive(false);
        go_deathEffects.SetActive(true);
        go_deathEffects.transform.parent = null;
    }

    internal void Setup(int _i_id)
    {
        view.ViewID = 4000 + _i_id;
        PhotonNetwork.RegisterPhotonView(view);
    }

    public bool IsDead()
    {
        return false;
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        if (b_isHost)
            view.RPC("ActualTakeDamage", RpcTarget.All, damage);
    }
    [PunRPC]
    public void ActualTakeDamage(int damage)
    {
        i_currentHealth -= damage;
        if (i_currentHealth <= 0)
            Die();
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {

    }
}
