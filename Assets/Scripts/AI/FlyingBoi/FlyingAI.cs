using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FlyingAI : AIBase
{

    [SerializeField] private GameObject go_throwProjectile;

    private FlyingMover mover;

    private void Start()
    {
        mover = GetComponent<FlyingMover>();

        tree = new BehaviourTree(ParentDefine());

    }

    private void FixedUpdate()
    {
        tree.DoTreeIteration();
    }

    #region Defines

    private SelectorNode ParentDefine()
    {

        ActionNode moveToPlayer = new ActionNode(BlankAction);

        SequencerNode orbitAttackSeq = new SequencerNode(OrbitAttack());

        return new SelectorNode(orbitAttackSeq, moveToPlayer);

    }

    private SequencerNode OrbitAttack()
    {

        QueryNode inRange = new QueryNode(BlankQuery);
        ActionNode orbit = new ActionNode(BlankAction);
        ActionNode shoot = new ActionNode(BlankAction);

        return new SequencerNode(inRange, orbit, shoot);
    }

    #endregion

}
