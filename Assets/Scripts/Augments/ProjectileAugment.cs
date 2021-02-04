using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileAugment : Augment
{
    [SerializeField] int i_shotsPerRound;
    [SerializeField] float f_gravity;
    [SerializeField] PhysicMaterial pm_physMat;
    [SerializeField] float f_bulletScale;

    public void InitProjectile(int _i_shots, float _f_grav, PhysicMaterial _pm_mat, float _f_scale)
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
        List<string[]> audioClips = ac.GetAudioProperties();
        c.s_name = ac.Name;
        c.at_type = AugmentType.projectile;
        c.InitAudio(audioClips[0], audioClips[1], audioClips[2]);
        c.InitPhysical(ac.GetPhysicalProperties());
        c.i_shotsPerRound = a.i_shotsPerRound + b.i_shotsPerRound;
        c.f_gravity = a.f_gravity + b.f_gravity;
        c.f_bulletScale = a.f_bulletScale + b.f_bulletScale;
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
