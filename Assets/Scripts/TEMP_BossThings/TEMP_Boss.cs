using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_Boss : MonoBehaviourPunCallbacks, IHitable
{
    private PhotonView view;
    private List<Transform> tL_potentialTargets = new List<Transform>();
    private bool b_isAttacking;
    [SerializeField] private float f_timeBetweenAttacks = 6;
    private float f_currentTimer;
    [SerializeField] private float f_timeBetweenMoves = 15;
    private float f_currentMoveTimer;
    private int i_currentTarget;
    [SerializeField] private int i_currentHealth;
    private bool b_isHost;
    [SerializeField] private LayerMask lm_playerLayer;

    [Header("Homing Attack")]
    [SerializeField] private GameObject go_homingPrefab;
    [SerializeField] private Vector2 v_shotsPerHomingRound;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        PhotonNetwork.RegisterPhotonView(view);


        if (PhotonNetwork.IsMasterClient)
        {
            //Get the players as targets
            GameObject[] _goA = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < _goA.Length; i++)
                tL_potentialTargets.Add(_goA[i].transform);

            Invoke("GetTargets", 1);

            Invoke("BeginAttacks", 7);
            b_isHost = true;
        }
    }

    public void GetTargets()
    {
        Collider[] _cA = Physics.OverlapSphere(transform.position, 30, lm_playerLayer);

        for (int i = 0; i < _cA.Length; i++)
            if (_cA[i].CompareTag("Player"))
                tL_potentialTargets.Add(_cA[i].transform);

        i_currentHealth *= tL_potentialTargets.Count;
    }

    private void Update()
    {
        if (b_isAttacking)
        {
            f_currentTimer += Time.deltaTime;
            f_currentMoveTimer += Time.deltaTime;

            if (f_currentMoveTimer > f_timeBetweenMoves) view.RPC("MoveAttack", RpcTarget.All, PickTargetPosition());
            if (f_currentTimer > f_timeBetweenAttacks) PickAttack();
        }
        transform.LookAt(new Vector3(tL_potentialTargets[i_currentTarget].position.x, transform.position.y, tL_potentialTargets[i_currentTarget].position.z));
    }

    private Vector3 PickTargetPosition()
    {
        Vector3 _v = tL_potentialTargets[Random.Range(0, tL_potentialTargets.Count)].position;
        _v = Vector3.Scale(_v, Vector3.one - Vector3.up);
        return _v;
    }

    private void BeginAttacks()
    {
        b_isAttacking = true;
    }

    [PunRPC]
    private void ChangeTarget(int _i_newTarget)
    {
        i_currentTarget = _i_newTarget;
    }

    [PunRPC]
    private void MoveAttack(Vector3 _v_newPos)
    {
        b_isAttacking = false;
        if (b_isHost)
            view.RPC("ChangeTarget", RpcTarget.All, Random.Range(0, tL_potentialTargets.Count));
        StartCoroutine(TimedMove(_v_newPos));
    }
    private IEnumerator TimedMove(Vector3 _v_newPos)
    {
        for (int i = 0; i < 60; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position += Vector3.down * 0.5f;
        }

        yield return new WaitForSeconds(1);

        transform.position = _v_newPos + Vector3.down * 20;

        Collider[] _cA = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * 50, 15);
        for (int i = 0; i < _cA.Length; i++)
            _cA[i].GetComponent<IHitable>()?.TakeDamage(50, false);


        for (int i = 0; i < 60; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position += Vector3.up * 0.5f;
        }

        f_currentMoveTimer = 0;
        if (b_isHost)
            b_isAttacking = true;
    }



    private void PickAttack()
    {
        int _i_moveIndex = Random.Range(0, 3);

        switch (_i_moveIndex)
        {
            case 0:
                view.RPC("HomingAttackRPC", RpcTarget.All, Mathf.RoundToInt(Random.Range(v_shotsPerHomingRound.x, v_shotsPerHomingRound.y)));
                break;
            case 3:
                view.RPC("MortarAttack", RpcTarget.All);
                break;
            case 4:
                view.RPC("MeleeAttack", RpcTarget.All);
                break;
        }
        f_currentTimer = 0;
        if (b_isHost)
            view.RPC("ChangeTarget", RpcTarget.All, Random.Range(0, tL_potentialTargets.Count));
    }

    [PunRPC]
    private void MortarAttack()
    {

    }

    [PunRPC]
    private void HomingAttackRPC(int _i_amount)
    {
        StartCoroutine(HomingAttack(_i_amount));
    }
    private IEnumerator HomingAttack(int _i_amount)
    {
        for (int i = 0; i < _i_amount; i++)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject _go = Instantiate(go_homingPrefab);
            _go.GetComponent<BossProjectile>().Setup(tL_potentialTargets[i_currentTarget]);
            _go.transform.position = transform.position + transform.forward;
            _go.transform.forward = transform.forward;
        }
    }


    [PunRPC]
    private void MeleeAttack()
    {

    }


    public void TakeDamage(int damage, bool activatesThunder)
    {
        if (b_isHost)
            view.RPC("ActualTakeDamage", RpcTarget.All, damage);
    }
    [PunRPC]
    public void ActualTakeDamage(int damage)
    {
        i_currentHealth -= damage;
        if (i_currentHealth < 0)
            Die();
    }


    public bool IsDead()
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {
        throw new System.NotImplementedException();
    }
}
