using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NGoapAgent : MonoBehaviourPun, IHitable, IPoolable
{

    //Variables
    #region Serialised
    [Header("Movement")]
    [SerializeField] private AIMovementType movementType;
    [SerializeField] private AIMovementStats movementStats;

    [Space]
    [Header("Living")]
    [SerializeField] private int i_maxHealth;

    [Space]
    [Header("Combat")]
    [SerializeField] private AITargetting targetting;
    [SerializeField] internal int i_damage = 5;
    [SerializeField] internal float f_attackRange = 3;
    [SerializeField] internal float f_lungeForce = 10;

    [Header("Particles")]
    [SerializeField] private GameObject go_aggroParticles;
    [SerializeField] private GameObject go_deathParticles;
    [SerializeField] private GameObject go_exploParticles;
    [SerializeField] private ParticleSystem p_hitParticles;

    #endregion

    #region Private

    internal int i_currentHealth;
    internal bool b_canAttack = true;
    private bool b_isHost = true;
    internal Animator anim;
    internal AIGroundMover mover;
    internal Rigidbody rb;
    internal Transform target;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (!b_isHost)
            return;
        if (!CanSeeTarget())
            target = targetting.GetTarget();
        if (b_canAttack && target != null && (target.position - transform.position).magnitude < f_attackRange)
            Attack();

    }

    protected virtual void FixedUpdate()
    {
        if (!b_isHost)
            return;
        if (!mover.HasPath())
            mover.Retarget(target, true);
        anim.SetBool("Running", rb.velocity.magnitude >= 0.1f);
        if (Vector3.Scale(rb.velocity, Vector3.one - Vector3.up).magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(Vector3.Scale(rb.velocity, Vector3.one - Vector3.up), Vector3.up);
        //if (!go_aggroParticles.activeSelf && target != null)
        //    photonView.RPC("SetParticles", RpcTarget.All, true);
        //else if (go_aggroParticles.activeSelf && target == null)
        //    photonView.RPC("SetParticles", RpcTarget.All, false);

        mover.Move();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (b_canAttack)
            return;
        if (rb.velocity.sqrMagnitude > 15)
            photonView.RPC("Explode", RpcTarget.All);
        //Explode();
    }

    #endregion

    #region Private Voids

    protected virtual void Init()
    {
        b_isHost = PhotonNetwork.IsMasterClient;

        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        i_currentHealth = i_maxHealth;
        targetting.SetTransform(transform);

        switch (movementType)
        {
            case AIMovementType.ground:
                mover = new AIGroundMover(movementStats);
                mover.SetRB(rb);
                break;
            case AIMovementType.air:
                break;
            default:
                break;
        }
    }

    protected virtual void Attack()
    {
        mover.SetCanMove(false);
        anim.SetBool("Jump", true);
        anim.SetBool("Spin", true);
        rb.AddForce(((targetting.GetTarget().position + (Vector3.up * 1.2f)) - transform.position).normalized * f_lungeForce, ForceMode.Impulse);
        b_canAttack = false;
    }

    #endregion

    #region Public Voids

    [PunRPC]
    public void SetParticles(bool val)
    {
        go_aggroParticles.SetActive(val);
    }

    public void SetTarget(Transform target)
    {
        mover.Retarget(target, true);
    }

    public void SetTarget(Vector3 target)
    {
        mover.Retarget(target, true);
    }

    public void TakeDamage(int damage)
    {
        if (!b_isHost)
        {
            photonView.RPC("RemoteTakeDamage", RpcTarget.Others, damage);
        }
        i_currentHealth -= damage;

        if (p_hitParticles)
            p_hitParticles?.Play();

        if (i_currentHealth <= 0)
        {
            photonView.RPC("Die", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void RemoteTakeDamage(int damage)
    {
        if (!b_isHost)
            return;
        TakeDamage(damage);

    }

    [PunRPC]
    public void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1);
        foreach (Collider c in hits)
        {
            c.GetComponent<IHitable>()?.TakeDamage(i_damage);
        }
        //photonView.RPC("SplosionFX", RpcTarget.All);
        //photonView.RPC("Die", RpcTarget.AllViaServer);
        Die();
        SplosionFX();
    }

    [PunRPC]
    public void SplosionFX()
    {
        go_exploParticles?.SetActive(true);
        go_exploParticles.transform.parent = null;

    }

    #endregion

    #region Private Returns

    private bool CanSeeTarget()
    {
        if (target == null)
            return false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit))
        {
            return hit.collider.transform == target;
        }
        return false;
    }

    private float PathLength(NavMeshPath path)
    {

        float distance = 0;

        for (int i = 1; i < path.corners.Length; i++)
        {
            distance += (path.corners[i - 1] - path.corners[i]).magnitude;
        }

        return distance;
    }

    #endregion

    #region Public Returns

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    [PunRPC]
    public void Die()
    {
        if (go_deathParticles)
        {
            go_deathParticles.SetActive(true);
            go_deathParticles.transform.parent = null;
        }

        gameObject.SetActive(false);
        PoolManager.x.ReturnObjectToPool(gameObject);
        EnemySpawner.x?.EnemyDied();
    }

    public bool IsNetworkedObject()
    {
        return true;
    }

    public string ResourcePath()
    {
        return "";
    }

    public bool IsDead()
    {
        return true;
    }

    #endregion

}

public enum AIMovementType
{
    ground,
    air
}

[System.Serializable]
public struct AIMovementStats
{
    public float f_movementSpeed;
    public float f_jumpHeight;
    public Vector3 v_drag;

}