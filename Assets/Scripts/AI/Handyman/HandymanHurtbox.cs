using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandymanHurtbox : MonoBehaviour
{

    [SerializeField] private float f_hitForce;
    [SerializeField] private int f_damage;
    private int i_actualDamage;

    [SerializeField] private Transform parent;
    private bool b_active;

    private void Start()
    {
        i_actualDamage = Mathf.RoundToInt(f_damage * DifficultyManager.x.ReturnCurrentDifficulty().f_damageMult);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!b_active)
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<PlayerMover>().HitKnockback(parent.forward, f_hitForce);
            other.GetComponent<IHitable>().TakeDamage(i_actualDamage, false);
        }
    }

    public void SetHurtboxActive(bool active)
    {
        b_active = active;
    }

}
