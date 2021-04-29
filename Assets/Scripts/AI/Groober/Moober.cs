using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moober : MoverBase
{

    public override void Move(Vector3 _dir)
    {

        rb.velocity = Vector3.Scale(rb.velocity, Vector3.one - v_drag);
        rb.AddForce(Vector3.ProjectOnPlane(_dir, Vector3.up).normalized * f_moveSpeed * (b_grounded ? 1 : f_airControlMultiplier), ForceMode.Impulse);
        if (rb.velocity.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(rb.velocity, Vector3.one - Vector3.up), Vector3.up), 0.2f);
        LayerMask mask = ~LayerMask.NameToLayer("Player");
        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), transform.forward, 1.5f, mask))
        {
            if(Time.realtimeSinceStartup - lastJumped > 1.5f)
            {
                rb.AddForce(Vector3.up * f_jumpForce, ForceMode.Impulse);
                lastJumped = Time.realtimeSinceStartup;

            }
        }

    }

}
