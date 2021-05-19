using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NGoapAgent : MonoBehaviourPun, IHitable, IPoolable
{

    //Variables
    #region Serialised

    [Header("Living")]
    [SerializeField] private int i_maxHealth;
    [SerializeField] private int i_explosionDamage;

    [Header("Particles")]
    [SerializeField] private GameObject go_aggroParticles;
    [SerializeField] private GameObject go_deathParticles;
    [SerializeField] private GameObject go_exploParticles;
    [SerializeField] private ParticleSystem p_hitParticles;

    [SerializeField] private bool playParticles;

    #endregion

    #region Private

    internal int i_currentHealth;
    private bool b_isHost = true;
    internal Animator anim;
    internal Rigidbody rb;

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

    }

    protected virtual void FixedUpdate()
    {
        if (!b_isHost)
            return;
        anim.SetBool("Running", rb.velocity.magnitude >= 0.1f);
        if (Vector3.Scale(rb.velocity, Vector3.one - Vector3.up).magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(Vector3.Scale(rb.velocity, Vector3.one - Vector3.up), Vector3.up);

    }

    private void OnCollisionEnter(Collision collision)
    {
        //if()
    }

    #endregion

    #region Private Voids

    protected virtual void Init()
    {
        b_isHost = PhotonNetwork.IsMasterClient;

        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        i_currentHealth = i_maxHealth;
    }

    protected virtual void Attack()
    {
        anim.SetBool("Jump", true);
        anim.SetBool("Spin", true);
    }

    #endregion

    #region Public Voids

    [PunRPC]
    public void SetParticles(bool val)
    {
        if (playParticles)
            go_aggroParticles.SetActive(val);
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        if (!b_isHost)
        {
            photonView.RPC("RemoteTakeDamage", RpcTarget.Others, damage, activatesThunder);
        }
        i_currentHealth -= damage;

        if (p_hitParticles && playParticles)
            p_hitParticles?.Play();

        if (i_currentHealth <= 0)
        {
            photonView.RPC("Die", RpcTarget.AllViaServer);
        }
    }
    public void TakeDamage(int damage, bool activatesThunder, float _delay)
    {
        StartCoroutine(DelayedTakeDamage(damage, activatesThunder, _delay));
    }

    IEnumerator DelayedTakeDamage(int damage, bool activatesThunder, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        TakeDamage(damage, activatesThunder);
    }

    [PunRPC]
    public void RemoteTakeDamage(int damage, bool activatesThunder)
    {
        if (!b_isHost)
            return;
        TakeDamage(damage, activatesThunder);

    }

    [PunRPC]
    public void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1);
        foreach (Collider c in hits)
        {
            c.GetComponent<IHitable>()?.TakeDamage(i_explosionDamage, true);
        }
        Die();
        SplosionFX();
    }

    [PunRPC]
    public void SplosionFX()
    {
        if (playParticles)
        {
            go_exploParticles?.SetActive(true);
            go_exploParticles.transform.parent = null;
            Invoke("SetExploPartsInactive", 2);

        }

    }

    public void SetExploPartsInactive()
    {
        go_exploParticles?.SetActive(false);
    }

    public void SetDeathPartsInactive()
    {
        if (playParticles)
            go_deathParticles?.SetActive(false);
    }

    #endregion

    #region Private Returns



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
            if (playParticles)
            {
                go_deathParticles.SetActive(true);
                go_deathParticles.transform.parent = null;

            }
        }

        gameObject.SetActive(false);
        Invoke("SetDeathPartsInactive", 2);
        PoolManager.x.ReturnObjectToPool(gameObject);
        //EnemySpawner.x?.EnemyDied();
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

    public bool IsNug()
    {
        return false;
    }

}