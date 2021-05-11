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
    [SerializeField] private Teleportal tp_portalPrefab;
    [SerializeField] private float f_portalLifeSpan;

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
        //singleton and persist
    }

    public void Reset()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
    }

    #endregion

    #region Private Voids

    public void Init()
    {

        DontDestroyOnLoad(gameObject);

        if (x != null)
            Destroy(gameObject);
        else
            x = this;

        //connect to the network and store reference to the players networked
        Connect();
        photonView.ViewID = 676869;
        PhotonNetwork.RegisterPhotonView(photonView);
        netPlayer = FindObjectOfType<NetworkedPlayer>();
    }

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
        //Debug.Log("Loading level");
        PhotonNetwork.LoadLevel(levelName);
    }

    [PunRPC]
    public void HostLevelLoad()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

    }

    public void PlayerDied()
    {
        if (!b_canLoad)
            return;
        //Debug.Log("Checking all dead");
        foreach (PlayerHealth h in FindObjectsOfType<PlayerHealth>())
        {
            if (!h.GetIsDead())
                return;
        }
        b_canLoad = false;
        foreach (PlayerHealth h in FindObjectsOfType<PlayerHealth>())
        {
            h.FullRespawn();
        }
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.ClientTimeout)
            PhotonNetwork.ReconnectAndRejoin();
        base.OnDisconnected(cause);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (!PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RequestMaxDifficulty), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RequestMaxDifficulty()
    {
        photonView.RPC(nameof(PassOutMaxDifficulty), RpcTarget.Others, DifficultyManager.x.ReturnCurrentDifficultyInt());
    }

    [PunRPC]
    private void PassOutMaxDifficulty(int _i_newDiff)
    {
        DifficultyManager.x.SetNewMaxDifficulty(_i_newDiff);
    }

    public void TeleportalSpawn(Vector3 origin, Vector3 target, Vector3 direction)
    {
        photonView.RPC(nameof(RPCSpawnPortal), RpcTarget.All, origin, target, direction);
    }

    [PunRPC]
    private void RPCSpawnPortal(Vector3 _origin, Vector3 _targ, Vector3 _dir)
    {

        Teleportal _tp_startPortal = Instantiate(tp_portalPrefab);
        Teleportal _tp_endPortal = Instantiate(tp_portalPrefab);

        _tp_startPortal.transform.position = _origin + _dir * 5 + Vector3.up;
        _tp_startPortal.transform.parent = null;
        _tp_startPortal.transform.forward = _dir;
        _tp_startPortal.Setup(f_portalLifeSpan, _tp_endPortal);

        _tp_endPortal.transform.position = _targ - _dir * 3 + Vector3.up;
        _tp_endPortal.transform.parent = null;
        _tp_endPortal.transform.forward = -_dir;
        _tp_endPortal.Setup(f_portalLifeSpan, _tp_startPortal);
    }

    #endregion

    #region Private Returns

    #endregion

    #region Public Returns

    #endregion

    internal void SetDiffDisplay(int _i)
    {
        photonView.RPC(nameof(SyncDiffSelector), RpcTarget.Others, _i);
    }

    [PunRPC]
    private void SyncDiffSelector(int _i)
    {
        foreach (DifficultySelector item in FindObjectsOfType<DifficultySelector>())
            item.SetScreenView(_i);
    }

}
