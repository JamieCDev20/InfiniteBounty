using Photon.Pun;
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

    public override void OnConnected()
    {
        PhotonNetwork.Instantiate(s_playerPath, transform.position, Quaternion.identity);
    }
}