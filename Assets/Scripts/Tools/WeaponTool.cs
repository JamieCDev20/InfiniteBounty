using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTool : ToolBase
{
    [Header("Weapon Stats")]
    [SerializeField] protected int i_damage;
    [SerializeField] protected int i_weight;
    [SerializeField] protected float f_recoil;
    [SerializeField] protected float f_speed;
    [SerializeField] protected float f_knockback;
    [SerializeField] protected float f_lodeScalar;
    [SerializeField] protected GameObject go_hitBox;

    [Header("Elemental Stats")]
    [SerializeField] Systemic sy_element;
    [SerializeField] protected int i_elementDamage;
    [SerializeField] protected float f_elementDuration;
    [SerializeField] protected float f_elementFrequency;
    [Header("Explosion Stats")]
    [SerializeField] GameObject go_explosion;
    [SerializeField] float f_radius;
    [SerializeField] float f_explosionKnockback;

    // Detonation is if it explodes immediately, on impact or on a timer
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Use()
    {

    }

}
