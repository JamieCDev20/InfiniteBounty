using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRipple : BossProjectile
{

    [Header("Ripple")]
    [SerializeField] private float f_growthRate;

    void Update()
    {
        transform.localScale += f_growthRate * Time.deltaTime * (Vector3.one - (Vector3.up * 0.5f));
    }
}
