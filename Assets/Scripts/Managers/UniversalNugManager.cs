﻿using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UniversalNugManager : MonoBehaviourPunCallbacks
{

    public static UniversalNugManager x;

    private int[][] i2A_playerNugCounts = new int[][] { new int[] { 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 } };
    private int[] iA_totalNugCounts = new int[6];
    private int i_localID = -1;
    private int localNugCount;
    private string[] sA_names = new string[4];
    private bool b_levelFinished;
    private int[] nugValues = new int[6] { 1, 1, 1, 2, 2, 3 };


    private void Awake()
    {
    }

    private void Start()
    {
    }

    public void Reset()
    {
        ResetValues();

    }


    public void Init()
    {
        if (x != null)
        {
            if (x != this)
                Destroy(gameObject);
        }
        else
            x = this;
        DontDestroyOnLoad(gameObject);
        photonView.ViewID = 99999;
        PhotonNetwork.RegisterPhotonView(photonView);

    }

    public void RecieveNugs(Nug nugCollected)
    {

        i2A_playerNugCounts[i_localID][(int)nugCollected.nt_type] += 1;
        localNugCount += nugCollected.i_worth;

        RefreshTotalNugCount();
        photonView.RPC(nameof(UpdateCount), RpcTarget.AllViaServer, i2A_playerNugCounts[i_localID], localNugCount, i_localID, PhotonNetwork.NickName);
    }

    public void Handshake(int id)
    {
        i_localID = id;
        RefreshTotalNugCount();
        photonView.RPC(nameof(UpdateCount), RpcTarget.AllViaServer, i2A_playerNugCounts[i_localID], localNugCount, i_localID, PhotonNetwork.NickName);
    }

    private void RefreshTotalNugCount()
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;
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
        if (i_localID >= 0)
            localNugCount = CalculateValues(i2A_playerNugCounts[i_localID]);
        else
            localNugCount = 0;

    }

    [PunRPC]
    private void UpdateCount(int[] _playerTotal, int nugCount, int _id, string _name)
    {
        sA_names[_id] = _name;

        i2A_playerNugCounts[_id] = _playerTotal;
        RefreshTotalNugCount();
        HUDController.x.SetNugValues(iA_totalNugCounts);
    }

    public void FinishedLevel()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RemoteFinished), RpcTarget.All);
    }

    [PunRPC]
    public void RemoteFinished()
    {
        Debug.Log("remotefinish");
        b_levelFinished = true;
        //PoolManager.x.Reset();
        PlayerSaveData psd = new PlayerSaveData(localNugCount, -1, -1, null, null, null, null, null, null, -1);
        SaveEvent se = new SaveEvent(psd);
        FindObjectOfType<SaveManager>()?.OnNotify(se);
        StartCoroutine(WaitThenScore());
    }

    IEnumerator WaitThenScore()
    {
        yield return new WaitForSeconds(5);
        DoScoring();
    }

    private void ResetValues()
    {
        localNugCount = 0;
        i2A_playerNugCounts = new int[][] { new int[] { 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 } };
        RefreshTotalNugCount();
        b_levelFinished = false;
        HUDController.x?.SetNugValues(iA_totalNugCounts);
    }

    public void DoScoring()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        photonView.RPC(nameof(DoRemoteScoring), RpcTarget.All);

    }

    [PunRPC]
    public void DoRemoteScoring()
    {
        if (!b_levelFinished)
            ResetValues();
        Debug.Log(b_levelFinished);
        RefreshTotalNugCount();
        ScoreboardManager sMan = FindObjectOfType<ScoreboardManager>();
        sMan?.SetValues(i2A_playerNugCounts, sA_names);
        HUDController.x?.SetBBTotal();
        ResetValues();

    }

    public int CalculateValues(int[] _Vals)
    {
        int total = 0;
        for (int i = 0; i < _Vals.Length; i++)
        {
            total += (_Vals[i] * nugValues[i]);
        }
        return total;
    }

}
