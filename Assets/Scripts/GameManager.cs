using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{

    //Variables
    #region Serialised


    #endregion

    #region Private


    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        Init();
    }

    #endregion

    #region Private Voids

    private void Init()
    {
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Public Voids

    public override void OnJoinedRoom()
    {

        PoolManager.x.InitialisePools();

    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
