using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConeAugment : Augment
{
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] float f_angle;
    [Newtonsoft.Json.JsonProperty]
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

    public AugmentCone GetConeData()
    {
        return new AugmentCone(f_angle, f_radius);
    }

    public static ConeAugment Combine(ConeAugment a, ConeAugment b)
    {
        ConeAugment c = new ConeAugment();
        Augment ac = Augment.Combine(a, b);
        if (a.as_stage == AugmentStage.full && a.s_name != b.s_name)
            c.as_stage = AugmentStage.fused;
        List<string[]> audioClips = ac.GetAudioProperties();
        c.Level = a.Level + b.Level;
        c.eo_element = ac.AugElement;
        c.s_name = ac.Name;
        c.Cost = ac.Cost;
        c.mat_augColor = ac.AugmentMaterial;
        c.InitAudio(audioClips[0], audioClips[1], audioClips[2]);
        c.InitPhysical(ac.GetPhysicalProperties());
        c.at_type = AugmentType.cone;
        c.f_angle = a.f_angle + b.f_angle;
        c.f_radius = a.f_radius + b.f_radius;
        return c;
    }

    public static ConeAugment operator +(ConeAugment a, ConeAugment b)
    {
        a = Combine(a, b);
        return a;
    }
}
