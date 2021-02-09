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
        if (other.CompareTag("Player"))
        {
            i_playersCount += -1;
        }
    }

    internal void SetLevelToLoad(string _s_levelName)
    {
        levelToLoad = _s_levelName;
        LoadingScreenManager.x.SetSceneToLoad(levelToLoad);
    }

    #endregion

    #region Private Voids

    private void LoadLevel(string levelname)
    {
        i_playersCount = 0;
        if (levelname.Contains("Lobby"))
            ReturnToShip();
        LoadingScreenManager.x.SetSceneToLoad(levelname);
        LoadingScreenManager.x.CallLoadLevel();
    }

    private void CheckPlayers()
    {
        int cCount = i_playersCount;
        foreach (PlayerHealth ph in FindObjectsOfType<PlayerHealth>())
        {
            if (ph.GetIsDead())
                cCount += 1;
        }
        if (cCount >= PhotonNetwork.CurrentRoom.PlayerCount)
            LoadLevel(levelToLoad);

    }

    #endregion

    #region Public Voids

    public void Interacted()
    {
    }

    public void Interacted(Transform interactor)
    {
        if (interactor.GetComponent<PlayerInputManager>().GetID() == 0)
            CheckPlayers();
    }

    public void ReturnToShip()
    {

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
