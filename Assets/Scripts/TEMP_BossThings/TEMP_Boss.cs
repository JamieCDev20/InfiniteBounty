using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_Boss : MonoBehaviourPunCallbacks, IHitable
{
    private PhotonView view;
    internal List<Transform> tL_potentialTarget = new List<Transform>();
    private bool b_isAttacking;
    [Space, SerializeField] private float f_timeBetweenAttacks = 6;
    private float f_currentTimer;
    [SerializeField] private float f_timeBetweenMoves = 15;
    private float f_currentMoveTimer;
    private int i_currentTarget;
    [SerializeField] private int i_currentHealth;
    private bool b_isHost;
    [SerializeField] private LayerMask lm_playerLayer;

    [Header("Homing Attack")]
    [SerializeField] private GameObject go_homingPrefab;
    [SerializeField] private float f_homingForwardMovement;
    [SerializeField] private Vector2 v_shotsPerHomingRound;
    [SerializeField] private string s_homingMissilePath;


    private void Start()
    {
        view = GetComponent<PhotonView>();
        PhotonNetwork.RegisterPhotonView(view);


        if (PhotonNetwork.IsMasterClient)
        {
            //Get the players as targets
            GameObject[] _goA = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < _goA.Length; i++)
                tL_potentialTarget.Add(_goA[i].transform);

            Invoke(nameof(BeginAttacks), 7);
            b_isHost = true;
        }
        Invoke(nameof(SetHealth), 0.5f);

    }

    private void SetHealth()
    {
        i_currentHealth *= tL_potentialTarget.Count;
    }

    private void Update()
    {
        if (b_isAttacking)
        {
            f_currentTimer += Time.deltaTime;
            f_currentMoveTimer += Time.deltaTime;

            if (f_currentMoveTimer > f_timeBetweenMoves) view.RPC("MoveAttack", RpcTarget.All, PickTargetPosition(), Random.Range(1f, 5f));
            if (f_currentTimer > f_timeBetweenAttacks) PickAttack();
        }
        transform.LookAt(new Vector3(tL_potentialTarget[i_currentTarget].position.x, transform.position.y, tL_potentialTarget[i_currentTarget].position.z));
    }

    private Vector3 PickTargetPosition()
    {
        Vector3 _v = tL_potentialTarget[Random.Range(0, tL_potentialTarget.Count)].position;
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
    private void MoveAttack(Vector3 _v_newPos, float _f_timeToWait)
    {
        b_isAttacking = false;
        if (b_isHost)
            view.RPC("ChangeTarget", RpcTarget.All, Random.Range(0, tL_potentialTarget.Count));
        StartCoroutine(TimedMove(_v_newPos, _f_timeToWait));
    }
    private IEnumerator TimedMove(Vector3 _v_newPos, float _f_timeToWait)
    {
        for (int i = 0; i < 60; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position += Vector3.down * 0.5f;
        }

        yield return new WaitForSeconds(_f_timeToWait);

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
                StartCoroutine(HomingAttack(Mathf.RoundToInt(Random.Range(v_shotsPerHomingRound.x, v_shotsPerHomingRound.y)), tL_potentialTarget[Random.Range(0, tL_potentialTarget.Count)]));
                break;
            case 1:
                view.RPC("MortarAttack", RpcTarget.All);
                break;
            case 2:
                view.RPC("MeleeAttack", RpcTarget.All);
                break;
        }
        f_currentTimer = 0;
        if (b_isHost)
            view.RPC("ChangeTarget", RpcTarget.All, Random.Range(0, tL_potentialTarget.Count));
    }

    [PunRPC]
    private void MortarAttack()
    {

    }

    private IEnumerator HomingAttack(int _i_amount, Transform _t_target)
    {
        List<GameObject> _goL_orbs = new List<GameObject>();
        for (int i = 0; i < _i_amount; i++)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject _go = PhotonNetwork.Instantiate(s_homingMissilePath + go_homingPrefab.name, transform.position + transform.forward, Quaternion.identity);
            _go.GetComponent<BossProjectile>().Setup(tL_potentialTarget[i_currentTarget]);
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


    [PunRPC]
    private void MeleeAttack()
    {

    }


    public void TakeDamage(int damage, bool activatesThunder)
    {
        view.RPC("ActualTakeDamage", RpcTarget.All, damage);
    }
    [PunRPC]
    public void ActualTakeDamage(int damage)
    {
        print("I HAVE TAKEN SOME DAMAGE");
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
        Debug.LogError("I'm being told to die");
        gameObject.SetActive(false);
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {
        throw new System.NotImplementedException();
    }
}
