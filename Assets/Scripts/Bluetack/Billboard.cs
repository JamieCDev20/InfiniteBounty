using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera c_cam;
    /*
    private IEnumerator Start()
    {

        
        if (GetComponentInParent<PlayerInputManager>().GetID() == NetworkedPlayer.x.PlayerID)
        {
            print(GetComponentInParent<PlayerInputManager>().GetID() + "/" + NetworkedPlayer.x.PlayerID);
            gameObject.SetActive(false);
        }
        yield return new WaitForEndOfFrame();
        c_cam = Camera.main;
    }
    */
    void Update()
    {
        if (c_cam == null)
            c_cam = FindObjectOfType<CameraController>()?.ReturnCamera();
        transform.LookAt(c_cam?.transform);
    }
}
