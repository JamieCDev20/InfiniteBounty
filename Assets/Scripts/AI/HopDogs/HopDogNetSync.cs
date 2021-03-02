using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopDogNetSync : MonoBehaviourPunCallbacks, IPunObservable
{

    private Vector3 v_pos;
    private Vector3 v_rot;
    private float t;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        v_rot = transform.eulerAngles;

        if (stream.IsWriting)
        {
            stream.SendNext(transform.position.x);
            stream.SendNext(transform.position.y);
            stream.SendNext(transform.position.z);

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

    private void LateUpdate()
    {
        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
            return;
        transform.position = (transform.position - v_pos).sqrMagnitude > 100 ? transform.position = v_pos : Vector3.Lerp(transform.position, v_pos, 0.3f);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, v_rot, 0.3f);
    }

}
