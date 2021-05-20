using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTool : ToolBase
{

    #region Serialized Fields
    [Header("Weapon Stats")]
    [SerializeField] protected int i_damage;
    [SerializeField] protected int i_lodeDamage;
    [SerializeField] internal float f_weight;
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
    public virtual AugmentType AugType { get { return augType; } }
    #endregion

    // Detonation is if it explodes immediately, on impact or on a timer
    #region Protected
    [SerializeField] protected Augment[] A_augs;
    [SerializeField] protected GameObject augGo;
    protected Color[] c_trail = new Color[0];
    protected GameObject[] physicals = new GameObject[8];
    protected Element[] eo_element;
    
    #endregion


    #region Get/Sets
    public bool RackUpgrade { get { return b_rackUpgrade; } set { b_rackUpgrade = value; } }
    public Augment[] Augs { get { return A_augs; } set { A_augs = value; } }
    #endregion


    public void InitAugmentArrayBlank()
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

        a_playerAnims?.SetBool(s_meleeAnim, _b_);
    }

    public void SetAnimName(string nam)
    {
        s_meleeAnim = nam;
    }

    protected bool CheckAnimPlaying(string _s_animName)
    {
        if (a_playerAnims.GetCurrentAnimatorStateInfo(0).IsName(_s_animName))
        {
            return true;
        }
        else return false;
    }

    protected float GetAugmentLevelModifier(int level)
    {
        return (1 / ((0.00053f * -level) - 0.01969f)) + 50.46f;
    }

    public virtual bool AddStatChanges(Augment aug)
    {
        if (A_augs != null && A_augs.Length >= 0)
        {
            int i = GetInactiveAugmentIndex();
            if (i == -1)
                return false;

            float mod = GetAugmentLevelModifier(aug.Level);

            GameObject augmentGameObject = PoolManager.x.SpawnObject(augGo);
            augmentGameObject.transform.parent = A_augmentSlots[i].parent;
            augmentGameObject.transform.rotation = A_augmentSlots[i].transform.rotation;
            augmentGameObject.transform.position = A_augmentSlots[i].transform.position;
            augmentGameObject.GetComponent<Rigidbody>().isKinematic = true;
            augmentGameObject.GetComponent<Collider>().isTrigger = true;
            augmentGameObject.GetComponent<AugmentGo>().ApplyMaterial(aug.AugmentMaterial);

            physicals[i] = augmentGameObject;
            AugmentGo actualGo = augmentGameObject.GetComponent<AugmentGo>();
            actualGo.Aug = aug;
            actualGo.Mat = Resources.Load<Material>(aug.AugmentMaterial);

            A_augs[i] = aug;
            AddToAugmentProperties(aug.GetAugmentProperties(), mod);
            AddToPhysicalProperties(aug.GetPhysicalProperties());
            AddToAudioProperties(aug.GetAudioProperties());
            AddToExplosionProperties(aug.GetExplosionProperties(), mod);
            return true;
        }
        return false;

    }

    public virtual void RemoveStatChanges(Augment aug)
    {
        if(A_augs != null && A_augs.Length >= 0)
        {
            (string nam, int lev) thing = (aug.Name, aug.Level);

            int i = -1;
            for (int j = 0; j < Augs.Length; j++)
            {
                if(Augs[j]?.Tup == thing)
                {
                    i = j;
                    break;
                }
            }

            float mod = GetAugmentLevelModifier(aug.Level);

            physicals[i].SendMessage("Die");
            physicals[i] = null;

            A_augs[i] = null;

            RemoveToAugmentProperties(aug.GetAugmentProperties(), mod);
            RemoveToPhysicalProperties(aug.GetPhysicalProperties());
            RemoveToAudioProperties(aug.GetAudioProperties());
            RemoveToExplosionProperties(aug.GetExplosionProperties(), mod);

        }

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

    //add
    private void AddToAugmentProperties(AugmentProperties ap, float mod)
    {
        i_damage += Mathf.RoundToInt(mod * ap.i_damage);
        i_lodeDamage += Mathf.RoundToInt(mod * ap.i_lodeDamage);
        f_weight += mod * ap.f_weight;
        f_recoil += mod * ap.f_recoil;
        f_speed += mod * ap.f_speed;
        f_heatsink += mod * ap.f_heatsink;
        f_knockback += mod * ap.f_knockback;
        f_energyGauge = Mathf.Clamp(f_energyGauge + mod * ap.f_energyGauge, 10, Mathf.Infinity);
        
    }

    private void AddToPhysicalProperties(AugmentPhysicals ap)
    {
        if (tr_trail != null)
        {
            tr_trail.startWidth = ap.f_trWidth;
            tr_trail.endWidth = ap.f_trWidth;
            tr_trail.time = ap.f_trLifetime;
        }
        // Add the keys here
        c_trail = Utils.CombineArrays(c_trail, ap.A_trKeys);
        if (this is ProjectileTool)
            Utils.AddToArray<GameObject>(go_hitBox, Resources.Load<GameObject>(ap.go_projectile));

    }

    protected void AddToAudioProperties(List<string[]> _sL_audio)
    {
        string[] use = _sL_audio[0];
        string[] travel = _sL_audio[1];
        string[] hit = _sL_audio[2];
        if (use != null)
            LoadAndAddObjectArray(ac_activationSound, use);
        if (travel != null)
            LoadAndAddObjectArray(ac_diegeticAudio, travel);
        if (hit != null)
            LoadAndAddObjectArray<AudioClip>(ac_hitSound, hit);
    }

    protected void AddToExplosionProperties(AugmentExplosion ae, float mod)
    {
        ae_explode.b_impact = ae_explode.b_impact == true || ae.b_impact == true ? true : false;
        ae_explode.f_detonationTime += ae.f_detonationTime;
        ae_explode.f_explockBack += ae.f_explockBack;
        ae_explode.f_radius += ae.f_radius;
        ae_explode.i_damage += ae.i_damage;
        ae_explode.i_lodeDamage += ae.i_lodeDamage;
        if (go_explarticles != null && go_explarticles.Length > 0)
            LoadAndAddObjectArray(go_explarticles, ae.go_explarticles);
    }

    //remove
    private void RemoveToAugmentProperties(AugmentProperties ap, float mod)
    {
        Debug.Log("Removing augment properties");
        Debug.Log(i_damage);
        i_damage -= Mathf.RoundToInt(mod * ap.i_damage);
        i_lodeDamage -= Mathf.RoundToInt(mod * ap.i_lodeDamage);
        f_weight -= mod * ap.f_weight;
        f_recoil -= mod * ap.f_recoil;
        f_speed -= mod * ap.f_speed;
        f_heatsink -= mod * ap.f_heatsink;
        f_knockback -= mod * ap.f_knockback;
        f_energyGauge -= mod * ap.f_energyGauge;
        Debug.Log(i_damage);
    }

    private void RemoveToPhysicalProperties(AugmentPhysicals ap)
    {
        Debug.Log("Removing physical properties");
        if (tr_trail != null)
        {
            //tr_trail.startWidth = ap.f_trWidth;
            //tr_trail.endWidth = ap.f_trWidth;
            //tr_trail.time = ap.f_trLifetime;
            //dont know what these were originally
        }
    }

    protected void RemoveToAudioProperties(List<string[]> _sL_audio)
    {
        //dont know how to undo these...
    }

    protected void RemoveToExplosionProperties(AugmentExplosion ae, float mod)
    {
        //some of these i dont know how to undo
        //ae_explode.b_impact = ae_explode.b_impact == true || ae.b_impact == true ? true : false;
        ae_explode.f_detonationTime -= ae.f_detonationTime;
        ae_explode.f_explockBack -= ae.f_explockBack;
        ae_explode.f_radius -= ae.f_radius;
        ae_explode.i_damage -= ae.i_damage;
        ae_explode.i_lodeDamage -= ae.i_lodeDamage;
        //if (go_explarticles != null && go_explarticles.Length > 0)
            //LoadAndAddObjectArray(go_explarticles, ae.go_explarticles);
    }

    protected void LoadAndAddObjectArray<T>(T[] _tA_existingObjects, string[] _s_newObjectPaths) where T : Object
    {
        T[] tempObjects = new T[_s_newObjectPaths.Length];
        for (int i = 0; i < _s_newObjectPaths.Length; i++)
            tempObjects[i] = Resources.Load<T>(_s_newObjectPaths[i]);
        _tA_existingObjects = Utils.CombineArrays(_tA_existingObjects, tempObjects);
    }

    public Augment[] GetAugments()
    {
        List<Augment> augs = new List<Augment>();
        for (int i = 0; i < A_augs.Length; i++)
        {
            if (A_augs[i] != null)
                augs.Add(A_augs[i]);
        }
        return augs.ToArray();
    }

}
