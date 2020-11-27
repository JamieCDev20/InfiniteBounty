using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Burst;

[System.Serializable]
public class AIGroundMover : AIMover
{
    //Variables
    #region Private

    private AIMovementStats stats;
    private Vector3 v_groundNormal;
    private NavMeshPath path;
    private bool b_hasPath;

    #endregion

    //Methods
    #region Unity Standards



    #endregion

    #region Private Voids

    private void GroundCheck()
    {
        LayerMask mask = ~LayerMask.GetMask("Player");
        RaycastHit hit;
        Physics.Raycast(rb.position, Vector3.down, out hit, 2, mask);
        v_groundNormal = hit.normal;
    }

    #endregion

    #region Public Voids

    public AIGroundMover(AIMovementStats _stats)
    {
        stats = _stats;
        path = new NavMeshPath();
    }

    [BurstCompile]
    public override void Move()
    {
        base.Move();
        if (t_moveTarget == null)
        {
            b_hasPath = false;
            return;
        }
        NavMesh.CalculatePath(rb.position, b_transformTracking ? t_moveTarget.position : v_moveTarget, NavMesh.AllAreas, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            b_hasPath = true;
            Vector3 mTarget = path.corners[1];
            Vector3 lTarget = b_targetLooking ? mTarget : Vector3.zero;

            rb.AddForce(Vector3.ProjectOnPlane((mTarget - rb.position), Vector3.up).normalized * stats.f_movementSpeed * Time.deltaTime, ForceMode.Impulse);
            rb.velocity = Vector3.Scale(rb.velocity, Vector3.one - stats.v_drag);
        }
        else
            b_hasPath = false;

    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns

    public NavMeshPath Path()
    {
        return path;
    }

    public bool HasPath()
    {
        return b_hasPath;
    }

    #endregion

}
