using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCamera : MonoBehaviour
{

    public void SwitchToLobbyCam(Transform _t_newPos)
    {
        gameObject.transform.position = _t_newPos.position;
        gameObject.transform.rotation = _t_newPos.rotation;
        gameObject.SetActive(true);
        NetworkedPlayer.x.SetCameraActive(false);
    }

    public void SwitchToPlayerCam()
    {
        NetworkedPlayer.x.SetCameraActive(true);
        gameObject.SetActive(false);
    }
}
