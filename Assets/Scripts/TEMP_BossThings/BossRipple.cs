using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRipple : BossProjectile
{

    [Header("Ripple")]
    [SerializeField] private float f_growthRate;

    private void Start()
    {
        if (f_growthRate > 0)
            transform.position -= Vector3.up * 5;
    }

    void Update()
    {
        transform.localScale += f_growthRate * Time.deltaTime * (Vector3.one - (Vector3.up * 0.6f));
    }
}
