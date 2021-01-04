using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class Lobby : MonoBehaviourPunCallbacks
{

    [SerializeField] private InputField if_gameTitleInput;
    [SerializeField] private InputField if_playerName;
    [SerializeField] private Scrollbar sb_bar;
    [SerializeField] private GameObject go_roomListing;
    [SerializeField] private Transform t_roomListParent;
    [SerializeField] private Transform t_camera;
    [SerializeField] private Button HostButton;

    private List<Listing> goL_listings = new List<Listing>();

    private void Start()
    {
        sb_bar.value = 1;
        //PhotonNetwork.ConnectUsingSettings();
        HostButton.interactable = false;
        if (PlayerPrefs.HasKey("playerName"))
            if_playerName.text = PlayerPrefs.GetString("playerName");
        if (PlayerPrefs.HasKey("roomName"))
            if_gameTitleInput.text = PlayerPrefs.GetString("roomName");
    }

    public void OnRoomNameChange()
    {
        PlayerPrefs.SetString("roomName", if_gameTitleInput.text);
    }

    public void OnPlayerNameChange()
    {
        PlayerPrefs.SetString("playerName", if_playerName.text);
        PhotonNetwork.NickName = if_playerName.text;

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");
        HostButton.interactable = true;
        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() 
    {
        Debug.Log("joined lobby");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room!!");
        if_gameTitleInput.interactable = false;
        if_playerName.interactable = false;
    }

    public void OnClickJoin()
    {
        PhotonNetwork.JoinRoom(if_gameTitleInput.text);
        t_camera.gameObject.SetActive(false);
    }

    public void OnClickLeave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnClickCreate()
    {
        PhotonNetwork.CreateRoom(if_gameTitleInput.text, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
        t_camera.gameObject.SetActive(false);
    }
    
    public void OnClickQuit()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log("room list update : " + roomList.Count);
        UpdateRoomList(roomList);

    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {

        if (roomList.Count > goL_listings.Count)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].IsVisible)
                {
                    if (i < goL_listings.Count)
                    {
                        goL_listings[i].SetInfo(roomList[i]);
                    }
                    else
                    {
                        GameObject l = Instantiate(go_roomListing, t_roomListParent);
                        goL_listings.Add(l.GetComponent<Listing>());
                        goL_listings[goL_listings.Count - 1].SetInfo(roomList[i]);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < goL_listings.Count; i++)
            {
                if (i < roomList.Count)
                {
                    if (roomList[i].IsVisible)
                        goL_listings[i].SetInfo(roomList[i]);
                    else
                    {
                        goL_listings[i].Destroy();
                        goL_listings[i] = null;
                    }
                }
                else
                {
                    goL_listings[i].Destroy();
                    goL_listings[i] = null;
                }
            }
        }

        RemoveAllNullFromList(goL_listings);
    }

    private void RemoveAllNullFromList(List<Listing> list)
    {
        for (int i = list.Count - 1; i > 0; i++)
        {
            if (list[i] == null)
                list.RemoveAt(i);
        }
    }

}
