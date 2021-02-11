using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandymanAI : AIBase
{
    //float f_summoningSickness = 1;
    //float f_spottingDistance = 50;
    //LayerMask lm_spottingMask;

    //float f_timeStarted;
    //float f_targetFindLimiter = 0;
    //BehaviourTree tree;
    //Transform t_target;

    private HandymanMover mover;

    private void Awake()
    {
        mover = GetComponent<HandymanMover>();
        tree = new BehaviourTree(ParentDefine());
    }

    private void Update()
    {
        tree.DoTreeIteration();
    }

    #region Defines

    private SequencerNode ParentDefine()
    {
        QueryNode summonSick = new QueryNode(IsOverSummoningSickness);

        ActionNode moveAction = new ActionNode(MoveAction);

        SequencerNode parent = new SequencerNode(summonSick, new SequencerNode(TargetterDefine(), ActionSelectorDefine()), moveAction);

        return parent;
    }

    private SelectorNode TargetterDefine()
    {

        QueryNode hasTarg = new QueryNode(StillHasTarget);
        ActionNode getTarg = new ActionNode(GetTargetAction);

        SequencerNode getTargSeq = new SequencerNode(getTarg, hasTarg);

        SelectorNode targetSel = new SelectorNode(hasTarg, getTargSeq);

        return targetSel;
    }

    private SelectorNode ActionSelectorDefine()
    {

        SelectorNode attackSel = new SelectorNode(PunchDefine(), ThrowDefine());

        return attackSel;
    }

    private SequencerNode PunchDefine()
    {
        QueryNode inRange = new QueryNode(InPunchRangeOfPlayer);

        ActionNode punch = new ActionNode(PunchAction);

        SequencerNode punchSeq = new SequencerNode(inRange, punch);

        return punchSeq;
    }

    private SequencerNode ThrowDefine()
    {
        QueryNode canSee = new QueryNode(CanSeeTarget);
        QueryNode nearThrowable = new QueryNode(InThrowRangeOfThing);
        ActionNode _throw = new ActionNode(ThrowAction);

        SequencerNode throwSeq = new SequencerNode(canSee, nearThrowable, _throw);

        return throwSeq;
    }

    #endregion

    #region Actions

    public void MoveAction()
    {

    }

    public void PunchAction()
    {

    }

    public void ThrowAction()
    {

    }

    #endregion

    #region Queries

    public bool InPunchRangeOfPlayer()
    {
        Debug.Log((transform.position - t_target.position).magnitude < 5? "in punch range" : "not in punch range");
        return (transform.position - t_target.position).magnitude < 5;
    }

    public bool InThrowRangeOfThing()
    {
        return true;
    }

    #endregion

}
