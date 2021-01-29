using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAI : MonoBehaviour
{

    [SerializeField] private float f_initLifetimeDelay = 1;
    [SerializeField] private float f_visionDistance = 50;
    [SerializeField] private float f_speed = 2;
    [SerializeField] private GameObject followee;

    private BehaviourTree tree;
    private float f_startTime;

    private void Awake()
    {

        QueryNode startDelayTimeout = new QueryNode(AliveForLongerThanDelay);
        QueryNode canSeePlayer = new QueryNode(CanSeeFollowee);
        ActionNode moveTowardsPlayer = new ActionNode(MoveTowardsFollowee);

        SequencerNode followSequence = new SequencerNode(startDelayTimeout, canSeePlayer, moveTowardsPlayer);

        ActionNode idleAction = new ActionNode(IdleAction);

        SelectorNode parentSelector = new SelectorNode(followSequence, idleAction);

        tree = new BehaviourTree(parentSelector);

    }

    private void OnEnable()
    {
        f_startTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        tree.DoTreeIteration();
    }

    public void IdleAction()
    {

    }

    public bool AliveForLongerThanDelay()
    {
        return (Time.realtimeSinceStartup - f_startTime) > f_initLifetimeDelay;
    }

    public bool CanSeeFollowee()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, (followee.transform.position - transform.position), out hit, f_visionDistance);
        if(hit.collider != null)
            return hit.collider.gameObject == followee;
        return false;
    }

    public void MoveTowardsFollowee()
    {
        transform.LookAt(followee.transform);
        transform.Translate((followee.transform.position - transform.position).normalized * f_speed * Time.deltaTime, Space.World);
    }

}
