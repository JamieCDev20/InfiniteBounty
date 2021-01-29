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
    [SerializeField] private GameObject[] goA_projectiles = new GameObject[3];
    private int i_currentTarget;
    [SerializeField] private int i_currentHealth;
    private bool b_isHost;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        PhotonNetwork.RegisterPhotonView(view);

        //Get the players as targets
        GameObject[] _goA = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < _goA.Length; i++)
            tL_potentialTargets.Add(_goA[i].transform);

        if (PhotonNetwork.IsMasterClient)
        {
            Invoke("BeginAttacks", 5);
            b_isHost = true;
        }

    }

    private void Update()
    {
        if (b_isAttacking)
        {
            f_currentTimer += Time.deltaTime;
            f_currentMoveTimer += Time.deltaTime;

            if (f_currentMoveTimer > f_timeBetweenMoves) view.RPC("MoveAttack", RpcTarget.All, GenerateRandomArenaPos());
            if (f_currentTimer > f_timeBetweenAttacks) PickAttack();
        }
        transform.LookAt(new Vector3(tL_potentialTargets[i_currentTarget].position.x, transform.position.y, tL_potentialTargets[i_currentTarget].position.z));
    }

    private Vector3 GenerateRandomArenaPos()
    {
        Vector3 _v_direction = new Vector3(Random.value, 0, Random.value).normalized;
        _v_direction *= Random.Range(0, 75);
        return _v_direction;
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

        for (int i = 0; i < 60; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position += Vector3.up * 0.5f;
        }

        b_isAttacking = true;
        f_currentMoveTimer = 0;
    }



    private void PickAttack()
    {
        int _i_moveIndex = Random.Range(0, 3);

        switch (_i_moveIndex)
        {
            default:
                view.RPC("RangedAttack", RpcTarget.All, _i_moveIndex);
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
    private void RangedAttack(int _i_whichProjectile)
    {
        if (_i_whichProjectile == 0) //Homing Attack
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject _go = Instantiate(goA_projectiles[_i_whichProjectile]);
                _go.GetComponent<BossProjectile>().Setup(tL_potentialTargets[i_currentTarget]);
                _go.transform.position = transform.position + (transform.forward * i);
                _go.transform.forward = transform.forward;
            }
        }
        else
        {
            GameObject _go = Instantiate(goA_projectiles[_i_whichProjectile]);
            _go.GetComponent<BossProjectile>().Setup(tL_potentialTargets[i_currentTarget]);
            _go.transform.position = transform.position + transform.forward * 3;
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
