using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hopdog : MonoBehaviour
{

    [SerializeField] private float f_summoningSickness = 1;
    [SerializeField] private float f_attackRange = 15;
    [SerializeField] private float f_spottingDistance = 50;

    private float f_timeStarted;
    private BehaviourTree tree;
    private Transform t_target;

    private void Awake()
    {

        //tree = new BehaviourTree();

    }

    private void OnEnable()
    {
        f_timeStarted = Time.realtimeSinceStartup;
    }

    private bool CanSeeTransform(Transform _targ)
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, _targ.position - transform.position, out hit, f_spottingDistance))
        {
            return hit.collider.transform == _targ;
        }
        return false;
    }

    #region BehaviourNodeDefinitions

    private SelectorNode TargetedActionSelectorDefinition()
    {

        QueryNode isOverSummoningSicknessNode = new QueryNode(IsOverSummoningSickness);
        QueryNode stillHasTargetNode = new QueryNode(StillHasTarget);
        ActionNode idleActionNode = new ActionNode(IdleAction);

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

        QueryNode hasTargetNode = new QueryNode(HasTarget);

        QueryNode withinRangeNode = new QueryNode(IsWithinAttackRange);

        ActionNode attackActionNode = new ActionNode(AttackAction);

        SequencerNode attackSequence = new SequencerNode(hasTargetNode, withinRangeNode, attackActionNode);

        return attackSequence;

    }

    #endregion

    #region Queries

    public bool StillHasTarget()
    {
        if(t_target != null)
        {
            if (CanSeeTransform(t_target))
                return CanSeeTransform(t_target);
            else
                t_target = null;
        }
        return false;
    }

    public bool IsOverSummoningSickness()
    {
        return (Time.realtimeSinceStartup - f_timeStarted) > f_summoningSickness;
    }

    public bool HasTarget()
    {
        return t_target != null;
    }

    public bool IsWithinAttackRange()
    {
        return (transform.position - t_target.position).sqrMagnitude < f_attackRange * f_attackRange;
    }

    public bool CanSeeAPlayer()
    {
        Debug.LogError("Not implemented can see player check");
        return false;
    }

    #endregion

    #region Actions

    public void AttackAction()
    {
        Debug.Log("Doing attack action");
    }

    public void MoveTowardsTargetAction()
    {
        Debug.Log("Moving towards target");
    }

    public void IdleAction()
    {
        Debug.Log("Idling");
        return;
    }

    #endregion

}
