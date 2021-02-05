using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPManager : MonoBehaviour
{
    public static PvPManager x;
    private int i_playersStillAlive;

    IEnumerator Start()
    {
        x = this;

        for (int i = 0; i < 2; i++)
            yield return new WaitForEndOfFrame();

        i_playersStillAlive = GameObject.FindGameObjectsWithTag("Player").Length;

    }

    public void PlayerDied()
    {
        i_playersStillAlive--;

        if (i_playersStillAlive < 1)
            EndGame();
    }

    private void EndGame()
    {
        PhotonNetwork.LoadLevel(0);
    }
}