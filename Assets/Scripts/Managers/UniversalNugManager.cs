using Photon.Pun;
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
        photonView.ViewID = 99999;
        PhotonNetwork.RegisterPhotonView(photonView);
    }

    public void RecieveNugs(int id, int[] counts, int nugCount)
    {
        if (i_localID < 0)
            i_localID = id;
        int i = 0;
        foreach (int c in counts)
        {
            i2A_playerNugCounts[id][i] = c;
            i++;
        }
        RefreshTotalNugCount();
        photonView.RPC("UpdateCount", RpcTarget.AllViaServer, i2A_playerNugCounts[i_localID], i_localID);
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

    [PunRPC]
    private void UpdateCount(int[] _playerTotals, int _id)
    {
        i2A_playerNugCounts[_id] = _playerTotals;
        RefreshTotalNugCount();
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Lobby"))
        {
            ScoreboardManager sMan = FindObjectOfType<ScoreboardManager>();
            sMan.SetValues(i2A_playerNugCounts, localNugCount);

        }
    }

}
