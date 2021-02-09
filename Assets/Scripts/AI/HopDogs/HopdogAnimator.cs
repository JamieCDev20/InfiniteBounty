using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopdogAnimator : MonoBehaviourPun
{

    private Animator anim;
    private Rigidbody rb;
    private bool b_isGrounded = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        anim.SetBool("Running", b_isGrounded ? rb.velocity.sqrMagnitude > 0.05f : false);
        anim.SetFloat("Idle", Mathf.Sin(Time.realtimeSinceStartup) + 1);
        if (!b_isGrounded)
        {
            anim.SetBool("Jump", true);
            anim.SetBool("Spin", true);
        }
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

    internal void SetGrounded(bool _b_grounded)
    {
        b_isGrounded = _b_grounded;
    }
}
