using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Listing : MonoBehaviour
{

    [SerializeField] private Text t_titleText;
    [SerializeField] private Text t_playerCountText;

    public void SetInfo(RoomInfo info)
    {
        t_titleText.text = info.Name;
        t_playerCountText.text = $"{info.PlayerCount}/{info.MaxPlayers}";
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

}
