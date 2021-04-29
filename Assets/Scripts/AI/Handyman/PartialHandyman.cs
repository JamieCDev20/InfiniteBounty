using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HandymanAI : AIBase
{
    private HandymanMover mover;

    [Header("Punch Attack")]
    [SerializeField] private float f_punchRange;
    private bool b_shouldBeThrowing;

    [Header("Throwing Stats")]
    [SerializeField] private float f_pickupRange;
    private GameObject go_nearestThrowable;
    private bool b_hasThrowable;
    [SerializeField] private float f_minThrowDistance;



    #region Queries

    private bool IsInPunchRangeQuery()
    {
        return Vector3.Distance(transform.position, t_target.position) <= f_punchRange;
    }

    private bool IsInPickUpRangeQuery()
    {
        return Vector3.Distance(transform.position, go_nearestThrowable.transform.position) <= f_pickupRange;
    }

    private bool HasThrowableObjectQuery()
    {
        return b_hasThrowable;
    }

    private bool ShouldBeThrowingQuery()
    {
        return b_shouldBeThrowing;
    }

    #endregion

    #region Check Actions

    private GameObject GetClosestTaggedObjectAction(string _s_tag)
    {
        float _f_distance = 1000000000;
        GameObject go_object = null;

        foreach (GameObject item in TagManager.x.GetTagSet(_s_tag))
        {
            float _f_distanceCheck = Vector3.SqrMagnitude(item.transform.position - transform.position);
            if (_f_distanceCheck < _f_distance)
            {
                go_object = item;
                _f_distance = _f_distanceCheck;
            }
        }

        return go_object;
    }

    private void GetClosestPlayerAction()
    {
        t_target = GetClosestTaggedObjectAction("Player").transform;
    }

    private void GetClosestThrowableObjectAction()
    {
        go_nearestThrowable = GetClosestTaggedObjectAction("Throwable");
    }

    private void CheckShouldBeThrowing()
    {
        b_shouldBeThrowing = Vector3.Distance(transform.position, t_target.position) >= f_minThrowDistance;
    }


    #endregion

    #region Actions

    private void MoveTowardsAction()
    {
        if (b_shouldBeThrowing)
            mover.Move((go_nearestThrowable.transform.position - transform.position).normalized);
        else
            mover.Move((t_target.position - transform.position).normalized);
    }

    private void ThrowAction()
    {

    }

    private void PunchAction()
    {

    }

    #endregion

}