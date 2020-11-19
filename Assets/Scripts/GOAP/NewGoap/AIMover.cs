using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIMover
{

    //Variables
    #region Protected

    protected bool b_transformTracking;
    protected bool b_targetLooking;
    protected bool b_doMovement = true;
    protected float f_moveSpeed;
    protected float f_jumpForce;

    protected Vector3 v_moveTarget;
    protected Vector3 v_lookTarget;

    protected Transform t_moveTarget;
    protected Transform t_lookTarget;

    protected Rigidbody rb;

    #endregion

    //Methods
    #region Unity Standards


    #endregion

    #region Private Voids


    #endregion

    #region Public Voids

    public void Retarget(Transform moveTarget, bool lookAtTarget)
    {
        b_doMovement = true;
        b_transformTracking = true;
        t_moveTarget = moveTarget;

        b_targetLooking = lookAtTarget;

        t_lookTarget = (lookAtTarget ? moveTarget : null);

    }

    public void Retarget(Transform moveTarget, Transform lookTarget)
    {
        b_doMovement = true;

        b_transformTracking = true;
        t_moveTarget = moveTarget;

        b_targetLooking = false;
        t_lookTarget = lookTarget;
    }

    public void Retarget(Vector3 moveTarget, bool lookAtTarget)
    {
        b_doMovement = true;

        b_transformTracking = false;
        v_moveTarget = moveTarget;

        b_targetLooking = lookAtTarget;

        v_lookTarget = (lookAtTarget ? moveTarget : Vector3.zero);

    }

    public void Retarget(Vector3 moveTarget, Vector3 lookTarget)
    {
        b_doMovement = true;

        b_targetLooking = false;
        v_moveTarget = moveTarget;

        b_targetLooking = false;
        v_lookTarget = lookTarget;

    }

    public virtual void Move()
    {
        if (!b_doMovement)
            return;
    }

    public void SetRB(Rigidbody _rb)
    {
        rb = _rb;
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}