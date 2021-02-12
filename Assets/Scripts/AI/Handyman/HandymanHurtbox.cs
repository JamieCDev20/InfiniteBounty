using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandymanHurtbox : MonoBehaviour
{

    [SerializeField] private float f_hitForce;
    [SerializeField] private int f_damage;

    [SerializeField] private Transform parent;

    private bool b_active;

    private void OnTriggerEnter(Collider other)
    {
        if (!b_active)
            return;
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<PlayerMover>().HitKnockback(parent.forward, f_hitForce);
            other.GetComponent<IHitable>().TakeDamage(f_damage, false);
        }
    }

    public void SetHurtboxActive(bool active)
    {
        b_active = active;
    }

}
