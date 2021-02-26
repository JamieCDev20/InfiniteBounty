using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AugmentProjectile
{
    public int i_shotsPerRound;
    public float f_gravity;
    public string pm_phys;
    public float f_bulletScale;

    public AugmentProjectile(int _i_shots, float _f_grav, string _pm, float _f_scale)
    {
        i_shotsPerRound = _i_shots;
        f_gravity = _f_grav;
        pm_phys = _pm;
        f_bulletScale = _f_scale;
    }
}
