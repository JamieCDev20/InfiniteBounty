using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{

    //Variables
    #region Serialised

    [SerializeField] private GameObject[] goA_toSpawnOnStart;

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


        if (PhotonNetwork.IsConnectedAndReady)
            Destroy(gameObject);
        else
        {
            //PhotonNetwork.Instantiate(string.Format("{0}{1}", "NetworkPrefabs/", goA_toSpawnOnStart[0].name), Vector3.zero, Quaternion.identity);
            for (int i = 0; i < goA_toSpawnOnStart.Length; i++)
            {
                Instantiate(goA_toSpawnOnStart[i]);
            }
        }

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
