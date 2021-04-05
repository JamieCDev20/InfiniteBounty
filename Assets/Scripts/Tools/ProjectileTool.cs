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
    public override AugmentType AugType { get { return augType; } }
    AugmentProjectile ap_projAugment;

    [Header("Heat")]
    [SerializeField] private float f_maxHeat;
    [SerializeField] private float f_heatPerShot;
    [SerializeField] private AudioSource as_heatGuageSource;
    private float f_currentHeat;
    [SerializeField] private float f_maxHeatPitch;
    private PlayerInputManager pim;
    internal bool b_isLeftHandWeapon;
    [SerializeField] private float f_heatVolumeMult;

    public override void SetActive(bool val)
    {
        b_active = val;
        if (transform.root.GetComponent<PlayerInputManager>() != null)
        {
            pim = transform.root.GetComponent<PlayerInputManager>();
            cc_cam = pim.GetCamera();
        }
        c_playerCollider = transform.root.GetComponent<Collider>();
    }

    public override void Use(Vector3 _v_forwards)
    {
        if (!b_active)
            return;

        if (b_usable)
        {
            if (f_currentHeat >= f_maxHeat)
                return;
            base.Use(_v_forwards);
            SpawnBullet(_v_forwards);
            b_usable = false;
            StartCoroutine(TimeBetweenUsage());
            PlayParticles(true);
            cc_cam?.Recoil(f_recoil);
            PlayAudio(ac_activationSound);
            f_currentHeat += f_heatPerShot;
        }
    }

    private void Update()
    {
        if (f_currentHeat > 0 && !(b_isLeftHandWeapon ? pim.GetToolBools().b_LToolHold : pim.GetToolBools().b_RToolHold))
            f_currentHeat -= Time.deltaTime * f_heatsink;
        DoHeatSound();
    }

    public void DoHeatSound()
    {
        as_heatGuageSource.volume = ((float)(f_currentHeat / f_maxHeat)) * f_heatVolumeMult;
        as_heatGuageSource.pitch = Mathf.Lerp(0, f_maxHeatPitch, (float)f_currentHeat / f_maxHeat);
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
            foreach (GameObject partics in go_particles)
            {
                if (partics != null)
                {
                    partics.SetActive(false);
                    partics.SetActive(true);
                }
            }
        }
    }

    public override bool AddStatChanges(Augment aug)
    {
        if (!base.AddStatChanges(aug))
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
