using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hopdog : MonoBehaviour
{

    [SerializeField] private float f_summoningSickness = 1;
    [SerializeField] private float f_attackRange = 15;
    [SerializeField] private float f_spottingDistance = 50;

    private float f_timeStarted;
    private float f_targetFindLimiter = 0;
    private BehaviourTree tree;
    private Transform t_target;
    private HopdogMover mover;

    private void Awake()
    {
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
    }

    private bool CanSeeTransform(Transform _targ)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, _targ.position - transform.position, out hit, f_spottingDistance))
        {
            return hit.collider.transform == _targ;
        }
        return false;
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

    public bool StillHasTarget()
    {
        if (t_target != null)
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
        foreach (PlayerHealth ph in FindObjectsOfType<PlayerHealth>())
        {
            if (!ph.IsDead())
            {
                if (CanSeeTransform(ph.transform))
                {
                    t_target = ph.transform;
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region Actions

    public void DebugTarget()
    {
        Debug.Log($"Target: {t_target}");
    }

    public void AttackAction()
    {

        mover.Launch(t_target.position);

    }

    public void MoveTowardsTargetAction()
    {
        mover.Move(t_target.position - transform.position);
    }

    public void IdleAction()
    {
        return;
    }

    public void GetTargetAction()
    {
        f_targetFindLimiter += Time.deltaTime;
        t_target = null;
        if (f_targetFindLimiter <= 1)
            return;

        f_targetFindLimiter = 0;

        t_target = FindObjectOfType<PlayerInputManager>().transform;

        //potentials = potentials.OrderBy(x => (transform.position - x.position).sqrMagnitude).ToList();

        //if (potentials.Count > 0)
        //    t_target = potentials[0];
    }

    #endregion

}
