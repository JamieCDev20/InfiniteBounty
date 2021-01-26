using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRipple : BossProjectile
{

    [Header("Ripple")]
    [SerializeField] private float f_growthRate;

    private void Start()
    {
        transform.position -= Vector3.up * 7;
    }

    void Update()
    {
        transform.localScale += f_growthRate * Time.deltaTime * (Vector3.one);
    }
}
