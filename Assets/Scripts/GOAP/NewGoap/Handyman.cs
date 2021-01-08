using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handyman : NGoapAgent
{
    [Header("Handyman Stats")]
    [SerializeField] private GameObject go_tailHitBox;
    [SerializeField] private float f_tailActiveTime = 0.7f;

    protected override void Attack()
    {
        anim.SetBool("Attack", true);
        go_tailHitBox.SetActive(true);
        Invoke("TurnTailOff", f_tailActiveTime);
    }
    private void TurnTailOff()
    {
        go_tailHitBox.SetActive(false);
    }

    protected override void FixedUpdate()
    {
        if (!mover.HasPath())
            mover.Retarget(target, true);
        anim.SetBool("Walking", rb.velocity.magnitude >= 0.1f);

        if (Vector3.Scale(rb.velocity, Vector3.one - Vector3.up).magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(Vector3.Scale(rb.velocity, Vector3.one - Vector3.up), Vector3.up);

        mover.Move();
    }

}