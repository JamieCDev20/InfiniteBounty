using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandymanAnimator : MonoBehaviour
{

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        anim.SetBool("Walking", Vector3.Scale(rb.velocity, Vector3.one - Vector3.up).sqrMagnitude > 1);
    }

    public void Slap()
    {
        anim.SetBool("Attacking", true);
        StartCoroutine(SlapOff());
    }

    IEnumerator SlapOff()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Attacking", false);
    }

}
