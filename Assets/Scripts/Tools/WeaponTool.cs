using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTool : ToolBase
{

    #region Serialized Fields
    [Header("Weapon Stats")]
    [SerializeField] protected int i_damage;
    [SerializeField] protected int i_weight;
    [SerializeField] protected int i_lodeDamage;
    [SerializeField] protected float f_recoil;
    [SerializeField] protected float f_speed;
    [SerializeField] protected float f_knockback;
    [SerializeField] protected GameObject go_hitBox;
    [Header("Elemental Stats")]
    [SerializeField] protected int i_elementDamage;
    [SerializeField] protected float f_elementDuration;
    [SerializeField] protected float f_elementFrequency;
    [Header("Explosion Stats")]
    [SerializeField] protected GameObject go_explosion;
    [SerializeField] protected float f_explosionRadius;
    [SerializeField] protected float f_explosionKnockback;
    #endregion

    #region Serialized Privates
    [SerializeField] private string s_meleeAnim;
    [SerializeField] private Animator a_playerAnims;
    #endregion

    // Detonation is if it explodes immediately, on impact or on a timer
    #region Protected
    [SerializeField] protected bool b_rackUpgrade;
    [SerializeField] protected Collider c_playerCollider;
    #endregion


    #region Get/Sets
    public bool RackUpgrade { get { return b_rackUpgrade; } set { b_rackUpgrade = value; } }
    #endregion

    protected void OnEnable()
    {

    }
    public override void Use()
    {

    }

    public override void Use(Vector3 _v_forwards)
    {
        //SetAnimBool(true);        
        transform.root.forward = Vector3.Scale(_v_forwards, Vector3.one - Vector3.up);
    }

    public void SetAnimBool(bool _b_)
    {
        a_playerAnims.SetBool(s_meleeAnim, _b_);
    }

    public void GetStatChanges()
    {

    }
}
