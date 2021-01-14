using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Augment
{
    [SerializeField] string s_name;
    public string Name { get { return s_name; } }
    #region Audio
    
    [SerializeField] AudioClip ac_useSound;
    [SerializeField] AudioClip ac_travelSound;
    [SerializeField] AudioClip ac_hitSound;

    #endregion

    #region Tool Information Properties

    [SerializeField] float f_weight;
    [SerializeField] float f_recoil;
    [SerializeField] float f_speed;
    [SerializeField] float f_heatsink;
    [SerializeField] float f_knockback;
    [SerializeField] float f_energyGauge;
    [SerializeField] int i_damage;
    [SerializeField] int i_lodeDamage;

    #endregion

    #region Tool Physical Properties

    [SerializeField] float f_trWidth;
    [SerializeField] float f_trLifetime;
    [SerializeField] Color[] A_trKeys;
    [SerializeField] GameObject go_weaponProjectile;

    #endregion

    #region EXPLOSION

    [SerializeField] int i_explosionDamage;
    [SerializeField] float f_explockBack;
    [SerializeField] float f_detonationTime;
    [SerializeField] Vector3 v_exploSize;
    [SerializeField] GameObject go_explosion;
    [SerializeField] GameObject go_explarticles;

    #endregion

    #region Elemental 

    // Coming soon to a tool near you!

    #endregion

    public void InitAudio(AudioClip _ac_use, AudioClip _ac_travel, AudioClip _ac_hit)
    {
        ac_useSound = _ac_use;
        ac_travelSound = _ac_travel;
        ac_hitSound = _ac_hit;
    }

    public void InitInfo(float _f_weight, float _f_recoil, float _f_speed, float _f_heatsink, float _f_knockback, float _f_energy, int _i_damage, int _i_lode)
    {
        f_weight = _f_weight;
        f_recoil = _f_recoil;
        f_speed = _f_speed;
        f_heatsink = _f_heatsink;
        f_knockback = _f_knockback;
        f_energyGauge = _f_energy;
        i_damage = _i_damage;
        i_lodeDamage = _i_lode;
    }

    public void InitInfo(AugmentProperties _ap_data)
    {
        s_name = _ap_data.s_name;
        f_weight = _ap_data.f_weight;
        f_speed = _ap_data.f_speed;
        f_heatsink = _ap_data.f_heatsink;
        f_knockback = _ap_data.f_knockback;
        f_energyGauge = _ap_data.f_energyGauge;
        i_damage = _ap_data.i_damage;
        i_lodeDamage = _ap_data.i_lodeDamage;
    }
    public void InitPhysical(float _f_width, float _f_lifetime, Color[] _a_keys, GameObject _go_projectile)
    {
        f_trWidth = _f_width;
        f_trLifetime = _f_lifetime;
        A_trKeys = _a_keys;
        go_weaponProjectile = _go_projectile;
    }
    public void InitPhysical(AugmentPhysicals _phys_aug)
    {
        f_trWidth = _phys_aug.f_trWidth;
        f_trLifetime = _phys_aug.f_trLifetime;
        if(_phys_aug.A_trKeys != null)
            A_trKeys = _phys_aug.A_trKeys.ToArray();
        go_weaponProjectile = _phys_aug.go_projectile;
    }

    public void InitExplosion(float _f_knockback, float _f_detTime, GameObject _go_explosion, GameObject _go_explarticles)
    {
        f_explockBack = _f_knockback;
        f_detonationTime = _f_detTime;
        go_explosion = _go_explosion;
        go_explarticles = _go_explarticles;
    }

    public void InitExplosion(AugmentExplosion _ae_boom)
    {
        f_explockBack = _ae_boom.f_explockBack;
        f_detonationTime = _ae_boom.f_detonationTime;
        go_explosion = _ae_boom.go_explosion;
        go_explarticles = _ae_boom.go_explarticles;
    }
    public virtual void ApplyAugment(WeaponTool _t_toolRef)
    {
        _t_toolRef.GetStatChanges();
    }
}
