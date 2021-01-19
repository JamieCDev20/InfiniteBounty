using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTool : ToolBase
{

    #region Serialized Fields
    [Header("Weapon Stats")]
    [SerializeField] protected int i_damage;
    [SerializeField] protected int i_lodeDamage;
    [SerializeField] protected float f_weight;
    [SerializeField] protected float f_recoil;
    [SerializeField] protected float f_speed;
    [SerializeField] protected float f_heatsink;
    [SerializeField] protected float f_knockback;
    [SerializeField] protected float f_energyGauge;
    [SerializeField] protected GameObject go_hitBox;
    [Header("Elemental Stats")]
    [SerializeField] protected int i_elementDamage;
    [SerializeField] protected float f_elementDuration;
    [SerializeField] protected float f_elementFrequency;
    [Header("Explosion Stats")]
    [SerializeField] protected AugmentExplosion ae_explode;
    #endregion
    [SerializeField] protected bool b_rackUpgrade;
    [SerializeField] protected Collider c_playerCollider;
    [SerializeField] protected Transform[] A_augmentSlots;

    [SerializeField] protected TrailRenderer tr_trail;

    #region Serialized Privates
    [SerializeField] private string s_meleeAnim;
    [SerializeField] private Animator a_playerAnims;
    #endregion

    // Detonation is if it explodes immediately, on impact or on a timer
    #region Protected
    protected Augment[] A_augs;
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

    public virtual void AddStatChanges(Augment aug)
    {
        A_augs[GetInactiveAugmentIndex()] = aug;
        // Set the properties
        AugmentProperties ap = aug.GetAugmentProperties();
        AddToAugmentProperties(ap);
        // Set the physical
        AugmentPhysicals aPhys = aug.GetPhysicalProperties();
        AddToPhysicalProperties(aPhys);


    }

    public virtual void RemoveStatChanges(Augment aug)
    {

    }

    public Transform GetInactiveAugmentSlot()
    {
        return A_augmentSlots[GetInactiveAugmentIndex()] != null ? A_augmentSlots[GetInactiveAugmentIndex()] : null;
    }

    public int GetInactiveAugmentIndex()
    {
        for (int i = 0; i < A_augs.Length; i++)
            if (A_augs[i] == null)
                return i;
        return -1;
    }

    private void AddToAugmentProperties(AugmentProperties ap)
    {
        i_damage        += ap.i_damage;
        i_lodeDamage    += ap.i_lodeDamage;
        f_weight        += ap.f_weight;
        f_recoil        += ap.f_recoil;
        f_speed         += ap.f_speed;
        f_heatsink      += ap.f_heatsink;
        f_knockback     += ap.f_knockback;
        f_energyGauge   += ap.f_energyGauge;
    }

    private void AddToPhysicalProperties(AugmentPhysicals ap)
    {
        tr_trail.startWidth = ap.f_trWidth;
        tr_trail.endWidth = ap.f_trWidth;
        // Add the keys here
        // ap.A_trKeys;
    }
}
