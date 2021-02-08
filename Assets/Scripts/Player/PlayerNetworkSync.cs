﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkSync : MonoBehaviourPunCallbacks, IPunObservable
{

    private Vector3 v_posVector;
    private Vector3 v_rotVector;

    private bool b_isSprinting;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        v_rotVector = transform.eulerAngles;
        v_posVector = transform.position;
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position.x);
            stream.SendNext(transform.position.y);
            stream.SendNext(transform.position.z);

            stream.SendNext(transform.eulerAngles.y);

            stream.SendNext(b_isSprinting);

        }
        else
        {
            v_posVector.x = (float)stream.ReceiveNext();
            v_posVector.y = (float)stream.ReceiveNext();
            v_posVector.z = (float)stream.ReceiveNext();

            v_rotVector.y = (float)stream.ReceiveNext();

            b_isSprinting = (bool)stream.ReceiveNext();

        }

    }

    private void Update()
    {
        if (photonView.IsMine)
            return;
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, v_rotVector, 0.3f);
        transform.position = Vector3.Lerp(transform.position, v_posVector, 0.3f);
    }

    public bool GetIsSprinting()
    {
        return b_isSprinting;
    }

}
