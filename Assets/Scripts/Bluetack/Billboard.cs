using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera c_cam;

    private void Start()
    {
        c_cam = Camera.main;

        /*
        if (GetComponentInParent<PlayerInputManager>().GetID() == NetworkedPlayer.x.PlayerID)
        {
            print(GetComponentInParent<PlayerInputManager>().GetID() + "/" + NetworkedPlayer.x.PlayerID);
            gameObject.SetActive(false);
        }
        */
    }

    void Update()
    {
        transform.LookAt(c_cam.transform);
    }
}
