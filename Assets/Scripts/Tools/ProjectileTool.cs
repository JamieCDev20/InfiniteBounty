using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTool : WeaponTool
{
    [SerializeField] private float f_shotSpeed;
    [SerializeField] private int i_shotsPerRound;
    [SerializeField] private PhysicMaterial pm_physicsMat;
    [SerializeField] private Transform t_firePoint;
    private Ray r_flightPath;
    private CameraController cc_cam;
    private const AugmentType augType = AugmentType.projectile;
    AugmentProjectile ap_projAugment;

    public override void SetActive(bool val)
    {
        b_active = val;
        if(transform.root.GetComponent<PlayerInputManager>() != null)
            cc_cam = transform.root.GetComponent<PlayerInputManager>().GetCamera();
        c_playerCollider = transform.root.GetComponent<Collider>();
    }

    public override void Use(Vector3 _v_forwards)
    {
        if (!b_active)
            return;
        if (b_usable)
        {
            base.Use(_v_forwards);
            SpawnBullet(_v_forwards);
            b_usable = false;
            StartCoroutine(TimeBetweenUsage());
            PlayParticles(true);
            cc_cam?.Recoil(f_recoil);
            PlayAudio(ac_activationSound);
        }
    }

    public override void NetUse(Vector3 _v_forwards)
    {
        SpawnBullet(_v_forwards);
    }

    public void SpawnBullet(Vector3 _v_direction)
    {
        Bullet newBullet = PoolManager.x.SpawnObject(go_hitBox[0], t_firePoint.position, t_firePoint.rotation).GetComponent<Bullet>();
        newBullet.Setup(i_damage, i_lodeDamage, c_playerCollider, ap_projAugment, ae_explode);
        newBullet.MoveBullet(_v_direction, f_shotSpeed);
    }

    public override void PlayParticles(bool val)
    {
        if (go_particles.Length != 0)
        {
            foreach(GameObject partics in go_particles)
            {
                if(partics != null)
                {
                    partics.SetActive(false);
                    partics.SetActive(true);
                }
            }
        }
    }

    public override bool AddStatChanges(Augment aug)
    {
        if(!base.AddStatChanges(aug))
            return false;
        ProjectileAugment pa = (ProjectileAugment)FindObjectOfType<AugmentManager>().GetAugment(aug.Name).Aug;
        AugmentProjectile augData = pa.GetProjectileData();
        i_shotsPerRound += Mathf.RoundToInt(augData.i_shotsPerRound * (GetAugmentLevelModifier(aug.Level) * 0.25f));
        ap_projAugment.f_gravity += augData.f_gravity;
        //augData.pm_phys;
        ap_projAugment.f_bulletScale += Mathf.RoundToInt(augData.f_bulletScale * (GetAugmentLevelModifier(aug.Level) * 0.25f));
        return true;
    }

}
