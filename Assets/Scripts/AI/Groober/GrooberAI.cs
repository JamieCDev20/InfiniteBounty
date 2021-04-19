using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GrooberAI : AIBase
{
    private void Start()
    {
        tree = new BehaviourTree(DefineTree());
        mover = GetComponent<HandymanMover>();
    }

    private void Update()
    {
        tree.DoTreeIteration();
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

        ActionNode moveToPlayer = new ActionNode(MoveTowardTarget);

        SequencerNode moveAtkSeq = new SequencerNode(withGroup, moveToPlayer, AttackSequence());

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
