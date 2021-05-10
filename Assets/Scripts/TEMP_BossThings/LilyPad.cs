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

    [Header("Boss Things")]
    [SerializeField] private BossHealth bh_ifArmourUseThis;
    private int i_armourID;

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
        if (bh_ifArmourUseThis == null)
        {
            if (view != null)
            {
                view.ViewID = 5000 + _i_id;
                PhotonNetwork.RegisterPhotonView(view);
            }
        }
        else
        {
            i_armourID = _i_id;
        }
    }

    public bool IsDead()
    {
        return false;
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        if (bh_ifArmourUseThis == null)
        {
            if (b_isHost)
                view.RPC(nameof(TakeDamageRPC), RpcTarget.All, damage);
        }
        else
        {
            if (b_isHost)
                bh_ifArmourUseThis.ArmourDamaged(i_armourID, damage);
        }
    }
    [PunRPC]
    public void TakeDamageRPC(int damage)
    {
        i_currentHealth -= damage;
        if (i_currentHealth <= 0)
            Die();
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay) { }
}
