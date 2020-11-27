using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NGoapAgent : MonoBehaviour, IHitable, IPunObservable, IPoolable
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
    [SerializeField] private int i_damage = 5;
    [SerializeField] private float f_attackRange = 3;
    [SerializeField] private float f_lungeForce = 10;

    [Header("Particles")]
    [SerializeField] private GameObject go_aggroParticles;
    [SerializeField] private GameObject go_deathParticles;
    [SerializeField] private GameObject go_exploParticles;

    #endregion

    #region Private

    private int i_currentHealth;
    private bool b_canAttack = true;
    private AIGroundMover mover;
    private Rigidbody rb;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (!mover.HasPath())
        {
            mover.Retarget(targetting.GetTarget(), true);
        }
        else
        {
            if (!go_aggroParticles.activeInHierarchy)
                go_aggroParticles.SetActive(true);
            if (b_canAttack && targetting.GetTarget() != null && (targetting.GetTarget().position - transform.position).magnitude < f_attackRange)
                Attack();
        }
    }

    private void FixedUpdate()
    {
        mover.Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (b_canAttack)
            return;
        if (rb.velocity.sqrMagnitude > 15)
            Explode();
    }

    #endregion

    #region Private Voids

    private void Init()
    {

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

    private void Attack()
    {
        mover.SetCanMove(false);
        rb.AddForce(((targetting.GetTarget().position + (Vector3.up * 1.2f)) - transform.position).normalized * f_lungeForce, ForceMode.Impulse);
        b_canAttack = false;
    }

    #endregion

    #region Public Voids

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
        i_currentHealth -= damage;
        if (i_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1);
        foreach (Collider c in hits)
        {
            c.GetComponent<IHitable>()?.TakeDamage(i_damage);
        }
        go_exploParticles?.SetActive(true);
        go_exploParticles.transform.parent = null;
        Die();
    }

    #endregion

    #region Private Returns

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

    public NavMeshPath Path()
    {
        return mover.Path();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Die()
    {
        go_deathParticles.SetActive(true);
        go_deathParticles.transform.parent = null;

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