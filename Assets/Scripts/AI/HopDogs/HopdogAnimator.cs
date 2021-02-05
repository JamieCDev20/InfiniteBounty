using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopdogAnimator : MonoBehaviourPun
{

    private Animator anim;
    private Rigidbody rb;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        anim.SetBool("Running", rb.velocity.sqrMagnitude > 0.05f);
        anim.SetFloat("Idle", Mathf.Sin(Time.realtimeSinceStartup) + 1);
    }

    public void SetLaunchAnims(bool _val)
    {
        photonView.RPC("AnimLaunch", RpcTarget.All, _val);
    }

    [PunRPC]
    public void AnimLaunch(bool _launched)
    {
        anim.SetBool("Jump", _launched);
        anim.SetBool("Spin", _launched);
    }

}
