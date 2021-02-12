﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandymanHealth : MonoBehaviourPun, IHitable
{

    [SerializeField] private int i_maxHealth = 300;
    
    private int i_curHealth;

    private void OnEnable()
    {
        i_curHealth = i_maxHealth;
    }

    public void Die()
    {
        PhotonNetwork.Destroy(gameObject);
        if (photonView.IsMine)
            EnemySpawner.x?.EnemyDied();
    }

    public bool IsDead()
    {
        return true;
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            MasterTakeDamage(damage, activatesThunder);
            return;
        }
        photonView.RPC("RemoteTakeDamage", RpcTarget.Others, damage, activatesThunder);
    }

    private void MasterTakeDamage(int damage, bool activatesThunder)
    {
        i_curHealth -= damage;
        if (i_curHealth <= 0)
            Die();

    }
    
    [PunRPC]
    public void RemoteTakeDamage(int _dmg, bool _thun)
    {
        if (PhotonNetwork.IsMasterClient)
            MasterTakeDamage(_dmg, _thun);
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {

    }
}
