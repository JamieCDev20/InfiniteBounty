﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UniversalNugManager : MonoBehaviourPunCallbacks, IPunObservable
{

    public static UniversalNugManager x;

    private int[][] i2A_playerNugCounts = new int[][] { new int[] { 0, 0, 0, 0, 0, 0}, new int[] { 0, 0, 0, 0, 0, 0}, new int[] { 0, 0, 0, 0, 0, 0}, new int[] { 0, 0, 0, 0, 0, 0} };
    private int[] iA_totalNugCounts;
    private int i_localID = -1;
    private int localNugCount;

    private void Start()
    {
        if (x != null)
            Destroy(gameObject);
        else
            x = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    public void RecieveNugs(int id, Dictionary<NugType, int>.ValueCollection counts, int nugCount)
    {
        if (i_localID < 0)
        {
            SetLocal(id);
        }
        int i = 0;
        foreach (int c in counts)
        {
            i2A_playerNugCounts[id][i] = c;
            i++;
        }
        RefreshTotalNugCount();
    }

    public void SetLocal(int id)
    {
        i_localID = id;
        for (int i = 0; i < i2A_playerNugCounts.Length; i++)
        {
            i2A_playerNugCounts[i][6] = id;
        }
    }

    private void RefreshTotalNugCount()
    {
        iA_totalNugCounts = new int[6];
        for (int i = 0; i < i2A_playerNugCounts.Length; i++)
        {
            for (int j = 0; j < i2A_playerNugCounts[i].Length; j++)
            {
                iA_totalNugCounts[j] += i2A_playerNugCounts[i][j];
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(i2A_playerNugCounts[i_localID]);
            stream.SendNext(iA_totalNugCounts);
        }
        else
        {
            int[] t = (int[])stream.ReceiveNext();
            i2A_playerNugCounts[t[6]] = t;
            iA_totalNugCounts = (int[])stream.ReceiveNext();
        }

    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Lobby"))
        {
            ScoreboardManager sMan = FindObjectOfType<ScoreboardManager>();
            sMan.SetValues(i2A_playerNugCounts, localNugCount);

        }
        foreach(NugManager nMan in FindObjectsOfType<NugManager>())
        {
            nMan.ResetNugCount();
        }
    }

}
