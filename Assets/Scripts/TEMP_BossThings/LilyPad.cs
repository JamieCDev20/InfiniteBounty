using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyPad : MonoBehaviour, IHitable
{
    private int i_currentHealth = 50;
    private PhotonView view;
    private bool b_isHost;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        b_isHost = PhotonNetwork.IsMasterClient;
    }

    public void Die()
    {
        gameObject.SetActive(false);
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
        if (i_currentHealth < 0)
            Die();
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {

    }
}
