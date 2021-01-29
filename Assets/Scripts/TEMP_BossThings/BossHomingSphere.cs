using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHomingSphere : BossProjectile
{
    [Header("Homing Sphere")]
    [SerializeField] private float f_forwardMovement;
    [SerializeField] private GameObject go_explosionEffect;

    private void Update()
    {
        transform.position += f_forwardMovement * Time.deltaTime * transform.forward;
        transform.LookAt(t_target);
    }

    protected override void Die()
    {
        go_explosionEffect.SetActive(false);
        go_explosionEffect.transform.parent = null;
        go_explosionEffect.transform.position = transform.position;
        go_explosionEffect.SetActive(true);

        base.Die();
    }
}
