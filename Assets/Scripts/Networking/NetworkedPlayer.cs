﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkedPlayer : MonoBehaviourPunCallbacks, IPunObservable
{

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
        //make players go to spawn points on scene loads and persist
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        //players go to spawn on scene load
        playerIM.GoToSpawnPoint();
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

        //set player pos, cam and IM
        t_thisPlayer = player.transform;
        playerIM = player.GetComponent<PlayerInputManager>();
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
        throw new System.NotImplementedException();
    }
}