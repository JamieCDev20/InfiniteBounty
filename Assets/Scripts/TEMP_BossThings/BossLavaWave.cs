using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLavaWave : BossProjectile
{
    [Header("Lava Wave")]
    [SerializeField] private float f_forwardMovement;


    private void Update()
    {
        transform.position += f_forwardMovement * Time.deltaTime * transform.forward;
    }

}
