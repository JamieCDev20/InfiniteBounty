using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GrooberAI : AIBase
{
    private Animator anim;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        tree = new BehaviourTree(DefineTree());
        mover = GetComponent<HopdogMover>();
        f_timeStarted = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        tree.DoTreeIteration();
        anim.SetFloat("movblend", rb.velocity.magnitude);
    }

    private SelectorNode DefineTree()
    {

        QueryNode summonSickness = new QueryNode(IsOverSummoningSickness);

        SelectorNode parentSelector = new SelectorNode(summonSickness, BehaviourSelection());

        return parentSelector;

    }

    private SelectorNode BehaviourSelection()
    {
        SelectorNode behaviourSelector = new SelectorNode(MoveAttackSequence(), RunSequence());
        return behaviourSelector;
    }

    private SequencerNode MoveAttackSequence()
    {

        QueryNode withGroup = new QueryNode(IsWithGroupQuery);

        QueryNode hasTarget = new QueryNode(StillHasTarget);

        ActionNode moveToPlayer = new ActionNode(MoveTowardTarget);

        SequencerNode moveAtkSeq = new SequencerNode(withGroup, hasTarget, moveToPlayer, AttackSequence());

        return moveAtkSeq;
    }

    private SequencerNode AttackSequence()
    {

        QueryNode canAttack = new QueryNode(CanAttackQuery);

        ActionNode attack = new ActionNode(AttackAction);

        SequencerNode attackSeq = new SequencerNode(canAttack, attack);

        return attackSeq;
    }

    private SequencerNode RunSequence()
    {

        ActionNode findClosestPlayer = new ActionNode(FindClosestPlayer);
        ActionNode runFromPlayer = new ActionNode(MoveAwayFromTarget);

        SequencerNode runSeq = new SequencerNode(findClosestPlayer, runFromPlayer);

        return runSeq;
    }

}
