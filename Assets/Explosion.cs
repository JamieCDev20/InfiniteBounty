﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float f_radius = 2;
    [SerializeField] private int i_damage;
    [SerializeField] private int i_lodeDamage;
    [SerializeField] private float f_knockBack;
    [SerializeField] private LayerMask lm_blastMask;

    internal void OnEnable()
    {
        print("Explsoding");
        Collider[] _cA = Physics.OverlapSphere(transform.position, f_radius, lm_blastMask);

        for (int i = 0; i < _cA.Length; i++)
        {
            IHitable _i = _cA[i].GetComponent<IHitable>();
            if (_i != null)
            {
                if (_i is LodeBase)
                {
                    print("Hit Lode");
                    _i.TakeDamage(i_lodeDamage);
                }
                else
                {
                    print("Hit " + _cA[i].name);
                    _i.TakeDamage(i_damage);
                    _cA[i].GetComponent<Rigidbody>().AddExplosionForce(f_knockBack, transform.position, f_radius, -1);
                }
            }
        }
    }
}
