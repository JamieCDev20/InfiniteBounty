using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadIntoLevel : MonoBehaviour, IInteractible
{

    //Variables
    #region Serialised

    [SerializeField] private string NuggetRunName = "Nugget Run";

    #endregion

    #region Private

    private int i_playersInCannon = 0;

    #endregion

    //Methods
    #region Unity Standards



    #endregion

    #region Private Voids

    private void LoadLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(NuggetRunName);
        }
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

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
