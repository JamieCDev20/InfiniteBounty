using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadIntoLevel : MonoBehaviour, IInteractible
{

    //Variables
    #region Serialised

    [SerializeField] internal string levelToLoad = "LobbyScene";
    [SerializeField] private bool loadOnButtonPress = true;

    #endregion

    #region Private

    private int i_playersCount = 0;

    #endregion

    //Methods
    #region Unity Standards

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.CompareTag("Player"))
        {
            i_playersCount += 1;
            if (!loadOnButtonPress)
                CheckPlayers();
        }
        //ReturnToShip();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.CompareTag("Player"))
        {
            i_playersCount += -1;
        }
    }

    internal void SetLevelToLoad(string _s_levelName)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        levelToLoad = _s_levelName;
        LoadingScreenManager.x.SetSceneToLoad(levelToLoad);
    }

    #endregion

    #region Private Voids

    private void FuckingLoad()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        LoadLevel(levelToLoad);
    }

    private void LoadLevel(string levelname)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (PhotonNetwork.IsMasterClient)
        {
            if (levelname.Contains("Lobby"))
            {
                PhotonNetwork.CurrentRoom.IsVisible = true;
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }
            else
            {
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        i_playersCount = 0;
        if (levelname.Contains("Lobby"))
            ReturnToShip();        
        LoadingScreenManager.x.CallLoadLevel(levelname);
    }

    private void CheckPlayers()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        int cCount = i_playersCount;
        foreach (PlayerHealth ph in FindObjectsOfType<PlayerHealth>())
        {
            if (ph.GetIsDead())
                cCount += 1;
        }
        if (cCount >= FindObjectsOfType<PlayerHealth>().Length)
        {
            LoadLevel(levelToLoad);
        }

    }

    #endregion

    #region Public Voids

    public void Interacted()
    {
    }

    public void Interacted(Transform interactor)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (interactor.GetComponent<PlayerInputManager>().GetID() == 0)
            CheckPlayers();
    }

    public void ReturnToShip()
    {

        if (!PhotonNetwork.IsMasterClient)
            return;
        foreach (NugManager n in FindObjectsOfType<NugManager>())
        {
            UniversalNugManager.x.FinishedLevel();
            n.EndedLevel();
        }

        //LoadLevel(lobbySceneName);
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
