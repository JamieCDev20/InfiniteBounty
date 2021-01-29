using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossProjectile : MonoBehaviourPunCallbacks
{
    protected Transform t_target;
    [SerializeField] protected int i_playerDamage;
    [SerializeField] protected int i_padDamage;
    [SerializeField] private bool b_isDestroyedOnImpact;

    public virtual void Setup(Transform _t_newTarget)
    {
        t_target = _t_newTarget;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
            collision.collider.GetComponent<IHitable>()?.TakeDamage(i_playerDamage, true);
        else if (collision.transform.CompareTag("Lilypad"))
            collision.collider.GetComponent<IHitable>()?.TakeDamage(i_padDamage, true);

        if (b_isDestroyedOnImpact)        
            Die();        
    }

    protected virtual void Die()
    {
        gameObject.SetActive(false);
    }
}
