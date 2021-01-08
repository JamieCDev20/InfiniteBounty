using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadIntoLevel : MonoBehaviour, IInteractible
{

    //Variables
    #region Serialised

    [SerializeField] private string levelToLoad = "LobbyScene";
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

    #endregion

    #region Private Voids

    private void LoadLevel(string levelname)
    {
        i_playersCount = 0;
        if (levelname.Contains("Lobby"))
            ReturnToShip();
        NetworkManager.x.LoadLevel(levelname);
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
