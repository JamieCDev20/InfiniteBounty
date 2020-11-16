using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadIntoLevel : MonoBehaviour, IInteractible
{

    //Variables
    #region Serialised

    [SerializeField] private string LobbySceneName = "LobbyScene";
    [SerializeField] private string NuggetRunName = "Nugget Run";

    #endregion

    #region Private

    private int i_playersInCannon = 0;

    #endregion

    //Methods
    #region Unity Standards

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ReturnToShip();
    }

    #endregion

    #region Private Voids

    private void LoadLevel()
    {
        PhotonNetwork.LoadLevel(NuggetRunName);
    }

    #endregion

    #region Public Voids

    public void Interacted()
    {
        LoadLevel();
    }

    public void Interacted(Transform interactor)
    {



    }

    public void ReturnToShip()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(LobbySceneName);
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
