using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileAugment : Augment
{
    [SerializeField] int i_shotsPerRound;
    [SerializeField] float f_gravity;
    [SerializeField] PhysicMaterial pm_physMat;
    [SerializeField] Vector3 v_bulletScale;

    public void InitProjectile(int _i_shots, float _f_grav, PhysicMaterial _pm_mat, Vector3 _v_scale)
    {
        i_shotsPerRound = _i_shots;
        f_gravity = _f_grav;
        pm_physMat = _pm_mat;
        v_bulletScale = _v_scale;
    }
}
