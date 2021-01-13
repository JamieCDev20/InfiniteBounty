using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConeAugment : Augment
{
    [SerializeField] float f_angle;
    [SerializeField] float f_radius;

    public void InitCone(float _f_ang, float _f_rad)
    {
        f_angle = _f_ang;
        f_radius = _f_rad;
    }
    public void InitCone(AugmentCone _aCone)
    {
        f_angle = _aCone.f_angle;
        f_radius = _aCone.f_radius;
    }
}
