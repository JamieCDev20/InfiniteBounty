using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GrooberAI : AIBase
{
    private Animator anim;
    private Rigidbody rb;
    private bool b_inGroup;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        tree = new BehaviourTree(DefineTree());
        mover = GetComponent<HandymanMover>();
        f_timeStarted = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        tree.DoTreeIteration();
        Debug.Log(b_inGroup);
        anim.SetFloat("movblend", rb.velocity.magnitude);
    }

    private SelectorNode DefineTree()
    {

        QueryNode summonSickness = new QueryNode(IsOverSummoningSickness);

        SelectorNode parentSelector = new SelectorNode(summonSickness, BehaviourSequence());

        return parentSelector;

    }

    private SelectorNode ActionSelector()
    {

        QueryNode inGroup = new QueryNode(IsWithGroupQuery);

        ActionNode moveToPlayer = new ActionNode(MoveTowardTarget);

        SequencerNode inGroupSequence = new SequencerNode(inGroup, moveToPlayer/*, AttackSequence()*/);

        return new SelectorNode(inGroupSequence, new ActionNode(MoveAwayFromTarget));
    }

    private SequencerNode BehaviourSequence()
    {
        ActionNode findPlayer = new ActionNode(FindClosestPlayer);

        ActionNode checkIfInGroup = new ActionNode(IsWithGroupAction);

        SelectorNode actionSelector = new SelectorNode(ActionSelector());

        return new SequencerNode(findPlayer, checkIfInGroup, actionSelector);

    }

    private SequencerNode AttackSequence()
    {

        QueryNode canAttack = new QueryNode(CanAttackQuery);

        ActionNode attack = new ActionNode(AttackAction);

        SequencerNode attackSeq = new SequencerNode(canAttack, attack);

        return attackSeq;
    }

}
