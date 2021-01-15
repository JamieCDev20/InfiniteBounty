using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AugmentProperties
{
    public string s_name;
    public int i_damage;
    public int i_lodeDamage;
    public float f_weight;
    public float f_recoil;
    public float f_speed;
    public float f_heatsink;
    public float f_knockback;
    public float f_energyGauge;
    public AugmentProperties(string _s_name, float _f_weight, float _f_recoil, float _f_speed, float _f_heatsink, float _f_knockback, float _f_energyGauge, int _i_dmg, int _i_lodeDmg)
    {
        s_name = _s_name;
        f_weight = _f_weight;
        f_recoil = _f_recoil;
        f_speed = _f_speed;
        f_heatsink = _f_heatsink;
        f_knockback = _f_knockback;
        f_energyGauge = _f_energyGauge;
        i_damage = _i_dmg;
        i_lodeDamage = _i_lodeDmg;
    }
}
