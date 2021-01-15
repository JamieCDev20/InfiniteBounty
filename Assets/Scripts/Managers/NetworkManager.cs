using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //Variables
    public static NetworkManager x;
    public static char separator = '|';

    #region Serialised

    [SerializeField] private byte maxPlayersPerRoom = 4;
    [SerializeField] private string roomName;
    [SerializeField] private GameObject UI;

    #endregion

    #region Private

    private string gameVersion = "0.1";
    private bool b_canLoad = true;
    private NetworkedPlayer netPlayer;

    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        //sync the scene for anyone joining the game
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {

        Debug.Log("Recreated");

        //singleton and persist
        DontDestroyOnLoad(gameObject);

        if (x != null)
            Destroy(gameObject);
        else
            x = this;

        //connect to the network and store reference to the players networked
        Connect();
        netPlayer = FindObjectOfType<NetworkedPlayer>();
    }

    #endregion

    #region Private Voids

    #endregion

    #region Public Voids

    public void Connect()
    {

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("eu");
        PhotonNetwork.GameVersion = gameVersion;

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);

    }

    public void CreateRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, TypedLobby.Default);

        UI.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public void TellClientToSync()
    {
        netPlayer.SyncInfo();
    }

    public void LoadLevel(string levelName)
    {
        Debug.Log("Loading level");
        PhotonNetwork.LoadLevel(levelName);
    }

    [PunRPC]
    public void HostLevelLoad(string _name)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

    }

    public void PlayerDied()
    {
        if (!b_canLoad)
            return;
        Debug.Log("Checking all dead");
        foreach (PlayerHealth h in FindObjectsOfType<PlayerHealth>())
        {
            if (!h.GetIsDead())
                return;
        }
        b_canLoad = false;
        LoadLevel("LobbyScene");
    }

    public void SetCanLoad(bool _val)
    {
        b_canLoad = _val;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

    }

    #endregion

    #region Private Returns

    #endregion

    #region Public Returns

    #endregion

}
