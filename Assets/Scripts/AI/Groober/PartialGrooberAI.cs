using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GrooberAI : AIBase
{
    [Header("Grouping Behaviour")]
    [SerializeField] private int i_minimumGroupSizeToAAttack = 6;
    private int i_currentGroupSize;

    [Header("Attack")]
    [SerializeField] private float f_attackStartup;
    [SerializeField] private GameObject go_attackHitBox;
    [SerializeField] private float f_timeBetweenAttacks;
    private float f_currentTime;
    [SerializeField] private float f_attackRange;

    private HandymanMover mover;

    #region Queries

    private bool CanAttackQuery()
    {
        f_currentTime -= Time.deltaTime;

        if (f_currentTime <= 0)
            if (Vector3.Distance(t_target.position, transform.position) < f_attackRange)
                return true;

        return false;
    }

    private bool IsWithGroupQuery()
    {
        return i_currentGroupSize >= i_minimumGroupSizeToAAttack;
    }

    #endregion

    #region Actions

    private void AttackAction()
    {
        StartCoroutine(IAttackAction());
    }

    private IEnumerator IAttackAction()
    {
        yield return new WaitForSeconds(f_attackStartup);
        go_attackHitBox.SetActive(true);
        yield return new WaitForEndOfFrame();
        go_attackHitBox.SetActive(false);
        f_currentTime = f_timeBetweenAttacks;
    }

    #endregion

    #region Moving

    private void MoveTowardTarget()
    {
        mover.Move((t_target.position - transform.position).normalized);
    }

    private void MoveAwayFromTarget()
    {
        mover.Move((transform.position - t_target.position).normalized);

    }

    #endregion

    private void FindClosestPlayer()
    {
        float _f_distance = 1000000000;

        foreach (GameObject item in TagManager.x.GetTagSet("Player"))
        {
            float _f_distanceCheck = Vector3.SqrMagnitude(item.transform.position - transform.position);
            if (_f_distanceCheck < _f_distance)
            {
                t_target = item.transform;
                _f_distance = _f_distanceCheck;
            }
        }
    }

}
