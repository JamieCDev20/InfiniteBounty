using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkedPlayer : MonoBehaviourPunCallbacks, IPunObservable
{

    public static NetworkedPlayer x;

    private Vector3 v_spawnPoint;
    [SerializeField]
    private PlayerInfo playerInfo;
    private Transform t_thisPlayer;
    private PlayerInputManager playerIM;
    private NugManager nMan;
    private ToolHandler handler;
    private PhotonView view;
    private GameObject playerCamera;
    private PlayerHealth ph_health;

    public int PlayerID { get { return playerInfo.playerID; } set { playerInfo.playerID = value; } }

    private List<string> dataToSend = new List<string>();
    private int wepSync = 0;



    private void Start()
    {

        if (x == null)
        {
            x = this;
            name += " (Singleton)";
        }
        else
            Destroy(gameObject);

        //make players go to spawn points on scene loads and persist
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    public void Reset()
    {
        if (t_thisPlayer != null)
        {
            playerIM.RemoveAllPoolables();
            Destroy(t_thisPlayer.gameObject);
        }
        if (playerCamera != null)
            Destroy(playerCamera.transform.parent.gameObject);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (playerIM == null)
            return;
        //players go to spawn on scene load
        //playerIM = view.GetComponent<PlayerInputManager>();
        x.playerIM.GoToSpawnPoint();
        //playerIM.gameObject.SetActive(true);
        x.view.RPC("Respawn", RpcTarget.All);

        x.playerIM.ChangedScene(scene.name.Contains("Lobby"));
        FindObjectOfType<CameraController>().SetFollow(x.t_thisPlayer);
        t_thisPlayer?.SendMessage("FullRespawn");

        NetworkManager.x.SetCanLoad(true);
    }

    public override void OnJoinedRoom()
    {

        //Make it work <-------
        //Make it fast
        //Make it nice

        GameObject.FindObjectOfType<MainMenuController>().DisableCamera();

        //set player ID
        playerInfo.playerID = PhotonNetwork.CurrentRoom.PlayerCount - 1;

        //Initialise player info and stuff
        GameObject player = PhotonNetwork.Instantiate(playerInfo.go_playerPrefab.name, v_spawnPoint, Quaternion.identity);
        t_thisPlayer = player.transform;

        view = player.GetComponent<PhotonView>();

        handler = player.GetComponent<ToolHandler>();

        view.ObservedComponents.Add(this);


        view.RPC("SetPlayerID", RpcTarget.All, playerInfo.playerID, PhotonNetwork.NickName);
        view.RPC("JoinedRoom", RpcTarget.Others);

        //set player pos, cam and IM
        t_thisPlayer = player.transform;
        t_thisPlayer.GetComponent<PlayerNetworkSync>().SetID(PlayerID, PhotonNetwork.NickName);
        playerIM = player.GetComponent<PlayerInputManager>();
        playerIM.SetCanPickUpNugs(true);
        playerIM.SetPlayerNumber(playerInfo.playerID);
        playerIM.GoToSpawnPoint();
        ph_health = playerIM.GetComponent<PlayerHealth>();
        GameObject cam = Instantiate(playerInfo.go_camPrefab);
        playerIM.SetCamera(cam.GetComponent<CameraController>());
        playerCamera = cam.transform.GetChild(0).gameObject;
        nMan = t_thisPlayer.GetComponent<NugManager>();

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerIM.SyncNameOverNetwork();
    }

    public void SyncInfo()
    {
        //sync weapons for when new player joins
        handler.SyncToolOverNetwork();
        playerIM.SyncNameOverNetwork();
        playerIM.b_shouldPassInputs = true;
        playerIM.enabled = true;

    }

    public void Suicide()
    {
        ph_health.TakeDamage(1000000, false);
    }

    public void CollectEndLevelNugs(int value)
    {
        nMan?.CollectNugs(value, false);
    }

    public void SetCameraActive(bool val)
    {
        if (x != null)
            x.playerCamera?.SetActive(val);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
    }

    public NugManager GetLocalNugManager()
    {
        return nMan;
    }

    public Transform GetPlayer()
    {
        return t_thisPlayer;
    }

    public Camera GetCamera()
    {
        return playerCamera.GetComponent<Camera>();
    }

}