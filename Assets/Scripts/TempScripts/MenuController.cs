using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController x;

    [SerializeField] private Transform t_startCamPos;
    [SerializeField] private Transform t_targetCamPos;
    [SerializeField] private Vector3 v_finalCamLocalPos;
    [Space, SerializeField] private PlayerController pc_player;
    [SerializeField] private PlayerMover pm_player;
    [SerializeField] private Canvas c_playerCanvas;
    private int i_camLerps;

    void Awake()
    {
        x = this;
        if(pc_player != null)
            pc_player.enabled = false;
        else
            pm_player.enabled = false;
        c_playerCanvas.enabled = false;
        Camera.main.transform.position = t_startCamPos.position;
        Camera.main.transform.localEulerAngles = t_startCamPos.localEulerAngles;

    }

    public void ClickedPlay()
    {
        Invoke("CamLerp", Time.deltaTime);
    }

    private void CamLerp()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, t_targetCamPos.transform.position, 0.5f);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, t_targetCamPos.transform.rotation, 0.5f);

        i_camLerps++;
        if (i_camLerps < 10)
            Invoke("CamLerp", Time.deltaTime);
        else
        {
            EnablePlayer();
        }
    }

    private void EnablePlayer()
    {
        if(pm_player != null)
        {
            Camera.main.transform.localPosition = v_finalCamLocalPos;
            Camera.main.transform.localEulerAngles = Vector3.zero;

            pm_player.enabled = true;
        }
        else
        {
            Camera.main.transform.localPosition = v_finalCamLocalPos;
            Camera.main.transform.localEulerAngles = Vector3.zero;

            pc_player.enabled = true;
            c_playerCanvas.enabled = true;
        }

    }
}
