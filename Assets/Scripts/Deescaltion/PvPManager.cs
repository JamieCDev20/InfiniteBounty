using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPManager : MonoBehaviour
{
    public static PvPManager x;
    private int i_playersStillAlive;

    private void Start()
    {
        x = this;
        i_playersStillAlive = PhotonNetwork.CurrentRoom.PlayerCount;
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