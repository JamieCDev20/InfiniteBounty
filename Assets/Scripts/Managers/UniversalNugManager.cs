﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UniversalNugManager : MonoBehaviourPunCallbacks
{

    public static UniversalNugManager x;

    private int[][] i2A_playerNugCounts = new int[][] { new int[] { 0, 0, 0, 0, 0, 0}, new int[] { 0, 0, 0, 0, 0, 0}, new int[] { 0, 0, 0, 0, 0, 0}, new int[] { 0, 0, 0, 0, 0, 0} };
    private int[] iA_totalNugCounts = new int[6];
    private int i_localID = -1;
    private int localNugCount;
    private string[] sA_names = new string[4];

    private void Start()
    {
        if (x != null)
            Destroy(gameObject);
        else
            x = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoad;
        photonView.ViewID = 99999;
        PhotonNetwork.RegisterPhotonView(photonView);
    }

    public void RecieveNugs(int id, Nug nugCollected)
    {
        if (i_localID < 0)
        {
            i_localID = id;
        }

        i2A_playerNugCounts[i_localID][(int)nugCollected.nt_type] += 1;
        localNugCount += nugCollected.i_worth;
        
        RefreshTotalNugCount();
        photonView.RPC("UpdateCount", RpcTarget.AllViaServer, i2A_playerNugCounts[i_localID], localNugCount, i_localID, PhotonNetwork.NickName);
    }

    private void RefreshTotalNugCount()
    {
        iA_totalNugCounts = new int[6];
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            int curTotal = 0;
            for (int j = 0; j < i2A_playerNugCounts[i].Length; j++)
            {
                curTotal += i2A_playerNugCounts[i][j];
                iA_totalNugCounts[j] += i2A_playerNugCounts[i][j];
            }
        }

    }

    [PunRPC]
    private void UpdateCount(int[] _playerTotal, int nugCount, int _id, string _name)
    {
        sA_names[_id] = _name;

        i2A_playerNugCounts[_id] = _playerTotal;
        RefreshTotalNugCount();
        HUDController.x.SetNugValues(iA_totalNugCounts);
    }

    private void ResetValues()
    {
        localNugCount = 0;
        i2A_playerNugCounts = new int[][] { new int[] { 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 } };
        RefreshTotalNugCount();
        HUDController.x.SetNugValues(iA_totalNugCounts);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Lobby"))
        {
            RefreshTotalNugCount();
            ScoreboardManager sMan = FindObjectOfType<ScoreboardManager>();
            sMan.SetValues(i2A_playerNugCounts, sA_names);

            ResetValues();

        }
    }

}
