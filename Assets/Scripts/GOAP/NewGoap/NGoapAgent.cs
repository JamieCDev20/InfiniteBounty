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

    [Header("Particles")]
    [SerializeField] private GameObject go_aggroParticles;
    [SerializeField] private GameObject go_deathParticles;

    #endregion

    #region Private

    private int i_currentHealth;
    private AIGroundMover mover;

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
        }
    }

    private void FixedUpdate()
    {
        mover.Move();
    }

    #endregion

    #region Private Voids

    private void Init()
    {

        i_currentHealth = i_maxHealth;
        targetting.SetTransform(transform);

        switch (movementType)
        {
            case AIMovementType.ground:
                mover = new AIGroundMover(movementStats);
                mover.SetRB(GetComponent<Rigidbody>());
                break;
            case AIMovementType.air:
                break;
            default:
                break;
        }
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

    #endregion

    #region Private Returns


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
    }

    public bool IsNetworkedObject()
    {
        return true;
    }

    public string ResourcePath()
    {
        return null;
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