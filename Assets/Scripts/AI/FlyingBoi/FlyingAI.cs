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

        b_isRightWinged = Random.Range(0, 10) < 6 ? true : false;

    }

    private void FixedUpdate()
    {
        tree.DoTreeIteration();
    }

    #region Defines

    private SelectorNode ParentDefine()
    {

        ActionNode moveToPlayer = new ActionNode(MoveTowardtarget);

        SequencerNode orbitAttackSeq = new SequencerNode(OrbitAttack());

        return new SelectorNode(orbitAttackSeq, moveToPlayer);

    }

    private SequencerNode OrbitAttack()
    {

        QueryNode inRange = new QueryNode(IsInOrbitRangeQuery);
        ActionNode orbit = new ActionNode(OrbitTarget);
        ActionNode shoot = new ActionNode(Shoot);

        return new SequencerNode(inRange, orbit, shoot);
    }

    #endregion

}
