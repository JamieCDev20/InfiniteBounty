using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System;

public class Lobby : MonoBehaviourPunCallbacks
{

    [SerializeField] private InputField if_gameTitleInput;
    [SerializeField] private InputField if_playerName;
    [SerializeField] private Scrollbar sb_bar;
    [SerializeField] private GameObject go_roomListing;
    [SerializeField] private Transform t_roomListParent;
    [SerializeField] private Transform t_camera;
    [SerializeField] private Button HostButton;
    [SerializeField] private GraphicRaycaster gr_menuRaycaster;

    private List<Listing> goL_listings = new List<Listing>(20);
    private List<RoomInfo> riL_currentRooms = new List<RoomInfo>();
    [Space, SerializeField] private int f_lobbyButtonHeight;
    [SerializeField] private int f_topmostLobbyPositionY;

    [Space, SerializeField] private Button[] bA_buttonsToSetToNonInteractableWhenHostIsClicked = new Button[0];

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
            }
        if (sb_bar != null)
            sb_bar.value = 1f;
        //PhotonNetwork.ConnectUsingSettings();
        if (HostButton != null)
            HostButton.interactable = false;
        if (PlayerPrefs.HasKey("playerName"))
            if_playerName.text = PlayerPrefs.GetString("playerName");
        if (PlayerPrefs.HasKey("roomName"))
            if_gameTitleInput.text = PlayerPrefs.GetString("roomName");
        if_gameTitleInput.characterLimit = 16;
        if_playerName.characterLimit = 16;

        for (int i = 0; i < 20; i++)
        {
            GameObject _go = Instantiate(go_roomListing, t_roomListParent);
            _go.SetActive(false);
            goL_listings.Add(_go.GetComponent<Listing>());
        }
        if(PhotonNetwork.CurrentRoom == null)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }

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
        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("joined lobby");
        StartCoroutine(DelayButtonActivate());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room!!");
        if_gameTitleInput.interactable = false;
        if_playerName.interactable = false;

        gr_menuRaycaster.enabled = false;
        for (int i = 0; i < bA_buttonsToSetToNonInteractableWhenHostIsClicked.Length; i++)
            bA_buttonsToSetToNonInteractableWhenHostIsClicked[i].enabled = false;
    }

    public void OnClickJoin()
    {
        PhotonNetwork.JoinRoom(if_gameTitleInput.text);
        t_camera.gameObject.SetActive(false);

        FindObjectOfType<ToolRack>().Init();
        SaveManager.x.CreateSaveData();

    }

    public void OnClickLeave()
    {
        PhotonNetwork.LeaveRoom();
        for (int i = 0; i < bA_buttonsToSetToNonInteractableWhenHostIsClicked.Length; i++)
            bA_buttonsToSetToNonInteractableWhenHostIsClicked[i].enabled = true;
    }

    public void OnClickCreate()
    {
        PhotonNetwork.CreateRoom(if_gameTitleInput.text, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
        t_camera.gameObject.SetActive(false);
        gr_menuRaycaster.enabled = false;

        for (int i = 0; i < bA_buttonsToSetToNonInteractableWhenHostIsClicked.Length; i++)
            bA_buttonsToSetToNonInteractableWhenHostIsClicked[i].enabled = false;

        FindObjectOfType<ToolRack>().Init();
        SaveManager.x.CreateSaveData();
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

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList || !roomList[i].IsVisible || !roomList[i].IsOpen)
            {
                if (riL_currentRooms.Contains(roomList[i]))
                {
                    riL_currentRooms.Remove(roomList[i]);
                    continue;
                }
            }
            else
            {
                if (!riL_currentRooms.Contains(roomList[i]))
                {
                    riL_currentRooms.Remove(roomList[i]);
                    riL_currentRooms.Add(roomList[i]);
                }
            }

        }

        UpdateRoomListDisplay();
    }

    private void UpdateRoomListDisplay()
    {
        for (int i = 0; i < goL_listings.Count; i++)
            goL_listings[i].gameObject.SetActive(false);

        for (int i = 0; i < riL_currentRooms.Count; i++)
        {
            goL_listings[i].gameObject.SetActive(true);
            goL_listings[i].SetInfo(riL_currentRooms[i]);
        }
    }

    private void RemoveAllNullFromList(List<Listing> list)
    {
        for (int i = list.Count - 1; i > 0; i++)
        {
            if (list[i] == null)
                list.RemoveAt(i);
        }
    }

    IEnumerator DelayButtonActivate()
    {
        yield return new WaitForSeconds(0.5f);
        if (HostButton != null)
            HostButton.interactable = true;
    }

}
