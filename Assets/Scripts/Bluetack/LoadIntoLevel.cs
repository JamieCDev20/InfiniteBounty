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

    private void LoadLevel(string levelname)
    {
        NetworkManager.x.LoadLevel(levelname);
    }

    #endregion

    #region Public Voids

    public void Interacted()
    {
    }

    public void Interacted(Transform interactor)
    {
        if (interactor.GetComponent<PlayerInputManager>().GetID() == 0)
            PhotonNetwork.LoadLevel(NuggetRunName);
    }

    public void ReturnToShip()
    {

        foreach (NugManager n in FindObjectsOfType<NugManager>())
        {
            n.EndedLevel();
        }

        LoadLevel(LobbySceneName);
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
