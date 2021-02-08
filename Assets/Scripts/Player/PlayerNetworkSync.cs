using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkSync : MonoBehaviourPunCallbacks, IPunObservable
{

    private Vector3 v_posVector;
    private Vector3 v_rotVector;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        v_rotVector = transform.eulerAngles;
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position.x);
            stream.SendNext(transform.position.y);
            stream.SendNext(transform.position.z);

            stream.SendNext(transform.eulerAngles.y);

        }
        else
        {
            v_posVector.x = (float)stream.ReceiveNext();
            v_posVector.y = (float)stream.ReceiveNext();
            v_posVector.z = (float)stream.ReceiveNext();

            v_rotVector.y = (float)stream.ReceiveNext();

        }

    }

    private void Update()
    {
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, v_rotVector, 0.3f);
    }

}
