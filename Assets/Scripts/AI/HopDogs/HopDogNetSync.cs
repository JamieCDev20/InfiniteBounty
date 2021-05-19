using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HopDogNetSync : MonoBehaviourPunCallbacks, IPunObservable
{

    private Vector3 v_pos;
    private Vector3 v_rot;
    private float t;
    private bool send = true;

    private void Start()
    {
        //SceneManager.sceneLoaded += OnSceneLoad;
        if (PhotonNetwork.IsMasterClient)
            send = Random.Range(0f, 1f) > 0.5f;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        v_rot = transform.eulerAngles;
        send = !send;

        if (stream.IsWriting)
        {
            if (send)
            {
                stream.SendNext(transform.position.x);
                stream.SendNext(transform.position.y);
                stream.SendNext(transform.position.z);

                stream.SendNext(v_rot.y);
            }
                
        }
        else
        {
            if(stream.Count > 0)
            {
                v_pos.x = (float)stream.ReceiveNext();
                v_pos.y = (float)stream.ReceiveNext();
                v_pos.z = (float)stream.ReceiveNext();

                v_rot.y = (float)stream.ReceiveNext();

            }
        }

    }

    private void LateUpdate()
    {
        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
            return;
        transform.position = (transform.position - v_pos).sqrMagnitude > 100 ? transform.position = v_pos : Vector3.Lerp(transform.position, v_pos, 0.3f);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, v_rot, 0.3f);
    }

    
    private void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        //GetComponent<IHitable>().Die();
    }
    
}
