using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Listing : MonoBehaviour
{

    [SerializeField] private Text t_titleText;
    [SerializeField] private Text t_playerCountText;
    [SerializeField] private Button button;
    private int curPlayers, maxPlayers;

    public void SetInfo(RoomInfo info)
    {
        t_titleText.text = info.Name;
        t_playerCountText.text = $"{info.PlayerCount}/{info.MaxPlayers}";
        button.interactable = info.PlayerCount < info.MaxPlayers;
    }

    public void OnClick()
    {
        PhotonNetwork.JoinRoom(t_titleText.text);

        SaveManager.x.CreateSaveData();
        FindObjectOfType<ToolRack>().Init();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

}
