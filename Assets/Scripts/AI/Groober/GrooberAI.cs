using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GrooberAI : AIBase
{
    private GrooberAnimator anim;
    private Rigidbody rb;
    private bool b_inGroup;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<GrooberAnimator>();
        tree = new BehaviourTree(DefineTree());
        mover = GetComponent<Moober>();
        f_timeStarted = Time.realtimeSinceStartup;

        i_actualDamage = Mathf.RoundToInt(i_damage * DifficultyManager.x.ReturnCurrentDifficulty().f_damageMult);
    }

    private void Update()
    {
        tree.DoTreeIteration();
        f_currentTime -= Time.deltaTime;
    }

    private SequencerNode DefineTree()
    {

        QueryNode summonSickness = new QueryNode(IsOverSummoningSickness);

        SequencerNode parentSelector = new SequencerNode(summonSickness, BehaviourSequence());

        return parentSelector;

    }

    private SelectorNode ActionSelector()
    {

        QueryNode inGroup = new QueryNode(IsWithGroupQuery);

        QueryNode attackCooldown = new QueryNode(AttackOffCooldownQuery);

        ActionNode moveToPlayer = new ActionNode(MoveTowardTarget);

        SequencerNode inGroupSequence = new SequencerNode(inGroup, moveToPlayer, AttackSelector());

        return new SelectorNode(inGroupSequence, RetreatSelector());
    }

    private SelectorNode RetreatSelector()
    {
        SequencerNode attackedSeq = new SequencerNode(new QueryNode(AttackOnCooldownQuery), new ActionNode(MoveAwayFromTarget));
        return new SelectorNode(attackedSeq, new ActionNode(MoveTowardHorde));
    }

    private SequencerNode BehaviourSequence()
    {
        ActionNode findPlayer = new ActionNode(FindClosestPlayer);

        ActionNode checkIfInGroup = new ActionNode(IsWithGroupAction);

        SelectorNode actionSelector = new SelectorNode(ActionSelector());

        return new SequencerNode(findPlayer, checkIfInGroup, actionSelector);

    }

    private SelectorNode AttackSelector()
    {
        return new SelectorNode(AttackSequence(), new ActionNode(DummyAction));
    }

    private void DummyAction()
    {

    }

    private SequencerNode AttackSequence()
    {

        QueryNode cooldown = new QueryNode(AttackOffCooldownQuery);

        QueryNode canAttack = new QueryNode(IsWithinAttackRangeQuery);

        ActionNode attack = new ActionNode(AttackAction);

        SequencerNode attackSeq = new SequencerNode(cooldown, canAttack, attack);

        return attackSeq;
    }

}
