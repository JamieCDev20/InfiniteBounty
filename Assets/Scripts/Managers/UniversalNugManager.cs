using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UniversalNugManager : MonoBehaviourPunCallbacks, IPunObservable
{

    public static UniversalNugManager x;

    private int[][] playerNugCounts = new int[][] { new int[] { 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0 } };
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
            playerNugCounts[id][i] = c;
            i++;
        }
    }

    public void SetLocal(int id)
    {
        i_localID = id;
        for (int i = 0; i < playerNugCounts.Length; i++)
        {
            playerNugCounts[i][6] = id;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(playerNugCounts[i_localID]);
        }
        else
        {
            int[] t = (int[])stream.ReceiveNext();
            playerNugCounts[t[6]] = t;
        }

    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Lobby"))
        {
            ScoreboardManager sMan = FindObjectOfType<ScoreboardManager>();
            sMan.SetValues(playerNugCounts, localNugCount);

        }
        foreach(NugManager nMan in FindObjectsOfType<NugManager>())
        {
            nMan.ResetNugCount();
        }
    }

}
