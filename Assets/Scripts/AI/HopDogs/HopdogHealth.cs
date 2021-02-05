using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopdogHealth : MonoBehaviourPun, IHitable
{

    [SerializeField] private int i_maxHealth = 10;
    [SerializeField] private int i_explosionDamage = 5;
    [SerializeField] private float f_explosionRadius = 1.5f;
    [SerializeField] private float f_impactExplosionVelocty = 10;

    [Header("Particles")]
    [SerializeField] private GameObject go_aggroParticles;
    [SerializeField] private GameObject go_deathParticles;
    [SerializeField] private GameObject go_exploParticles;
    [SerializeField] private ParticleSystem p_hitParticles;

    private float i_currentHealth;
    private bool b_isHost = true;
    private bool b_isDead;
    private ElementalObject eo_elemO;
    private Rigidbody rb;

    private void Start()
    {
        eo_elemO = GetComponent<ElementalObject>();
        i_currentHealth = i_maxHealth;
        b_isHost = PhotonNetwork.IsMasterClient;
        rb = GetComponent<Rigidbody>();
        //Invoke(nameof(TimedDeath), 30);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rb != null)
            if (Vector3.Dot((collision.contacts[0].point - transform.position).normalized, rb.velocity.normalized) < 0.3f)
                if (rb.velocity.sqrMagnitude > f_impactExplosionVelocty * f_impactExplosionVelocty)
                    photonView.RPC("Explode", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RemoteTakeDamage(int damage, bool activatesThunder)
    {
        if (!b_isHost)
            return;
        TakeDamage(damage, activatesThunder);

    }

    private void TimedDeath()
    {
        TakeDamage(10000, true);
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
        //Invoke("SetDeathPartsInactive", 2);
        PoolManager.x.ReturnObjectToPool(gameObject);
        EnemySpawner.x?.EnemyDied();
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {
        StartCoroutine(DelayedDamage(damage, activatesThunder, delay));
    }

    IEnumerator DelayedDamage(int _dmg, bool _thunderAc, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        TakeDamage(_dmg, _thunderAc);
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        if (!b_isHost)
        {
            photonView.RPC("RemoteTakeDamage", RpcTarget.Others, damage, activatesThunder);
        }
        i_currentHealth -= damage;

        p_hitParticles?.Play();

        if (i_currentHealth <= 0)
        {
            photonView.RPC("Die", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, f_explosionRadius);
        foreach (Collider c in hits)
        {
            c.GetComponent<IHitable>()?.TakeDamage(i_explosionDamage, true);
        }
        Die();
        SplosionFX();
    }

    public void SplosionFX()
    {

        go_exploParticles?.SetActive(true);
        go_exploParticles.transform.parent = null;
        //Invoke("SetExploPartsInactive", 2);

    }

    public bool IsDead()
    {
        return true;
    }

}
