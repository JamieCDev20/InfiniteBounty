using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class UniversalOverlord : MonoBehaviourPunCallbacks
{

    public static UniversalOverlord x;

    //Variables
    #region Serialised

    //The managers that will be spawned on start
    [SerializeField] private GameObject[] goA_toSpawnOnStart;

    #endregion

    #region Private

    private bool canLoadScene = true;

    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        if (x != null)
            Destroy(gameObject);
        else
            x = this;
        Init();
        PhotonNetwork.SerializationRate = 15;
        PhotonNetwork.SendRate = 15;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                PhotonNetwork.Disconnect();
                Reset();
                SceneManager.LoadScene(0);
            }
    }

    #endregion

    #region Private Voids

    /// <summary>
    /// Init function for the Game Manager, handles all the start functions
    /// </summary>
    private void Init()
    {

        canLoadScene = true;
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
        SceneManager.sceneLoaded += OnSceneLoad;
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
                Instantiate(goA_toSpawnOnStart[i], new Vector3(0, -20, 0), Quaternion.identity).SendMessage("Init");
            }
        }

    }
    public void Reset()
    {
        NetworkedPlayer.x.Reset();
        PoolManager.x.Reset();
        UniversalNugManager.x.Reset();
        TagManager.x.Reset();
        NetworkManager.x.Reset();
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        //SceneManager.LoadScene(0);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (!canLoadScene)
            return;
        Debug.Log("Left Room");
        SceneManager.LoadScene(0);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        //if (!scene.name.Contains("Lobby"))
        //    PhotonNetwork.CurrentRoom.IsOpen = false;
        //else
        //    PhotonNetwork.CurrentRoom.IsOpen = true;

        //PoolManager.x.ResetPool("BoomNug");
        //PoolManager.x.ResetPool("GooNug");
        //PoolManager.x.ResetPool("HydroNug");
        //PoolManager.x.ResetPool("MagmaNug");
        //PoolManager.x.ResetPool("TastyNug");
        //PoolManager.x.ResetPool("ThunderNug");
        //PoolManager.x.ResetPool("TEnemyProjectile");

    }

    #endregion

    #region Public Voids

    public void CantLoadScene()
    {
        canLoadScene = false;
    }

    public override void OnJoinedRoom()
    {
        //initialise the pools in pool manager when you join a room
        PoolManager.x.InitialisePools();
        UniversalNugManager.x.DoScoring();
        AugmentManager.x.JoinedRoom();

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        base.OnMasterClientSwitched(newMasterClient);
        PhotonNetwork.Disconnect();
        Reset();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Reset();
        base.OnDisconnected(cause);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ReturnToMainMenu()
    {
        Reset();
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
