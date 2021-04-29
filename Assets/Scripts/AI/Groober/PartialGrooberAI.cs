using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GrooberAI : AIBase
{
    [Header("Grouping Behaviour")]
    [SerializeField] private int i_minimumGroupSizeToAAttack = 6;
    [SerializeField] private float f_hordeRadius;
    [SerializeField] private LayerMask lm_enemyLayer;

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

    private void IsWithGroupAction()
    {
        b_inGroup = Physics.OverlapSphere(transform.position, f_hordeRadius, lm_enemyLayer, QueryTriggerInteraction.Ignore).Length >= i_minimumGroupSizeToAAttack;
    }

    private bool IsWithGroupQuery()
    {
        return b_inGroup;
    }

    #endregion

    #region Actions

    private void AttackAction()
    {
        StartCoroutine(IAttackAction());
    }

    private IEnumerator IAttackAction()
    {
        anim.SetBool("Windup", true);
        yield return new WaitForSeconds(f_attackStartup);
        anim.SetBool("Windup", false);
        anim.SetBool("attack", true);
        go_attackHitBox.SetActive(true);
        yield return new WaitForEndOfFrame();
        go_attackHitBox.SetActive(false);
        anim.SetBool("attack", false);
        f_currentTime = f_timeBetweenAttacks;
    }

    #endregion

    #region Moving

    private void MoveTowardTarget()
    {
        mover.Move((t_target.position - transform.position).normalized);
        print("MOVING TO THE TRAGEt");
    }

    private void MoveAwayFromTarget()
    {
        mover.Move((transform.position - t_target.position).normalized);
        print("MOVING AWAY FROM THE TRAGEt");
    }

    #endregion

    private void FindClosestPlayer()
    {
        float _f_distance = 1000000000;
        print("Finding closest player");

        foreach (GameObject item in TagManager.x.GetTagSet("Player"))
        {
            float _f_distanceCheck = Vector3.SqrMagnitude(item.transform.position - transform.position);
            if (_f_distanceCheck < _f_distance)
            {
                print("Found closest player");
                t_target = item.transform;
                _f_distance = _f_distanceCheck;
            }
        }
    }

}
