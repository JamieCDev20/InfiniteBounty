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
    private ToolHandler handler;

    public int PlayerID { get { return playerInfo.playerID; } set { playerInfo.playerID = value; } }

    private List<string> dataToSend = new List<string>();
    private int wepSync = 0;



    private void Start()
    {

        if (x == null)
            x = this;
        else
            Destroy(gameObject);

        //make players go to spawn points on scene loads and persist
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoad;
    }



    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        //players go to spawn on scene load
        playerIM.GoToSpawnPoint();
        playerIM.gameObject.SetActive(true);
    }

    public override void OnJoinedRoom()
    {

        //Make it work <-------
        //Make it fast
        //Make it nice

        //set player ID
        playerInfo.playerID = PhotonNetwork.CurrentRoom.PlayerCount - 1;

        //Initialise player info and stuff
        GameObject player = PhotonNetwork.Instantiate("NetworkPrefabs/" + playerInfo.go_playerPrefab.name, v_spawnPoint, Quaternion.identity);
        t_thisPlayer = player.transform;

        PhotonView view = player.GetComponent<PhotonView>();

        handler = player.GetComponent<ToolHandler>();

        view.ObservedComponents.Add(this);


        view.RPC("JoinedRoom", RpcTarget.Others);
        view.RPC("SetPlayerID", RpcTarget.All, playerInfo.playerID, PhotonNetwork.NickName);

        //set player pos, cam and IM
        t_thisPlayer = player.transform;
        playerIM = player.GetComponent<PlayerInputManager>();
        playerIM.SetCanPickUpNugs(true);
        playerIM.SetPlayerNumber(playerInfo.playerID);
        playerIM.GoToSpawnPoint();
        playerIM.SetCamera(Instantiate(playerInfo.go_camPrefab).GetComponent<CameraController>());

    }

    public void SyncWeapons()
    {
        //sync weapons for when new player joins
        handler.SyncToolOverNetwork();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
    }
}