using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UniversalOverlord : MonoBehaviourPunCallbacks
{

    //Variables
    #region Serialised

    //The managers that will be spawned on start
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

    /// <summary>
    /// Init function for the Game Manager, handles all the start functions
    /// </summary>
    private void Init()
    {
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
        //GM persist through scenes
        DontDestroyOnLoad(gameObject);

        //Make sure that on return to lobby, the gm isnt duplicated
        if (PhotonNetwork.IsConnectedAndReady)
            Destroy(gameObject);
        else
        {
            //spawn all managers
            for (int i = 0; i < goA_toSpawnOnStart.Length; i++)
            {
                Instantiate(goA_toSpawnOnStart[i]);
            }
        }

    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.Contains("Lobby"))
            PhotonNetwork.CurrentRoom.IsOpen = false;
        else
            PhotonNetwork.CurrentRoom.IsOpen = true;
    }

    #endregion

    #region Public Voids

    public override void OnJoinedRoom()
    {
        //initialise the pools in pool manager when you join a room
        PoolManager.x.InitialisePools();
        UniversalNugManager.x.DoScoring();

    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
