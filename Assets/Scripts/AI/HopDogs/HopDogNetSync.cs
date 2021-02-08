﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopDogNetSync : MonoBehaviourPunCallbacks, IPunObservable
{

    private Vector3 v_pos;
    private Vector3 v_rot;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        v_pos = transform.position;
        v_rot = transform.eulerAngles;
        if (stream.IsWriting)
        {
            stream.SendNext(v_pos.x);
            stream.SendNext(v_pos.y);
            stream.SendNext(v_pos.z);

            stream.SendNext(v_rot.y);
        }
        else
        {
            v_pos.x = (float)stream.ReceiveNext();
            v_pos.y = (float)stream.ReceiveNext();
            v_pos.z = (float)stream.ReceiveNext();

            v_rot.y = (float)stream.ReceiveNext();
        }

    }

    private void Update()
    {
        if (photonView.IsMine)
            return;
        transform.position = Vector3.Lerp(transform.position, v_pos, 0.3f);
        transform.eulerAngles = Vector3.Lerp(transform.position, v_rot, 0.3f);
    }

}
