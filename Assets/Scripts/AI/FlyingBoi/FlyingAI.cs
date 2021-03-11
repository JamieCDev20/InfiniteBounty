using Photon.Pun;
using System.Collections;
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

    [SerializeField] private GameObject go_throwProjectile;
    [SerializeField] private float f_throwDistance = 20;
    [SerializeField] private float f_throwForce = 20;
    [SerializeField] private float f_throwDelay = 3;

    private FlyingMover mover;
    private float f_lastThrowTime = 0;

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

        SequencerNode throwSeq = new SequencerNode(new QueryNode(OverThrowSickness), HasGetHas(), inRange, canSee, throwA);
        return throwSeq;
    }

    public SelectorNode HasGetHas()
    {

        QueryNode has = new QueryNode(StillHasTarget);
        ActionNode get = new ActionNode(GetTargetAction);

        SequencerNode getHas = new SequencerNode(get, has);

        Debug.Log(getHas.GetChildren().Length);

        SelectorNode hgh = new SelectorNode(has, getHas);
        return hgh;
    }

    #endregion

    #region Queries

    public bool InRangeOfTarget()
    {
        return (t_target.position - transform.position).magnitude < f_throwDistance;
    }

    #endregion

    #region Actions

    public void Move()
    {
        if (t_target != null)
            mover.Move(((t_target.position + (Vector3.up * 10)) - transform.position));
        else
        {
            Vector3 pos = Vector3.zero;
            int c = 0;

            foreach (GameObject p in TagManager.x.GetTagSet("Player"))
            {
                pos += p.transform.position;
                c++;
            }
            pos /= c;

            mover.Move(((pos + (Vector3.up * 10)) - transform.position));
        }


    }

    public void Throw()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        Vector3 _dir = t_target.position - transform.position;
        photonView.RPC(nameof(RemoteThrow), RpcTarget.AllViaServer, _dir);
        f_lastThrowTime = Time.realtimeSinceStartup;
    }

    public bool OverThrowSickness()
    {
        return Time.realtimeSinceStartup - f_lastThrowTime > f_throwDelay;
    }

    [PunRPC]
    public void RemoteThrow(Vector3 dir)
    {
        GameObject ob = PoolManager.x?.SpawnObject(go_throwProjectile, transform.position, Quaternion.LookRotation(dir));
        ob.GetComponent<Rigidbody>().AddForce(ob.transform.forward.normalized * f_throwForce, ForceMode.Impulse);
    }

    #endregion

}
