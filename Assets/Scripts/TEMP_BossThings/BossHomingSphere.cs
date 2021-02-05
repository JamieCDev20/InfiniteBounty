using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHomingSphere : BossProjectile
{
    [Header("Homing Sphere")]    
    [SerializeField] private GameObject go_explosionEffect;


    internal override void Die()
    {
        go_explosionEffect.SetActive(false);
        go_explosionEffect.transform.parent = null;
        go_explosionEffect.transform.position = transform.position;
        go_explosionEffect.SetActive(true);

        base.Die();
    }
}
