using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AugmentExplosion
{
    public int i_damage;
    public int i_lodeDamage;
    public float f_explockBack;
    public float f_detonationTime;
    public float f_radius;
    public bool b_impact;
    public string[] sA_explarticles;

    public AugmentExplosion(int _dmg, int _lode, float _f_kb, float _f_dt, float _rad, bool _b_imp, string[] _particles)
    {
        i_damage = _dmg;
        i_lodeDamage = _lode;
        f_explockBack = _f_kb;
        f_detonationTime = _f_dt;
        f_radius = _rad;
        b_impact = _b_imp;
        sA_explarticles = _particles;
    }
}
