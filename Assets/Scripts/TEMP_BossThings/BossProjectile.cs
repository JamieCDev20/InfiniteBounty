using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossProjectile : MonoBehaviourPunCallbacks
{
    protected Transform t_target;
    [SerializeField] protected int i_damageOnImpact;
    [SerializeField] private bool b_isDestroyedOnImpact;

    public virtual void Setup(Transform _t_newTarget)
    {
        t_target = _t_newTarget;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        collision.collider.GetComponent<IHitable>()?.TakeDamage(i_damageOnImpact, true);
        if (b_isDestroyedOnImpact)
            gameObject.SetActive(false);
    }


}
