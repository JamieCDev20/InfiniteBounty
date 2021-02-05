using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : AIBase
{
    private bool b_canAttack;
    internal List<Transform> tL_potentialTargets = new List<Transform>();
    private int i_currentTarget;
    [SerializeField] private float f_timeBetweenAttacks;

    [Header("Melee Thangs")]
    [SerializeField] private float f_meleeRange;

    [Header("Mortar Attack")]
    [SerializeField] private string s_mortarShotPath;
    [SerializeField] private Vector2 v_numberOfMortarShots;
    [SerializeField] private ParticleSystem p_mortarParticle;

    [Header("Homing Attack")]
    [SerializeField] private string s_homingMissilePath;
    [SerializeField] private Vector2 v_homingOrbAmount;
    [SerializeField] private float f_homingForwardMovement;

    private void Start()
    {
        QueryNode _q_canAttack = new QueryNode(CheckCanAttack);
        SequencerNode _s = new SequencerNode(_q_canAttack, AttackDefine());

        tree = new BehaviourTree(_s);
        Invoke(nameof(StartAttacking), 6);
    }
    private void StartAttacking()
    {
        b_canAttack = true;
    }
    private void StopAttackingForPeriod()
    {
        if (PhotonNetwork.IsMasterClient)
            b_canAttack = false;
        Invoke(nameof(StartAttacking), f_timeBetweenAttacks);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            tree.DoTreeIteration();
            print("Doing trees");
        }

        transform.LookAt(new Vector3(tL_potentialTargets[i_currentTarget].transform.position.x, transform.position.y, tL_potentialTargets[i_currentTarget].transform.position.z));
    }

    #region Defines

    private SequencerNode MeleeDefine()
    {
        QueryNode _qn_rangeCheck = new QueryNode(RangeCheck);
        ActionNode _a_melee = new ActionNode(DoTheMelee);
        return new SequencerNode(_qn_rangeCheck, _a_melee);
    }

    private SelectorNode RangedDefine()
    {
        QueryNode _qn_randomChance = new QueryNode(RandomValue);
        ActionNode _an_homing = new ActionNode(DoHomingAttack);
        ActionNode _an_mortar = new ActionNode(DoMortarAttack);
        SequencerNode _sn_isHoming = new SequencerNode(_qn_randomChance, _an_homing);
        SequencerNode _sn_isMortar = new SequencerNode(_qn_randomChance, _an_mortar);

        return new SelectorNode(_sn_isHoming, _sn_isMortar);
    }

    private SelectorNode AttackDefine()
    {
        ActionNode _a_move = new ActionNode(MoveAttack);
        return new SelectorNode(MeleeDefine(), RangedDefine(), _a_move);
    }

    #endregion

    #region Checks

    private bool CheckCanAttack()
    {
        return b_canAttack;
    }

    #endregion

    #region Melee

    private bool RangeCheck()
    {
        return ((transform.position - tL_potentialTargets[i_currentTarget].position).sqrMagnitude < f_meleeRange * f_meleeRange);
    }

    private void DoTheMelee()
    {
        print("AHHH! I'M DOING A MELEE");
        StopAttackingForPeriod();
    }


    #endregion

    #region Ranged

    private bool RandomValue()
    {
        return Random.value < 0.4f;
    }

    private void DoHomingAttack()
    {
        StartCoroutine(HomingAttack(Mathf.RoundToInt(Random.Range(v_homingOrbAmount.x, v_homingOrbAmount.y)), t_target));
        StopAttackingForPeriod();
    }
    private IEnumerator HomingAttack(int _i_amount, Transform _t_target)
    {
        List<GameObject> _goL_orbs = new List<GameObject>();
        for (int i = 0; i < _i_amount; i++)
        {
            yield return new WaitForSeconds(1);
            GameObject _go = PhotonNetwork.Instantiate(s_homingMissilePath, transform.position + transform.forward, Quaternion.identity);
            _go.GetComponent<BossProjectile>().Setup(tL_potentialTargets[i_currentTarget]);
            _go.transform.position = transform.position + transform.forward * i;
            _go.transform.forward = transform.forward;
            _goL_orbs.Add(_go);
        }

        while (_goL_orbs.Count > 0)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < _goL_orbs.Count; i++)
            {
                _goL_orbs[i].transform.position += _goL_orbs[i].transform.forward * f_homingForwardMovement * Time.deltaTime;
                _goL_orbs[i].transform.LookAt(_t_target);
            }
        }
    }

    private void DoMortarAttack()
    {
        photonView.RPC(nameof(MortarAttackRPC), RpcTarget.All, Random.Range(0, 9999999));
    }
    [PunRPC]
    public void MortarAttackRPC(int _i_seed)
    {
        StartCoroutine(MortarAttackActual(_i_seed));
        StopAttackingForPeriod();
    }
    private IEnumerator MortarAttackActual(int _i_seed)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Random.InitState(_i_seed);
            p_mortarParticle.Play();
            yield return new WaitForSeconds(2);

            for (int i = 0; i < Random.Range(v_numberOfMortarShots.x, v_numberOfMortarShots.y); i++)
            {
                yield return new WaitForSeconds(0.2f);
                Vector3 _v_posToDropOn = PickArenaPosition() + Vector3.up * 200;
                PhotonNetwork.Instantiate(s_mortarShotPath, _v_posToDropOn, Quaternion.identity);
            }
        }
    }
    private Vector3 PickArenaPosition()
    {
        Vector3 _v = new Vector3(Random.Range(-75, 75), 0, Random.Range(-75, 75));
        return _v;
    }
    #endregion

    #region Move

    private Vector3 PickTargetPosition()
    {
        return tL_potentialTargets[Random.Range(0, tL_potentialTargets.Count)].position;
    }

    private void MoveAttack()
    {
        photonView.RPC(nameof(MoveAttack), RpcTarget.All, PickTargetPosition(), Random.Range(1f, 5f));
    }

    [PunRPC]
    private void MoveAttack(Vector3 _v_newPos, float _f_timeToWait)
    {
        b_canAttack = false;
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("ChangeTarget", RpcTarget.All, Random.Range(0, tL_potentialTargets.Count));
        StartCoroutine(TimedMove(_v_newPos, _f_timeToWait));
    }
    private IEnumerator TimedMove(Vector3 _v_newPos, float _f_timeToWait)
    {
        for (int i = 0; i < 80; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position += Vector3.down * 0.5f;
        }

        yield return new WaitForSeconds(_f_timeToWait);

        transform.position = _v_newPos + Vector3.down * 30;

        Collider[] _cA = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * 30, 10);
        for (int i = 0; i < _cA.Length; i++)
            _cA[i].GetComponent<IHitable>()?.TakeDamage(50, false);


        for (int i = 0; i < 80; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position += Vector3.up * 0.5f;
        }

        if (PhotonNetwork.IsMasterClient)
            b_canAttack = true;
    }

    #endregion

    [PunRPC]
    public void ChangeTarget(int _i_newTargetIndex)
    {
        i_currentTarget = _i_newTargetIndex;
    }

}