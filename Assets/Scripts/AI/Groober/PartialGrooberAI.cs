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
    [SerializeField] private float f_timeBetweenAttacks;
    private float f_currentTime;
    private bool b_attacking = false;
    [SerializeField] private float f_attackRange;

    private Moober mover;
    [SerializeField] private int i_damage;
    private int i_actualDamage;

    private float lastGroupCheck;


    #region Queries

    private bool IsWithinAttackRangeQuery()
    {
        if (Vector3.Distance(t_target.position, transform.position) < f_attackRange)
            return true;
        return false;
    }

    private void IsWithGroupAction()
    {
        if (Time.realtimeSinceStartup - lastGroupCheck < 0.5f)
            return;

        b_inGroup = Physics.OverlapSphere(transform.position, f_hordeRadius, lm_enemyLayer, QueryTriggerInteraction.Ignore).Length >= i_minimumGroupSizeToAAttack;
        lastGroupCheck = Time.realtimeSinceStartup;
    }

    private bool IsWithGroupQuery()
    {
        return b_inGroup;
    }

    private bool AttackOffCooldownQuery()
    {
        if (f_currentTime <= 0)
            return true;
        return false;
    }

    private bool AttackOnCooldownQuery()
    {
        if (f_currentTime <= 0)
            return false;
        return true;
    }

    #endregion

    #region Actions

    private void AttackAction()
    {
        StartCoroutine(IAttackAction());
    }

    private IEnumerator IAttackAction()
    {

        if (!b_attacking)
        {
            b_attacking = true;

            anim.Headbutt();
            yield return new WaitForSeconds(f_attackStartup);
            f_currentTime = f_timeBetweenAttacks + f_attackStartup;

            foreach (Collider item in Physics.OverlapSphere(transform.position + transform.forward, 1f))
            {
                if (item.transform == transform)
                    continue;

                IHitable hit = item.GetComponent<IHitable>();
                if (hit != null)
                    hit.TakeDamage(i_actualDamage, true);
            }

            anim.EndHeadbutt();
            b_attacking = false;
        }

    }

    #endregion

    #region Moving

    private void MoveTowardTarget()
    {
        mover.Move((AttackOnCooldownQuery() ? (transform.position - t_target.position) : (t_target.position - transform.position)).normalized);
    }

    private void MoveTowardHorde()
    {
        mover.Move((GrooberSquadManager.AverageGrooberPosition() - transform.position).normalized);
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
