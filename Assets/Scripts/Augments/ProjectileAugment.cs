using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileAugment : Augment
{
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected int i_shotsPerRound;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_gravity;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] string pm_physMat;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_bulletScale;

    public void InitProjectile(int _i_shots, float _f_grav, string _pm_mat, float _f_scale)
    {
        i_shotsPerRound = _i_shots;
        f_gravity = _f_grav;
        pm_physMat = _pm_mat;
        f_bulletScale = _f_scale;
    }
    public void InitProjectile(AugmentProjectile _nyooom)
    {
        i_shotsPerRound = _nyooom.i_shotsPerRound;
        f_gravity = _nyooom.f_gravity;
        pm_physMat = _nyooom.pm_phys;
        f_bulletScale = _nyooom.f_bulletScale;
        
    }

    public AugmentProjectile GetProjectileData()
    {
        return new AugmentProjectile(i_shotsPerRound, f_gravity, pm_physMat, f_bulletScale);
    }

    public static ProjectileAugment Combine(ProjectileAugment a, ProjectileAugment b)
    {
        ProjectileAugment c = new ProjectileAugment();
        Augment ac = Augment.Combine(a, b);
        if (a.as_stage == AugmentStage.full && a.s_name != b.s_name)
            c.as_stage = AugmentStage.fused;
        c.Level = 1;
        c.Cost = ac.Cost;
        c.eo_element = ac.AugElement;
        List<string[]> audioClips = ac.GetAudioProperties();
        c.s_name = ac.Name;
        c.i_cost = ac.Cost;
        c.mat_augColor = ac.AugmentMaterial;
        c.at_type = AugmentType.projectile;
        c.InitAudio(audioClips[0], audioClips[1], audioClips[2]);
        c.InitPhysical(ac.GetPhysicalProperties());
        c.f_heatsink = ac.GetAugmentProperties().f_heatsink;
        c.f_energyGauge = ac.GetAugmentProperties().f_energyGauge;
        c.i_shotsPerRound = a.i_shotsPerRound + b.i_shotsPerRound;
        c.f_gravity = a.f_gravity + b.f_gravity;
        c.f_bulletScale = a.f_bulletScale + b.f_bulletScale;
        c.i_damage = ac.GetAugmentProperties().i_damage;
        c.i_lodeDamage = ac.GetAugmentProperties().i_lodeDamage;

        AugmentExplosion e = ac.GetExplosionProperties();
        c.f_detonationTime = e.f_detonationTime;
        c.f_explockBack = e.f_explockBack;
        c.f_expRad = e.f_radius;
        c.go_explarticles = e.sA_explarticles;

        // Ask John and Nick what it do
        c.pm_physMat = a.pm_physMat;
        return c;
    }

    public static ProjectileAugment operator +(ProjectileAugment a, ProjectileAugment b)
    {
        a = ProjectileAugment.Combine(a, b);
        return a;
    }

}
