using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class AIGroundMover : AIMover
{
    //Variables
    #region Private

    private AIMovementStats stats;

    private NavMeshPath path;

    #endregion

    //Methods
    #region Unity Standards


    #endregion

    #region Private Voids


    #endregion

    #region Public Voids

    public AIGroundMover(AIMovementStats _stats)
    {
        stats = _stats;
    }

    public override void Move()
    {
        base.Move();
        Vector3 mTarget = b_transformTracking? t_moveTarget.position : v_moveTarget;
        Vector3 lTarget = b_targetLooking ? mTarget : Vector3.zero;

        rb.AddForce((rb.position - mTarget) * stats.f_movementSpeed * Time.deltaTime, ForceMode.Impulse);
        rb.velocity = Vector3.Scale(rb.velocity, Vector3.one - stats.v_drag);

    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
