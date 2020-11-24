using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class Lobby : MonoBehaviourPunCallbacks
{

    [SerializeField] private InputField if_gameTitleInput;
    [SerializeField] private Scrollbar sb_bar;
    [SerializeField] private GameObject go_roomListing;
    [SerializeField] private Transform t_roomListParent;
    
    private List<Listing> goL_listings = new List<Listing>();

    private void Start()
    {
        sb_bar.value = 0;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("joined lobby");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room!!");
    }

    public void OnClickJoin()
    {
        PhotonNetwork.JoinRoom(if_gameTitleInput.text);

    }

    public void OnClickLeave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnClickCreate()
    {
        PhotonNetwork.CreateRoom(if_gameTitleInput.text, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
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
