using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTool : ToolBase
{

    #region Serialized Fields
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
    #endregion

    // Detonation is if it explodes immediately, on impact or on a timer
    #region Protected
    protected Camera c_cam;
    protected bool b_rackUpgrade = false;
    #endregion

    #region Get/Sets
    public bool RackUpgrade { get{ return b_rackUpgrade; } set{ b_rackUpgrade = value; } }
    #endregion

    protected void OnEnable()
    {
        // Figure out a nicer way of doing this
        c_cam = Camera.main;
    }
    public override void Use()
    {

    }

}
