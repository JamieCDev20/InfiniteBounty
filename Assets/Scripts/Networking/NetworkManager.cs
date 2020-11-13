using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks, IPunObservable
{
    //Variables
    public static NetworkManager x;
    public static char separator = '|';

    #region Serialised

    [SerializeField] private byte maxPlayersPerRoom = 4;
    [SerializeField] private string roomName;
    [SerializeField] private GameObject UI;
    [SerializeField] private Button PlayButton;

    #endregion

    #region Private

    private string gameVersion = "0.1";
    private NetworkedPlayer[] networkedPlayers;

    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        networkedPlayers = FindObjectsOfType<NetworkedPlayer>();
        Connect();
    }

    #endregion

    #region Private Voids

    private void FindNetworkPlayers()
    {



    }

    #endregion

    #region Public Voids

    public void Connect()
    {

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;

    }

    public void CreateRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, TypedLobby.Default);
        UI.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        //Debug.Log("Connected to the fucking masta");
        PlayButton.interactable = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log("Lol you have no friends, Let's see if they come to you");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnCreatedRoom()
    {
        //Debug.Log("You da man!");
    }

    public override void OnJoinedRoom()
    {
        //Debug.Log("YAY! You're in a room");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //Debug.LogWarning("The banhammer has spoken");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        object[] currentStream;

        while (stream.Count > 0)
        {

            currentStream = BreakdownText(stream.ReceiveNext().ToString());
            //do something to someone

        }

    }

    #endregion

    #region Private Returns

    private object[] BreakdownText(string text)
    {
        return text.Split(separator);
    }

    #endregion

    #region Public Returns


    #endregion

}
