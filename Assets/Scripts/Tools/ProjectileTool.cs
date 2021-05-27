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
    [SerializeField] private float f_recoilTime;
    [SerializeField] private GameObject exploEffects;

    [Header("Heat")]
    [SerializeField] private float f_heatPerShot;
    [SerializeField] private AudioSource as_heatGuageSource;
    private float f_currentHeat;
    [SerializeField] private float f_maxHeatPitch;
    private PlayerInputManager pim;
    internal bool b_isLeftHandWeapon;
    [SerializeField] private float f_heatVolumeMult;
    private bool b_isOverheating;
    [SerializeField] private ParticleSystem ps_overHeatEffects;
    [SerializeField] private AudioClip ac_overHeatClip;
    private PlayerAnimator pa_anim;
    [SerializeField] private ParticleSystem ps_shotEffects;
    private List<Element> elementList = new List<Element>();
    [SerializeField] private MeshRenderer mr_renderer;
    [SerializeField] private string defaultBulletPath;


    public override void SetActiveState(bool val)
    {
        pa_anim = GetComponentInParent<PlayerAnimator>();
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
            if (b_isOverheating)
                return;
            base.Use(_v_forwards);
            StartCoroutine(FireCoroutine(_v_forwards, true));
            b_usable = false;
            StartCoroutine(TimeBetweenUsage());
            //PlayParticles(true);
            ps_shotEffects.Play();

            cc_cam?.Recoil(f_recoil, f_recoilTime);

            pa_anim?.GunRecoil(b_isLeftHandWeapon, f_recoil, f_timeBetweenUsage);

            PlayAudio(ac_activationSound);
        }
    }

    private void Update()
    {
        if (pim == null)
        {
            pim = transform.root.GetComponent<PlayerInputManager>();
            return;
        }
        if (f_currentHeat > 0 && (!(b_isLeftHandWeapon ? pim.GetToolBools().b_LToolHold : pim.GetToolBools().b_RToolHold) || b_isOverheating))
            f_currentHeat -= Time.deltaTime * f_heatsink;

        if (f_currentHeat >= Mathf.Clamp(f_energyGauge, 10, Mathf.Infinity))
        {
            StartOverheating();
        }

        if (f_currentHeat <= 0)
            EndOverHeat();

        DoHeatSound();
    }

    private void StartOverheating()
    {
        if (!b_isOverheating)
        {
            PlayAudio(ac_overHeatClip);
            ps_overHeatEffects?.Play();
            b_isOverheating = true;
        }
    }

    public bool OverHeat()
    {
        return b_isOverheating;
    }

    private void EndOverHeat()
    {
        ps_overHeatEffects?.Stop();
        b_isOverheating = false;
    }

    public void DoHeatSound()
    {
        as_heatGuageSource.volume = ((float)(f_currentHeat / f_energyGauge)) * f_heatVolumeMult;
        as_heatGuageSource.pitch = Mathf.Lerp(0, f_maxHeatPitch, (float)f_currentHeat / f_energyGauge);

        mr_renderer.material.SetFloat("DamageFlash", (float)f_currentHeat / f_energyGauge);
    }

    public override void NetUse(Vector3 _v_forwards)
    {
        StartCoroutine(FireCoroutine(_v_forwards, false));
    }

    IEnumerator FireCoroutine(Vector3 _v_for, bool _b_local)
    {
        yield return new WaitForSeconds(Time.deltaTime * 2);
        SpawnBullet(_v_for);
        //if (_b_local)
        //{
        //    if ((b_isLeftHandWeapon ? pim.GetToolBools().b_LToolHold : pim.GetToolBools().b_RToolHold))
        //}
        //else
        //{
        //    SpawnBullet(_v_for);
        //}
    }

    public void SpawnBullet(Vector3 _v_direction)
    {
        float spread = 0;

        if (pim != null)
            spread = f_spreadAngle * (pim.ReturnVelocity(f_spreadAngle) * 0.05f);
        Vector3 up;
        Vector3 right;
        if (pim != null)
        {
            up = pim.transform.up;
            right = pim.transform.right;
        }
        else
        {
            up = transform.up;
            right = transform.right;
        }
        for (int i = 0; i < i_shotsPerRound; i++)
        {
            f_currentHeat += f_heatPerShot;
            _v_direction = Quaternion.AngleAxis(Random.Range(-spread, spread), up) * _v_direction;
            _v_direction = Quaternion.AngleAxis(Random.Range(-spread, spread), right) * _v_direction;

            Bullet newBullet = PoolManager.x.SpawnObject(go_hitBox[0], t_firePoint.position, t_firePoint.rotation).GetComponent<Bullet>();
            newBullet.Setup(Mathf.RoundToInt(i_damage / i_shotsPerRound) * 2, Mathf.RoundToInt(i_lodeDamage / i_shotsPerRound) * 2, c_playerCollider, ap_projAugment, ref ae_explode, eo_element, c_trail);
            newBullet.SetPhysicsMat(pm_physicsMat);
            newBullet.SetObjectsToSpawn(goA_explarticles);
            newBullet.MoveBullet(_v_direction, f_shotSpeed);
        }
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
        ProjectileAugment pa = (ProjectileAugment)AugmentManager.x.GetProjectileAugmentAt(aug.Stage, aug.Stage == AugmentStage.fused ? AugmentManager.x.GetIndicesByName(aug.Name) : new int[] { AugmentManager.x.GetAugmentIndex(aug.at_type, aug.Name) });
        AugmentProjectile augData = pa.GetProjectileData();
        ap_projAugment.f_gravity += augData.f_gravity;
        //augData.pm_phys;
        ap_projAugment.f_bulletScale += augData.f_bulletScale * (GetAugmentLevelModifier(aug.Level));
        if(elementList != null)
            if(elementList.Count > 0)
            {
                elementList.AddRange(aug.AugElement);
                eo_element = elementList.ToArray();
            }

        if (augData.pm_phys != "")
            pm_physicsMat = Resources.Load<PhysicMaterial>($"{augData.pm_phys}");

        i_shotsPerRound += Mathf.RoundToInt(augData.i_shotsPerRound * (GetAugmentLevelModifier(aug.Level)));
        f_shotSpeed = Mathf.Clamp(f_shotSpeed + aug.GetAugmentProperties().f_speed - (pa.GetProjectileData().f_gravity * 25), 1, 100000000000);

        float _f_speedMult = aug.GetAugmentProperties().f_speed + 1;
        f_timeBetweenUsage /= _f_speedMult > 0 ? _f_speedMult : 0.001f;


        string newBullet = pa.GetPhysicalProperties().go_projectile;
        if (newBullet != null && newBullet != "")
            go_hitBox[0] = Resources.Load<GameObject>(newBullet);

        return true;
    }

    public override void RemoveStatChanges(Augment aug)
    {
        base.RemoveStatChanges(aug);

        ProjectileAugment pa = (ProjectileAugment)AugmentManager.x.GetProjectileAugmentAt(aug.Stage, aug.Stage == AugmentStage.fused ? AugmentManager.x.GetIndicesByName(aug.Name) : new int[] { AugmentManager.x.GetAugmentIndex(aug.at_type, aug.Name) });
        float mod = GetAugmentLevelModifier(aug.Level);
        AugmentProjectile projData = pa.GetProjectileData();
        AugmentProperties augData = aug.GetAugmentProperties();
        ap_projAugment.f_gravity -= projData.f_gravity;

        ap_projAugment.f_bulletScale -= projData.f_bulletScale * mod;
        if(aug.AugElement != null)
        {
            if(aug.AugElement.Length > 0)
            {
                foreach (Element e in aug.AugElement)
                {
                    try
                    {
                        elementList.Remove(e);
                    }
                    catch { }
                }
                if(elementList != null)
                    if(elementList.Count > 0)
                        eo_element = elementList.ToArray();
            }
        }

        i_shotsPerRound -= Mathf.RoundToInt(projData.i_shotsPerRound * mod);
        f_shotSpeed -= augData.f_speed - (projData.f_gravity * 25);
        f_timeBetweenUsage *= Mathf.Clamp(augData.f_speed, 0.1f, 10000);

        string newBullet = pa.GetPhysicalProperties().go_projectile;
        if (newBullet != null && newBullet != "")
            go_hitBox[0] = Resources.Load<GameObject>(defaultBulletPath);

    }

}
