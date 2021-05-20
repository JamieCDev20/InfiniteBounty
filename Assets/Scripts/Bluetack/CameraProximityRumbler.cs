using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraProximityRumbler : MonoBehaviour
{

    private CameraController cc_cam;
    private float f_time;


    private void Start()
    {
        cc_cam = FindObjectOfType<CameraController>();
    }

    private void Update()
    {
        f_time += Time.deltaTime;

        if (f_time > 0.1f)
        {
            CamRumble();
            f_time = 0;
        }
    }

    private void CamRumble()
    {
        float dist = transform.position.z + 1000 - cc_cam.transform.position.z;
        //print("Rumbling " + (100 - Mathf.Abs(dist)) * 0.001f);

        if (Mathf.Abs(dist) < 100)
            cc_cam.Recoil((100 - Mathf.Abs(dist)) * 0.001f, 0.1f);
    }
}
