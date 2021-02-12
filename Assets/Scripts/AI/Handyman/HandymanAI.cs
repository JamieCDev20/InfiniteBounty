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
    private HandymanAnimator anim;

    [SerializeField] private HandymanHurtbox[] hurtBoxes;

    public delegate void HurtboxDel(bool active);

    public HurtboxDel toggleHurtboxes;

    private bool b_hurtsActive = false;

    private void Awake()
    {
        mover = GetComponent<HandymanMover>();
        anim = GetComponent<HandymanAnimator>();
        tree = new BehaviourTree(ParentDefine());

        for (int i = 0; i < hurtBoxes.Length; i++)
        {
            toggleHurtboxes += hurtBoxes[i].SetHurtboxActive;
        }

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

        SelectorNode behavSel = new SelectorNode(new SequencerNode(TargetterDefine(), ActionSelectorDefine()), moveAction);

        SequencerNode parent = new SequencerNode(summonSick, behavSel);

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
        //Debug.Log("Doing move action");
        foreach (GameObject p in TagManager.x.GetTagSet("Player"))
        {
            mover.Move(p.transform.position - transform.position);
            return;
        }
    }

    public void PunchAction()
    {
        anim.Slap();
        f_timeStarted = Time.realtimeSinceStartup;
        b_hurtsActive = true;
        toggleHurtboxes(true);
    }

    public void ThrowAction()
    {
        Debug.Log("Doing Throw action");

    }

    #endregion

    #region Queries

    public override bool IsOverSummoningSickness()
    {
        if(b_hurtsActive)
            if (base.IsOverSummoningSickness())
            {
                b_hurtsActive = false;
                toggleHurtboxes(false);
            }
        return base.IsOverSummoningSickness();
    }

    public bool InPunchRangeOfPlayer()
    {
        if ((t_target.position - transform.position).sqrMagnitude < 200)
            return true;
        return false;
    }

    public bool InThrowRangeOfThing()
    {
        return false;
    }

    #endregion

}
