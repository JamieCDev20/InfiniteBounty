using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HandymanAI : AIBase
{

    [SerializeField] private Vector2 throwForceRange = new Vector2(10, 500);
    [SerializeField] private HandymanHurtbox[] hurtBoxes;

    public delegate void HurtboxDel(bool active);

    public HurtboxDel toggleHurtboxes;


    private void Start()
    {
        tree = new BehaviourTree(ParentSequence());
        mover = GetComponent<HandymanMover>();
        anim = GetComponent<HandymanAnimator>();
        for (int i = 0; i < hurtBoxes.Length; i++)
        {
            toggleHurtboxes += hurtBoxes[i].SetHurtboxActive;
        }
    }

    private void Update()
    {
        tree.DoTreeIteration();
    }

    private SequencerNode ParentSequence()
    {
        return new SequencerNode(new QueryNode(IsOverSummoningSickness), SetupSequence());
    }

    private SequencerNode SetupSequence()
    {

        ActionNode getPlayer = new ActionNode(GetClosestPlayerAction);
        ActionNode getThrowable = new ActionNode(GetClosestThrowableObjectAction);
        ActionNode throwOrPunch = new ActionNode(CheckShouldBeThrowing);

        return new SequencerNode(getPlayer, getThrowable, throwOrPunch, ActionSelector());
    }

    private SelectorNode ActionSelector()
    {

        ActionNode moveAction = new ActionNode(MoveTowardsAction);

        return new SelectorNode(ThrowSequence(), PunchSequence(), moveAction);
    }

    private SequencerNode ThrowSequence()
    {
        QueryNode shouldThrow = new QueryNode(ShouldBeThrowingQuery);
        SelectorNode hasThrowableSelector = HasThrowableSelector();
        ActionNode doThrow = new ActionNode(ThrowAction);

        return new SequencerNode(shouldThrow, hasThrowableSelector, doThrow);
    }

    private SelectorNode HasThrowableSelector()
    {

        QueryNode hasThrowable = new QueryNode(HasThrowableObjectQuery);
        QueryNode inRange = new QueryNode(IsInPickUpRangeQuery);
        ActionNode pickup = new ActionNode(PickUpAction);

        SequencerNode getThrowableSeq = new SequencerNode(inRange, pickup, hasThrowable);

        return new SelectorNode(hasThrowable, getThrowableSeq);
    }

    private SequencerNode PunchSequence()
    {
        QueryNode inRange = new QueryNode(IsInPunchRangeQuery);
        ActionNode punch = new ActionNode(PunchAction);
        
        return new SequencerNode(inRange, punch);
    }

    private Vector3 GetThrowVector(Vector3 _pos)
    {
        //float dist = (transform.position - _pos).magnitude;
        //float distPecent = (dist / 2000);
        //float force = (distPecent * (throwForceRange.y - throwForceRange.x)) + throwForceRange.x;
        float force = 100;

        //float height = force * Mathf.Rad2Deg * Mathf.Tan(Mathf.Deg2Rad * angle);

        //force = Mathf.Sqrt((force * force) + (height * height));
        Vector3 dir = (_pos - go_centreofPickup.transform.position).normalized;
        Vector3 throwVec = dir;

        throwVec = throwVec.normalized * force;

        return throwVec;
    }

}
