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
    private bool b_hasPath;

    private int pathCount;
    private NavMeshPath path = new NavMeshPath();

    private int pathmasterID = 0;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        pathCount = Random.Range(0, 101);
    }

    #endregion

    #region Private Voids

    private void GroundCheck()
    {
        LayerMask mask = ~LayerMask.GetMask("Player");
        RaycastHit hit;
        Physics.Raycast(rb.position, Vector3.down, out hit, 2, mask);
        v_groundNormal = hit.normal;
    }

    private void BakePath()
    {
        if (pathCount >= 100)
        {
            NavMesh.CalculatePath(rb.position, t_moveTarget.position, NavMesh.AllAreas, path);
            pathCount = 0;
        }
        pathCount++;
    }

    #endregion

    #region Public Voids

    public AIGroundMover(AIMovementStats _stats)
    {
        stats = _stats;
    }

    public override void Move()
    {
        base.Move();
        if (t_moveTarget == null)
        {
            b_hasPath = false;
            return;
        }

        rb.AddForce((t_moveTarget.position - rb.position).normalized * stats.f_movementSpeed * Time.deltaTime, ForceMode.Impulse);
        rb.velocity = Vector3.Scale(rb.velocity, Vector3.one - stats.v_drag);

        //PathfindingMaster.x.AddToList(rb.position, v_moveTarget, out pathmasterID);
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns

    public bool HasPath()
    {
        return b_hasPath;
    }

    #endregion

}