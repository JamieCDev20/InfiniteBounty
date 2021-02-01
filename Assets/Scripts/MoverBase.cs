using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverBase : MonoBehaviour
{

    [SerializeField] protected float f_moveSpeed = 5;
    [SerializeField] protected Vector3 v_drag = new Vector3(0.1f, 0, 0.1f);

    protected bool b_canMove = true;
    protected RaycastHit hit;
    protected Vector3 v_groundNormal;
    protected Rigidbody rb;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Move(Vector3 _dir)
    {
        v_groundNormal = Vector3.up;
        rb.velocity = Vector3.Scale(rb.velocity, Vector3.one - v_drag);
        if (!Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 0.5f) || !b_canMove)
            return;
        v_groundNormal = hit.normal;
        Debug.Log(Vector3.ProjectOnPlane(_dir, v_groundNormal).normalized);
        rb.AddForce(Vector3.ProjectOnPlane(_dir, v_groundNormal).normalized * f_moveSpeed, ForceMode.Impulse);

    }

    public void SetCanMove(bool _val)
    {
        b_canMove = _val;
    }

}
