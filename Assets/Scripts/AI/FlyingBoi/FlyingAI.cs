﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAI : AIBase
{

    //float f_summoningSickness = 1;
    //float f_spottingDistance = 50;
    //LayerMask lm_spottingMask;

    //float f_timeStarted;
    //float f_targetFindLimiter = 0;
    //BehaviourTree tree;
    //Transform t_target;

    #region Defines

    private SequencerNode ParentDefine()
    {

        QueryNode sickness = new QueryNode(IsOverSummoningSickness);

        SelectorNode actionSequence = new SelectorNode(actionDefine());

        SequencerNode parent = new SequencerNode(sickness, actionSequence);
        return parent;
    }

    private SelectorNode actionDefine()
    {

        ActionNode moveAction = new ActionNode(Move);

        SelectorNode actionSel = new SelectorNode(ThrowDefine(), moveAction);
        return actionSel;
    }

    public SequencerNode ThrowDefine()
    {

        QueryNode inRange = new QueryNode(InRangeOfTarget);
        QueryNode canSee = new QueryNode(CanSeeTarget);
        ActionNode throwA = new ActionNode(Throw);

        SequencerNode throwSeq = new SequencerNode(HasGetHas(), inRange, canSee, throwA);
        return throwSeq;
    }

    public SequencerNode HasGetHas()
    {

        QueryNode has = new QueryNode(StillHasTarget);
        ActionNode get = new ActionNode(GetTargetAction);

        SequencerNode hgh = new SequencerNode(has, get, has);
        return hgh;
    }

    #endregion

    #region Queries

    public bool InRangeOfTarget()
    {
        return false;
    }

    #endregion

    #region Actions

    public void Move()
    {

    }

    public void Throw()
    {

    }

    #endregion

}
