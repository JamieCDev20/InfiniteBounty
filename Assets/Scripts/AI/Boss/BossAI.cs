using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : AIBase
{
    private Animator anim;
    private bool b_canAttack = true;
    internal List<Transform> tL_potentialTargets = new List<Transform>();
    private int i_currentTarget;
    [SerializeField] private float f_timeBetweenAttacks;

    [Header("Melee Thangs")]
    [SerializeField] private float f_meleeRange;

    [Header("Mortar Attack")]
    [SerializeField] private GameObject go_mortarBall;
    [SerializeField] private Vector2 v_numberOfMortarShots;
    [SerializeField] private ParticleSystem p_mortarParticle;

    [Header("Homing Attack")]
    [SerializeField] private string s_homingMissilePath;
    [SerializeField] private Vector2 v_homingOrbAmount;
    [SerializeField] private float f_homingForwardMovement;
    [SerializeField] private Transform t_firePoint;

    [Header("Movement Stats")]
    [SerializeField] private GameObject go_movementTelegraph;
    private GameObject go_looker;
    [SerializeField] private float f_timeBetweenMoves;
    private bool b_canMove = true;

    [Header("Enemies")]
    [SerializeField] private float f_timeBetweenEnemies = 20;
    [SerializeField] private string s_enemyPath;
    private Vector2Int vi_enemiesPerWave;

    private void Start()
    {
        go_looker = new GameObject("Boss Looker");
        anim = GetComponentInChildren<Animator>();
        QueryNode _q_canAttack = new QueryNode(CheckCanAttack);
        SequencerNode _s = new SequencerNode(_q_canAttack, AttackDefine());

        tree = new BehaviourTree(_s);
        Invoke(nameof(StartAttacking), 10);

        DifficultySet _ds = DifficultyManager.x.ReturnCurrentDifficulty();
        vi_enemiesPerWave = _ds.vi_enemiesPerBossWave;

    }
    private void StartAttacking()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            b_canAttack = true;
            StartCoroutine(SummonEnemies());
        }
    }
    private void StopAttackingForPeriod()
    {
        b_canAttack = false;
        Invoke(nameof(StartAttacking), f_timeBetweenAttacks);
    }

    private void StartMoving()
    {
        if (PhotonNetwork.IsMasterClient)
            b_canMove = true;
    }
    private void StopMovingForPeriod()
    {
        b_canMove = false;
        Invoke(nameof(StartMoving), f_timeBetweenMoves);

    }

    private void Update()
    {
        //if (PhotonNetwork.IsMasterClient)

        if (tL_potentialTargets.Count > 0)
        {
            go_looker.transform.position = transform.position;
            go_looker.transform.LookAt(new Vector3(tL_potentialTargets[i_currentTarget].transform.position.x, transform.position.y, tL_potentialTargets[i_currentTarget].transform.position.z));
            transform.forward = Vector3.Lerp(transform.forward, go_looker.transform.forward, Time.deltaTime);
        }
    }

    public void ChooseAction()
    {
        tree.DoTreeIteration();

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
        ActionNode _an_homing = new ActionNode(PlayHomingAnim);
        ActionNode _an_mortar = new ActionNode(PlayMetoerAnim);
        SequencerNode _sn_isHoming = new SequencerNode(_qn_randomChance, _an_homing);
        SequencerNode _sn_isMortar = new SequencerNode(_qn_randomChance, _an_mortar);

        return new SelectorNode(_sn_isHoming, _sn_isMortar);
    }

    private SelectorNode AttackDefine()
    {
        SequencerNode _a_move = new SequencerNode(new QueryNode(CheckCanMove), new ActionNode(PlayMoveAnim));
        return new SelectorNode(MeleeDefine(), RangedDefine(), _a_move);
    }

    #endregion

    #region Checks

    private bool CheckCanAttack()
    {
        return b_canAttack;
    }

    private bool CheckCanMove()
    {
        return b_canMove;
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
        float a = Random.value;
        return a < 0.5f;
    }

    private void PlayHomingAnim()
    {
        anim.SetTrigger("Missile");
    }

    internal void DoHomingAttack()
    {
        StartCoroutine(HomingAttack(Mathf.RoundToInt(Random.Range(v_homingOrbAmount.x, v_homingOrbAmount.y)), t_target));
        StopAttackingForPeriod();
    }
    private IEnumerator HomingAttack(int _i_amount, Transform _t_target)
    {
        List<GameObject> _goL_orbs = new List<GameObject>();
        for (int i = 0; i < _i_amount; i++)
        {
            GameObject _go = PhotonNetwork.Instantiate(s_homingMissilePath, t_firePoint.position + transform.forward, Quaternion.identity);
            _go.GetComponent<BossProjectile>().Setup(tL_potentialTargets[i_currentTarget]);
            //_go.transform.position = t_firePoint.position + transform.forward ;
            _go.transform.forward = transform.forward;
            _goL_orbs.Add(_go);
            yield return new WaitForSeconds(1);
        }

        while (_goL_orbs.Count > 0)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < _goL_orbs.Count; i++)
            {
                _goL_orbs[i].transform.position += _goL_orbs[i].transform.forward * f_homingForwardMovement * Time.deltaTime;
                _goL_orbs[i].transform.LookAt(tL_potentialTargets[i_currentTarget]);
            }
        }
    }

    private void PlayMetoerAnim()
    {
        anim.SetBool("Meteor", true);
    }

    internal void DoMortarAttack()
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
        p_mortarParticle.Play();

        Random.InitState(_i_seed);

        yield return new WaitForSeconds(2);

        for (int i = 0; i < v_numberOfMortarShots.x; i++)
        {
            yield return new WaitForSeconds(0.2f);
            Vector3 _v_posToDropOn = PickArenaPosition() + Vector3.up * 200;
            Instantiate(go_mortarBall, _v_posToDropOn, Quaternion.identity);
        }
        anim.SetBool("Meteor", false);
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

    private void PlayMoveAnim()
    {
        StopMovingForPeriod();
        anim.SetBool("Emerging", false);
        anim.SetBool("Submerging", true);
    }

    internal void MoveAttack()
    {
        photonView.RPC(nameof(MoveAttackRPC), RpcTarget.All, PickTargetPosition());
    }

    [PunRPC]
    private void MoveAttackRPC(Vector3 _v_newPos)
    {
        b_canAttack = false;
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(ChangeTarget), RpcTarget.All, Random.Range(0, tL_potentialTargets.Count));
        StartCoroutine(TimedMove(_v_newPos));

    }
    private IEnumerator TimedMove(Vector3 _v_newPos)
    {
        b_canAttack = false;

        yield return new WaitForSeconds(1);
        transform.position = new Vector3(_v_newPos.x, -100, _v_newPos.z);
        yield return new WaitForSeconds(2);

        go_movementTelegraph.transform.position = new Vector3(_v_newPos.x, 0, _v_newPos.z);

        yield return new WaitForSeconds(2);
        transform.position = new Vector3(_v_newPos.x, 0, _v_newPos.z);
        anim.SetBool("Submerging", false);
        anim.SetBool("Emerging", true);

        yield return new WaitForSeconds(1);

        Collider[] _cA = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * 30, 10);
        for (int i = 0; i < _cA.Length; i++)
            if (_cA[i].transform.root != transform)
                if (_cA[i].transform.CompareTag("Lilypad"))
                    _cA[i].GetComponent<IHitable>()?.TakeDamage(50, false);

        go_movementTelegraph.transform.position = Vector3.down * 100;

        if (PhotonNetwork.IsMasterClient)
            b_canAttack = true;
    }

    #endregion

    [PunRPC]
    public void ChangeTarget(int _i_newTargetIndex)
    {
        i_currentTarget = _i_newTargetIndex;

        if (PhotonNetwork.IsMasterClient)
            if (tL_potentialTargets[i_currentTarget].GetComponentInParent<PlayerHealth>().IsDead())
            {
                tL_potentialTargets.RemoveAt(i_currentTarget);
                photonView.RPC(nameof(ChangeTarget), RpcTarget.All, Random.Range(0, tL_potentialTargets.Count));
            }
    }

    private IEnumerator SummonEnemies()
    {
        for (int i = 0; i < Random.Range(vi_enemiesPerWave.x, vi_enemiesPerWave.y); i++)
            PhotonNetwork.Instantiate(s_enemyPath, PickArenaPosition().normalized * 300 + Vector3.up * 200, Quaternion.identity);

        yield return new WaitForSeconds(f_timeBetweenEnemies);

        StartCoroutine(SummonEnemies());
    }

}