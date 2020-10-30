using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //Variables
    public static NetworkManager x;

    #region Serialised

    [SerializeField] private byte maxPlayersPerRoom = 4;

    #endregion

    #region Private

    private string gameVersion = "1";

    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Connect();
    }

    #endregion

    #region Private Voids


    #endregion

    #region Public Voids

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the fucking masta");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Lol you have no friends, Let's see if they come to you");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("YAY! You're in a room");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("The banhammer has spoken");
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
