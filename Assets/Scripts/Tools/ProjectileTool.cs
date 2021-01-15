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

    AugmentProjectile ap_projAugment;

    public override void SetActive(bool val)
    {
        b_active = val;
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
        Bullet newBullet = PoolManager.x.SpawnObject(go_hitBox, t_firePoint.position, t_firePoint.rotation).GetComponent<Bullet>();
        newBullet.Setup(i_damage, i_lodeDamage, c_playerCollider, ap_projAugment, ae_explode);
        newBullet.MoveBullet(_v_direction, f_shotSpeed);
    }

    public override void PlayParticles(bool val)
    {
        if (go_particles)
        {
            go_particles.SetActive(false);
            go_particles.SetActive(true);
        }
    }


}
