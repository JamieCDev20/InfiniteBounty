using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrooberAnimator : MonoBehaviour
{

    private Animator anim;
    private Rigidbody rb;
    private bool b_isGrounded = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {

        anim.SetFloat("movblend", b_isGrounded ? rb.velocity.sqrMagnitude : 0);

        anim.SetBool("flying", !b_isGrounded);

        b_isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.35f, -1 ,QueryTriggerInteraction.Ignore);

    }

    public void Headbutt()
    {
        anim.SetBool("attack", true);
    }

    public void EndHeadbutt()
    {
        anim.SetBool("attack", false);
    }

    public void SetWindup(bool v)
    {
        anim.SetBool("Windup", v);
    }

}
