using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossProjectile : MonoBehaviourPunCallbacks
{
    protected Transform t_target;
    [SerializeField] protected int i_playerDamage;
    protected int i_actualDamage;
    [SerializeField] protected int i_padDamage;
    [SerializeField] private bool b_isDestroyedOnImpact;


    private void Start()
    {
        i_actualDamage = Mathf.RoundToInt(i_playerDamage * DifficultyManager.x.ReturnCurrentDifficulty().f_damageMult);
    }

    public virtual void Setup(Transform _t_newTarget)
    {
        t_target = _t_newTarget;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
            collision.collider.GetComponent<IHitable>()?.TakeDamage(i_actualDamage, true);
        else if (collision.transform.CompareTag("Lilypad"))
            collision.collider.GetComponent<IHitable>()?.TakeDamage(i_padDamage, true);

        if (b_isDestroyedOnImpact)
            Die();
    }

    internal virtual void Die()
    {
        gameObject.SetActive(false);
    }
}
