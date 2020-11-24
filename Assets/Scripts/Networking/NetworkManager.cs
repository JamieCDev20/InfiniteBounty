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
        //singleton and persist
        DontDestroyOnLoad(gameObject);
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
        PhotonNetwork.GameVersion = gameVersion;

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("joinedLobby!");

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
        netPlayer.SyncWeapons();
    }

    #endregion

    #region Private Returns

    #endregion

    #region Public Returns

    #endregion

}
