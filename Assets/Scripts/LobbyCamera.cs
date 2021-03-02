using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LobbyCamera : MonoBehaviour
{

    private void Start()
    {
        //if (PhotonNetwork.IsConnected)
        //    gameObject.SetActive(false);
    }

    public void SwitchToLobbyCam(Transform _t_newPos)
    {
        gameObject.transform.position = _t_newPos.position;
        gameObject.transform.rotation = _t_newPos.rotation;
        gameObject.SetActive(true);
    }

    public void SwitchToPlayerCam()
    {
        NetworkedPlayer.x.SetCameraActive(true);
        gameObject.SetActive(false);
    }
}
