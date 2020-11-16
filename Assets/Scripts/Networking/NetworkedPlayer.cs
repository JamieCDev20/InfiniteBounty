using Photon.Pun;
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
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        playerIM.GoToSpawnPoint();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        

    }

    private void FixedUpdate()
    {
        //PrepareSendData(NetworkDataType.pos, t_thisPlayer.position.ToString());
        //PrepareSendData(NetworkDataType.rot, t_thisPlayer.rotation.ToString());
    }

    public void PrepareSendData(NetworkDataType type, string data)
    {
        dataToSend.Add(string.Format("{1}{0}{2}{0}{3}", NetworkManager.separator, PlayerID, (int)type, data));
    }

    public override void OnJoinedRoom()
    {
        //Debug.Log("You Joined");
        playerInfo.playerID = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        //Debug.Log("You are player: " + (playerInfo.playerID + 1));
        GameObject player = PhotonNetwork.Instantiate("NetworkPrefabs/" + playerInfo.go_playerPrefab.name, v_spawnPoint, Quaternion.identity);
        t_thisPlayer = player.transform;



        PhotonView view = player.GetComponent<PhotonView>();

        handler = player.GetComponent<ToolHandler>();

        view.ObservedComponents.Add(this);

        //Make it work <-------
        //Make it fast
        //Make it nice

        NetworkManager.x.AddPlayer(view, playerInfo.playerID);

        t_thisPlayer = player.transform;
        playerIM = player.GetComponent<PlayerInputManager>();
        playerIM.SetPlayerNumber(playerInfo.playerID);
        playerIM.GoToSpawnPoint();
        playerIM.SetCamera(Instantiate(playerInfo.go_camPrefab).GetComponent<CameraController>());

        //Debug.Log("Spawned Player Prefab");

    }



}

public enum NetworkDataType
{
    pos,
    rot,
    anim,
    boole,
    skin

}