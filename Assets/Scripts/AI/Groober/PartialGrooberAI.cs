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
    [SerializeField] private HandymanHurtbox hhb_attackHitBox;
    [SerializeField] private float f_timeBetweenAttacks;
    private float f_currentTime;
    [SerializeField] private float f_attackRange;

    private Moober mover;




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
        f_currentTime = f_timeBetweenAttacks + f_attackStartup;

        anim.SetBool("attack", true);
        yield return new WaitForSeconds(f_attackStartup);
        hhb_attackHitBox.gameObject.SetActive(true);
        hhb_attackHitBox.SetHurtboxActive(true);
        yield return new WaitForEndOfFrame();
        hhb_attackHitBox.gameObject.SetActive(false);
        hhb_attackHitBox.SetHurtboxActive(false);
        anim.SetBool("attack", false);
    }

    #endregion

    #region Moving

    private void MoveTowardTarget()
    {
        mover.Move((t_target.position - transform.position).normalized);
    }

    private void MoveAwayFromTarget()
    {
        mover.Move((GrooberSquadManager.AverageGrooberPosition() - transform.position).normalized);
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
