using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHomingSphere : BossProjectile
{
    [Header("Homing Sphere")]
    [SerializeField] private float f_forwardMovement;

    private void Update()
    {
        transform.position += f_forwardMovement * Time.deltaTime * transform.forward;
        transform.LookAt(t_target);
    }
}
