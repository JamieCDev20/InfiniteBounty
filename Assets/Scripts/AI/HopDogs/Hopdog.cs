using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hopdog : AIBase
{

    [SerializeField] private float f_attackRange = 15;
    [SerializeField] private float f_attackDuration = 3;

    private float f_attackStart;
    private HopdogMover mover;
    private HopdogAnimator anima;

    private void Awake()
    {
        anima = GetComponent<HopdogAnimator>();
        mover = GetComponent<HopdogMover>();
        tree = new BehaviourTree(ParentSequencer());

    }

    private void OnEnable()
    {
        f_timeStarted = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        tree.DoTreeIteration();
        anima.SetGrounded(Physics.Raycast(transform.position + (Vector3.up * 0.01f), Vector3.down, 0.1f));
    }

    #region BehaviourNodeDefinitions

    private SequencerNode ParentSequencer()
    {
        QueryNode summoningSicknessNode = new QueryNode(IsOverSummoningSickness);
        SelectorNode actionSelector = new SelectorNode(ActionSelectorDefinition());

        SequencerNode parentNode = new SequencerNode(summoningSicknessNode, actionSelector);
        return parentNode;
    }

    private SelectorNode ActionSelectorDefinition()
    {
        ActionNode idleActionNode = new ActionNode(IdleAction);

        SelectorNode actionSelectNode = new SelectorNode(TargetAttackSequence(), idleActionNode);
        return actionSelectNode;
    }

    private SequencerNode TargetAttackSequence()
    {
        SelectorNode hasGetTargetSelector = new SelectorNode(RetargetSelector());
        SelectorNode targettedActionNode = new SelectorNode(TargetedActionSelectorDefinition());
        SequencerNode targAttackSequence = new SequencerNode(hasGetTargetSelector, targettedActionNode);
        return targAttackSequence;
    }

    private SelectorNode RetargetSelector()
    {

        QueryNode hasNode = new QueryNode(StillHasTarget);
        ActionNode getNode = new ActionNode(GetTargetAction);

        //this is to check if the agent has a target after getting one since actions always return true
        //the sequence and therefore the selector will fail if no target is had or found
        SequencerNode getTargetSequence = new SequencerNode(getNode, hasNode);

        SelectorNode retargetNode = new SelectorNode(hasNode, getTargetSequence);
        return retargetNode;
    }

    private SelectorNode TargetedActionSelectorDefinition()
    {

        SelectorNode targetedActionSelector = new SelectorNode(AttackPlayerDefinition(), FollowPlayerDefinition());

        return targetedActionSelector;

    }

    private SequencerNode FollowPlayerDefinition()
    {

        QueryNode canSeePlayerNode = new QueryNode(CanSeeAPlayer);

        ActionNode moveTowardsNode = new ActionNode(MoveTowardsTargetAction);

        SequencerNode followSequenceNode = new SequencerNode(canSeePlayerNode, moveTowardsNode);

        return followSequenceNode;

    }

    private SequencerNode AttackPlayerDefinition()
    {

        QueryNode withinRangeNode = new QueryNode(IsWithinAttackRange);

        ActionNode attackActionNode = new ActionNode(AttackAction);

        SequencerNode attackSequence = new SequencerNode(withinRangeNode, attackActionNode);

        return attackSequence;

    }

    #endregion

    #region Queries

    public bool IsWithinAttackRange()
    {
        return (transform.position - t_target.position).sqrMagnitude < f_attackRange * f_attackRange;
    }

    #endregion

    #region Actions

    public void AttackAction()
    {
        if (f_attackStart == 0)
        {
            if (Time.realtimeSinceStartup - f_attackStart > f_attackDuration)
            {
                f_attackStart = 0;
                t_target = null;
            }
            return;
        }
        f_attackStart = Time.realtimeSinceStartup;
        mover.Launch(t_target.position);
    }

    public void MoveTowardsTargetAction()
    {
        mover.Move(t_target.position - transform.position);
    }

    public void IdleAction()
    {
        if ((Time.realtimeSinceStartup + Random.value) * 100 % 100 <= 0.5f)
            mover.Move(Vector3.right * (Random.value * 2 - 1) + Vector3.forward * (Random.value * 2 - 1));
    }

    #endregion

}
