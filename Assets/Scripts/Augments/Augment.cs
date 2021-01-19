﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Augment
{
    [SerializeField] string s_name;
    public string Name { get { return s_name; } }
    #region Audio

    [SerializeField] protected AudioClip[] ac_useSound;
    [SerializeField] protected AudioClip[] ac_travelSound;
    [SerializeField] protected AudioClip[] ac_hitSound;

    #endregion

    #region Tool Information Properties

    [SerializeField] protected float f_weight;
    [SerializeField] protected float f_recoil;
    [SerializeField] protected float f_speed;
    [SerializeField] protected float f_heatsink;
    [SerializeField] protected float f_knockback;
    [SerializeField] protected float f_energyGauge;
    [SerializeField] protected int i_damage;
    [SerializeField] protected int i_lodeDamage;

    #endregion

    #region Tool Physical Properties

    [SerializeField] protected float f_trWidth;
    [SerializeField] protected float f_trLifetime;
    [SerializeField] protected Color[] A_trKeys;
    [SerializeField] protected GameObject go_weaponProjectile;

    #endregion

    #region EXPLOSION

    [SerializeField] protected int i_explosionDamage;
    [SerializeField] protected int i_expLodeDamage;
    [SerializeField] protected bool b_impact;
    [SerializeField] protected float f_explockBack;
    [SerializeField] protected float f_detonationTime;
    [SerializeField] protected float f_expRad;
    [SerializeField] protected GameObject[] go_explarticles;

    #endregion

    #region Elemental 

    [SerializeField] protected Element[] eo_element;
    public Element[] AugElement { get { return eo_element; } set { eo_element = value; } }

    #endregion

    public AugmentType at_type;

    public void InitAudio(AudioClip[] _ac_use, AudioClip[] _ac_travel, AudioClip[] _ac_hit)
    {
        ac_useSound = _ac_use;
        ac_travelSound = _ac_travel;
        ac_hitSound = _ac_hit;
    }

    public void InitInfo(float _f_weight, float _f_recoil, float _f_speed, float _f_heatsink, float _f_knockback, float _f_energy, int _i_damage, int _i_lode)
    {
        f_weight        = _f_weight;
        f_recoil        = _f_recoil;
        f_speed         = _f_speed;
        f_heatsink      = _f_heatsink;
        f_knockback     = _f_knockback;
        f_energyGauge   = _f_energy;
        i_damage        = _i_damage;
        i_lodeDamage    = _i_lode;
    }

    public void InitInfo(AugmentProperties _ap_data)
    {
        s_name      = _ap_data.s_name;
        f_weight    = _ap_data.f_weight;
        f_speed     = _ap_data.f_speed;
        f_heatsink  = _ap_data.f_heatsink;
        f_knockback = _ap_data.f_knockback;
        f_energyGauge = _ap_data.f_energyGauge;
        i_damage    = _ap_data.i_damage;
        i_lodeDamage = _ap_data.i_lodeDamage;
    }
    public void InitPhysical(float _f_width, float _f_lifetime, Color[] _a_keys, GameObject _go_projectile)
    {
        f_trWidth   = _f_width;
        f_trLifetime = _f_lifetime;
        A_trKeys    = _a_keys;
        go_weaponProjectile = _go_projectile;
    }
    public void InitPhysical(AugmentPhysicals _phys_aug)
    {
        f_trWidth = _phys_aug.f_trWidth;
        f_trLifetime = _phys_aug.f_trLifetime;
        if(_phys_aug.A_trKeys != null)
            A_trKeys = _phys_aug.A_trKeys;
        go_weaponProjectile = _phys_aug.go_projectile;
    }

    public void InitExplosion(int _i_dmg, int _i_lodedmg, float _f_knockback, float _f_detTime, float _f_rad, bool _b_imp, GameObject[] _go_explarticles)
    {
        i_explosionDamage   = _i_dmg;
        i_expLodeDamage     = _i_lodedmg;
        f_explockBack       = _f_knockback;
        f_detonationTime    = _f_detTime;
        f_expRad            = _f_rad;
        b_impact            = _b_imp;
        go_explarticles     = _go_explarticles;
    }

    /// <summary>
    /// Set the explosion data
    /// </summary>
    /// <param name="_ae_boom">Explosion data</param>
    public void InitExplosion(AugmentExplosion _ae_boom)
    {
        i_explosionDamage   = _ae_boom.i_damage;
        i_expLodeDamage     = _ae_boom.i_lodeDamage;
        f_explockBack       = _ae_boom.f_explockBack;
        f_detonationTime    = _ae_boom.f_detonationTime;
        f_expRad            = _ae_boom.f_radius;
        b_impact            = _ae_boom.b_impact;
        go_explarticles     = _ae_boom.go_explarticles;
    }

    public AugmentProperties GetAugmentProperties()
    {
        return new AugmentProperties(s_name, f_weight, f_recoil, f_speed, f_heatsink, f_knockback, f_energyGauge, i_damage, i_lodeDamage);
    }

    public AugmentPhysicals GetPhysicalProperties()
    {
        return new AugmentPhysicals(f_trWidth, f_trLifetime, A_trKeys, go_weaponProjectile);
    }

    public AugmentExplosion GetExplosionProperties()
    {
        return new AugmentExplosion(i_explosionDamage, i_expLodeDamage, f_explockBack, f_detonationTime, f_expRad, b_impact, go_explarticles);
    }

    public static Augment operator +(Augment a, Augment b)
    {
        // Name stuffs
        string newNamea = a.s_name.Split(' ')[0];
        string newNameb = b.s_name.Split(' ')[0];
        string newName = "";
        if (newNamea == newNameb)
            newName += "Super ";
        else
            newName += newNamea + " " + newNameb + " ";
        newName += a.s_name.Split(' ')[1];
        a.s_name += newName;
        // Audio data
        a.ac_useSound = Utils.CombineArrays(a.ac_useSound, b.ac_useSound);
        a.ac_travelSound = Utils.CombineArrays(a.ac_travelSound, b.ac_travelSound);
        a.ac_hitSound = Utils.CombineArrays(a.ac_hitSound, b.ac_hitSound);
        // Info data
        a.i_damage += b.i_damage;
        a.i_lodeDamage += b.i_lodeDamage;
        a.f_weight += b.f_weight;
        a.f_recoil += b.f_recoil;
        a.f_energyGauge += b.f_energyGauge;
        a.f_knockback += b.f_knockback;
        a.f_heatsink += b.f_heatsink;
        // Physical data
        a.f_trWidth += b.f_trWidth;
        a.f_trLifetime += b.f_trLifetime;
        a.A_trKeys = Utils.CombineArrays(a.A_trKeys, b.A_trKeys);
        a.go_weaponProjectile = b.go_weaponProjectile;
        // Explosion data
        a.i_explosionDamage += b.i_explosionDamage;
        a.i_expLodeDamage += b.i_expLodeDamage;
        a.f_explockBack += b.f_explockBack;
        a.f_detonationTime += b.f_detonationTime;
        a.f_expRad += b.f_expRad;
        a.go_explarticles = Utils.CombineArrays(a.go_explarticles, b.go_explarticles);
        // If any of them are set to impact, set to be impact
        if (a.b_impact || b.b_impact)
            a.b_impact = true;
        else
            a.b_impact = false;
        return a;
    }

    public static Augment operator -(Augment a, Augment b)
    {
        // Name stuffs
        string newNamea = a.s_name.Split(' ')[0];
        string newNameb = b.s_name.Split(' ')[0];
        string newName = "";
        if (newNamea == newNameb)
            newName += "Super ";
        else
            newName += newNamea + " " + newNameb + " ";
        newName += a.s_name.Split(' ')[1];
        a.s_name += newName;
        // Audio data
        a.ac_useSound = Utils.CombineArrays(a.ac_useSound, b.ac_useSound);
        a.ac_travelSound = Utils.CombineArrays(a.ac_travelSound, b.ac_travelSound);
        a.ac_hitSound = Utils.CombineArrays(a.ac_hitSound, b.ac_hitSound);
        // Info data
        a.i_damage += b.i_damage;
        a.i_lodeDamage += b.i_lodeDamage;
        a.f_weight += b.f_weight;
        a.f_recoil += b.f_recoil;
        a.f_energyGauge += b.f_energyGauge;
        a.f_knockback += b.f_knockback;
        a.f_heatsink += b.f_heatsink;
        // Physical data
        a.f_trWidth += b.f_trWidth;
        a.f_trLifetime += b.f_trLifetime;
        a.A_trKeys = Utils.CombineArrays(a.A_trKeys, b.A_trKeys);
        a.go_weaponProjectile = b.go_weaponProjectile;
        // Explosion data
        a.i_explosionDamage += b.i_explosionDamage;
        a.i_expLodeDamage += b.i_expLodeDamage;
        a.f_explockBack += b.f_explockBack;
        a.f_detonationTime += b.f_detonationTime;
        a.f_expRad += b.f_expRad;
        a.go_explarticles = Utils.CombineArrays(a.go_explarticles, b.go_explarticles);
        // If any of them are set to impact, set to be impact
        if (a.b_impact || b.b_impact)
            a.b_impact = true;
        else
            a.b_impact = false;
        return a;
    }
}
