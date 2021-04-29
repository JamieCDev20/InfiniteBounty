using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverBase : MonoBehaviour
{

    [SerializeField] protected float f_moveSpeed = 1.5f;
    [SerializeField] protected float f_airControlMultiplier = 0.2f;
    [SerializeField] protected Vector3 v_drag = new Vector3(0.1f, 0, 0.1f);
    [SerializeField] protected Transform t_groundCheckPoint;
    [SerializeField] protected float f_jumpForce = 10;
    [SerializeField] protected LayerMask jumpMask;

    protected float lastJumped = 0;

    protected bool b_canMove = true;
    protected bool b_grounded = false;
    protected RaycastHit hit;
    protected Vector3 v_groundNormal;
    protected Rigidbody rb;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Move(Vector3 _dir)
    {
        if (!b_canMove)
            return;
        v_groundNormal = Vector3.up;
        b_grounded = false;
        rb.velocity = Vector3.Scale(rb.velocity, Vector3.one - v_drag);
        if (Physics.Raycast(t_groundCheckPoint.position, Vector3.down, out hit, 0.25f) || !b_canMove)
        {
            v_groundNormal = hit.normal;
            b_grounded = true;
        }
        rb.AddForce(Vector3.ProjectOnPlane(_dir, v_groundNormal).normalized * f_moveSpeed * (b_grounded ? 1 : f_airControlMultiplier), ForceMode.Impulse);
        if (rb.velocity.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(rb.velocity, Vector3.one - Vector3.up), Vector3.up), 0.3f);

    }

    public void SetCanMove(bool _val)
    {
        b_canMove = _val;
    }

}
