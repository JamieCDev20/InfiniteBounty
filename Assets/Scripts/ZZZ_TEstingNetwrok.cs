using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZZZ_TEstingNetwrok : MonoBehaviourPunCallbacks
{
    public static ZZZ_TEstingNetwrok x;
    [SerializeField] private string s_playerPath;

    void Start()
    {
        x = this;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Do not come in here", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(s_playerPath, transform.position, Quaternion.identity);
    }
}