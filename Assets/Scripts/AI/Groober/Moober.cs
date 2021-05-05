using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moober : MoverBase
{

    private bool b_jumping = false;
    private GrooberAnimator anim;
    private float stillTime;

    private void Start()
    {
        anim = GetComponentInChildren<GrooberAnimator>();
        stillTime = Time.realtimeSinceStartup;
    }

    public override void Move(Vector3 _dir)
    {
        if (!b_canMove)
            return;
        rb.velocity = Vector3.Scale(rb.velocity, Vector3.one - v_drag);
        rb.AddForce(Vector3.ProjectOnPlane(_dir, Vector3.up).normalized * f_moveSpeed * (b_grounded ? 1 : f_airControlMultiplier), ForceMode.Impulse);
        if (rb.velocity.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(rb.velocity, Vector3.one - Vector3.up), Vector3.up), 0.2f);

        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), transform.forward, 1.5f, jumpMask, QueryTriggerInteraction.Ignore) && !b_jumping)
        {
            if (Time.realtimeSinceStartup - lastJumped > 1.5f)
            {
                b_jumping = true;
                anim.SetWindup(b_jumping);
                Invoke(nameof(Jump), 0.3f);
            }
        }
        if(rb.velocity.sqrMagnitude < 0.1f)
        {
            if(Time.realtimeSinceStartup - stillTime > 2)
            {
                Jump();
                stillTime = Time.realtimeSinceStartup;
            }
        }

    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * f_jumpForce, ForceMode.Impulse);
        lastJumped = Time.realtimeSinceStartup;
        b_jumping = false;
        anim.SetWindup(b_jumping);
    }

}
