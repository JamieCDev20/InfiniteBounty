using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkSync : MonoBehaviourPunCallbacks, IPunObservable
{

    private Vector3 v_posVector;
    private Vector3 v_rotVector;

    private bool b_isSprinting;
    private bool b_isGrounded;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        v_rotVector = transform.eulerAngles;
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
        transform.position = (transform.position - v_posVector).sqrMagnitude > 100 ? transform.position = v_posVector : Vector3.Lerp(transform.position, v_posVector, 0.3f);
    }

    public bool GetIsSprinting()
    {
        return b_isSprinting;
    }

    public void SetIsSprinting(bool _b_val)
    {
        b_isSprinting = _b_val;
    }

    public bool GetIsGrounded()
    {
        return b_isGrounded;
    }

    public void SetIsGrounded(bool _b_val)
    {
        b_isGrounded = _b_val;
    }

}
