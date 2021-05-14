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
    private bool b_die;

    private float secondsToNextChange = 1;
    private float fractionDeltaStep = 0.1f;
    private float currentScale = 1;
    private float directionOfChange = -1;
    private float elapsedTimeSinceChange = 0;

    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        if (x != null)
        {
            b_die = true;
            if (x != this)
                Destroy(gameObject);
        }
        else
            x = this;
        PhotonNetwork.SerializationRate = 15;
        PhotonNetwork.SendRate = 15;
        if (!b_die)
            Init();
    }

    private void Start()
    {
        DynamicResolutionHandler.SetDynamicResScaler(SetDynamicResolutionScale, DynamicResScalePolicyType.ReturnsMinMaxLerpFactor);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Time.deltaTime > 1)
            UnityEditor.EditorApplication.isPlaying = false;
        if (Input.GetKey(KeyCode.LeftControl))
            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                ReturnToMainMenu();
                //PhotonNetwork.Disconnect();
                //Reset();
                //SceneManager.LoadScene(0);
            }
    }
#endif

    #endregion

    #region Private Voids

    public float SetDynamicResolutionScale()
    {
        elapsedTimeSinceChange += Time.deltaTime;

        if (elapsedTimeSinceChange >= secondsToNextChange)
        {
            currentScale += directionOfChange * fractionDeltaStep;

            if (currentScale <= 0 || currentScale >= 1)
            {
                directionOfChange *= -1;
            }
            elapsedTimeSinceChange = 0;
        }
        return currentScale;

    }

    /// <summary>
    /// Init function for the Game Manager, handles all the start functions
    /// </summary>
    private void Init()
    {
        //Debug.Log("Init");
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
                GameObject g = Instantiate(goA_toSpawnOnStart[i], new Vector3(0, -50, 0), Quaternion.identity);

                g.SendMessage("Init");

            }
        }

    }
    public void Reset()
    {
        InfoText.x.Reset();
        PoolManager.x.Reset();
        NetworkedPlayer.x.Reset();
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
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (!scene.name.Contains("Lobby"))
            PhotonNetwork.CurrentRoom.IsOpen = false;
        else
            if (PhotonNetwork.CurrentRoom != null)
            PhotonNetwork.CurrentRoom.IsOpen = true;

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
        InfoText.x?.OnNotify(new InfoTextEvent(newMasterClient.NickName + " is now the client"));
        //PhotonNetwork.Disconnect();
        Reset();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Reset();
        base.OnDisconnected(cause);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        InfoText.x?.OnNotify(new InfoTextEvent("You were disconnected! " + cause.ToString()));
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
