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
    [SerializeField] protected GameObject[] go_hitBox;
    [Header("Elemental Stats")]
    [SerializeField] protected int i_elementDamage;
    [SerializeField] protected float f_elementDuration;
    [SerializeField] protected float f_elementFrequency;
    [Header("Explosion Stats")]
    [SerializeField] protected AugmentExplosion ae_explode;
    [SerializeField] protected GameObject[] go_explarticles;
    #endregion
    [SerializeField] protected bool b_rackUpgrade;
    [SerializeField] protected Collider c_playerCollider;
    [SerializeField] protected Transform[] A_augmentSlots;

    [SerializeField] protected TrailRenderer tr_trail;

    #region Serialized Privates
    [SerializeField] private string s_meleeAnim;
    [SerializeField] private Animator a_playerAnims;
    private const AugmentType augType = AugmentType.standard;
    public AugmentType AugType { get { return augType; } }
    #endregion

    // Detonation is if it explodes immediately, on impact or on a timer
    #region Protected
    [SerializeField] protected Augment[] A_augs;
    #endregion


    #region Get/Sets
    public bool RackUpgrade { get { return b_rackUpgrade; } set { b_rackUpgrade = value; } }
    #endregion

    protected void OnEnable()
    {
        A_augs = new Augment[A_augmentSlots.Length];
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

    protected bool CheckAnimPlaying(string _s_animName)
    {
        if (a_playerAnims.GetCurrentAnimatorStateInfo(0).IsName(_s_animName))
        {
            return true;
        }
        else return false;
    }

    public virtual bool AddStatChanges(Augment aug)
    {
        if (A_augs != null && A_augs.Length >= 0)
        {
            int i = GetInactiveAugmentIndex();
            if (i == -1)
                return false;
            A_augs[i] = aug;
            AddToAugmentProperties(aug.GetAugmentProperties());
            AddToPhysicalProperties(aug.GetPhysicalProperties());
            AddToAudioProperties(aug.GetAudioProperties());
            AddToExplosionProperties(aug.GetExplosionProperties());
            return true;
        }
        return false;

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
        {
            if (A_augs[i] == null)
                return i;
            else if (A_augs[i].Name == null)
                return i;
        }
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
        if(tr_trail != null)
        {
            tr_trail.startWidth = ap.f_trWidth;
            tr_trail.endWidth = ap.f_trWidth;
        }
        // Add the keys here
        //tr_trail.colorGradient.SetKeys(new GradientColorKey(ap.A_trKeys));
        if(this is ProjectileTool)
            Utils.AddToArray<GameObject>(go_hitBox, Resources.Load<GameObject>(ap.go_projectile));

    }

    protected void AddToAudioProperties(List<string[]> _sL_audio)
    {
        string[] use = _sL_audio[0];
        string[] travel = _sL_audio[1];
        string[] hit = _sL_audio[2];
        if(use != null)
            LoadAndAddObjectArray(ac_activationSound, use);
        if (travel != null)
            LoadAndAddObjectArray(ac_diegeticAudio, travel);
        if (hit != null)
            LoadAndAddObjectArray<AudioClip>(ac_hitSound, hit);
    }

    protected void AddToExplosionProperties(AugmentExplosion ae)
    {
        ae_explode.b_impact = ae_explode.b_impact == true || ae.b_impact == true ? true : false;
        ae_explode.f_detonationTime += ae.f_detonationTime;
        ae_explode.f_explockBack += ae.f_explockBack;
        ae_explode.f_radius += ae.f_radius;
        ae_explode.i_damage += ae.i_damage;
        ae_explode.i_lodeDamage += ae.i_lodeDamage;

        LoadAndAddObjectArray(go_explarticles, ae.go_explarticles);
    }

    protected void LoadAndAddObjectArray<T>(T[] _tA_existingObjects, string[] _s_newObjectPaths) where T : Object
    {
        T[] tempObjects = new T[_s_newObjectPaths.Length];
        for (int i = 0; i < _s_newObjectPaths.Length; i++)
            tempObjects[i] = Resources.Load<T>(_s_newObjectPaths[i]);
        _tA_existingObjects = Utils.CombineArrays(_tA_existingObjects, tempObjects);
    }
}
