using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviourPunCallbacks
{

    [SerializeField] private Button b_onlineButton;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject menuCam;

    private void Awake()
    {
        b_onlineButton.interactable = true;
        if (PhotonNetwork.InRoom)
        {
            mainMenu.SetActive(false);
            menuCam.SetActive(false);
        }
        else
        {
            //PhotonNetwork.ConnectUsingSettings();
            mainMenu.SetActive(true);
            menuCam.SetActive(true);
        }
        foreach (PlayerInputManager pim in FindObjectsOfType<PlayerInputManager>())
        {
            pim.ResetCamFollow();
        }
        Invoke("EnableButtons", 2);
    }

    public override void OnConnectedToMaster()
    {
        EnableButtons();
    }

    public void DisableCamera()
    {
        mainMenu.SetActive(false);
        menuCam.SetActive(false);
    }

    internal void EnableButtons()
    {
        b_onlineButton.interactable = true;

    }

    public void Clicked()
    {
        menuCam.SetActive(false);
        PhotonNetwork.JoinOrCreateRoom("New Room", new RoomOptions() { MaxPlayers = 4 }, TypedLobby.Default);
        mainMenu.SetActive(false);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
