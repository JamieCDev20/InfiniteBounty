using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Augment
{
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
    [SerializeField] float f_damage;
    [SerializeField] float f_lodeDamage;

    #endregion

    #region Tool Physical Properties

    [SerializeField] float f_trWidth;
    [SerializeField] float f_trLifetime;
    [SerializeField] Color[] A_trKeys;
    [SerializeField] GameObject go_weaponProjectile;

    #endregion

    #region EXPLOSION

    [SerializeField] float f_explockBack;
    [SerializeField] float f_detonationTime;
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

    public void InitInfo(float _f_weight, float _f_recoil, float _f_speed, float _f_heatsink, float _f_knockback, float _f_energy, float _f_damage, float _f_lode)
    {
        f_weight = _f_weight;
        f_recoil = _f_recoil;
        f_speed = _f_speed;
        f_heatsink = _f_heatsink;
        f_knockback = _f_knockback;
        f_energyGauge = _f_energy;
        f_damage = _f_damage;
        f_lodeDamage = _f_lode;
    }

    public void InitPhysical(float _f_width, float _f_lifetime, Color[] _a_keys, GameObject _go_projectile)
    {
        f_trWidth = _f_width;
        f_trLifetime = _f_lifetime;
        A_trKeys = _a_keys;
        go_weaponProjectile = _go_projectile;
    }

    public void InitExplosion(float _f_knockback, float _f_detTime, GameObject _go_explosion, GameObject _go_explarticles)
    {
        f_explockBack = _f_knockback;
        f_detonationTime = _f_detTime;
        go_explosion = _go_explosion;
        go_explarticles = _go_explarticles;
    }
}
