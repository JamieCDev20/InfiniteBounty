using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MobilityTool
{
    [Header("Jetpack Info")]
    [SerializeField] private float f_force = 5;
    private Animator anim;
    private Rigidbody rb;
    [SerializeField] private AnimationCurve ac_jetPackForce;
    private float f_timeHeld;
    [SerializeField] private float f_yVelocityKickStart;
    [Space, SerializeField] private GameObject go_fuelPool;
    [SerializeField] private float f_maxFuel;
    private float f_currentFuel;
    private bool b_isBeingUsed;
    [SerializeField] private float f_rechargeRate;
    [Space, SerializeField] private ParticleSystem ps_steamEffect;
    private bool b_isSteaming;
    [SerializeField] private Material m_readyMat;
    [SerializeField] private Material m_steamingMat;

    [Header("Jetpack Sound")]
    [SerializeField] private AudioClip ac_steamingSound;
    private AudioSource as_source;

    private Vector3 v_targetV;
    private float t;
    private AudioSource as_steamSource;
    private PlayerMover pm_mover;

    private void Start()
    {
        anim = GetComponentInParent<Animator>();
        rb = transform.root.GetComponent<Rigidbody>();
        f_currentFuel = f_maxFuel;
        as_source = GetComponent<AudioSource>();
        as_source.clip = ac_activationSound[0];
        EndSteaming();
        as_steamSource = gameObject.AddComponent<AudioSource>();

        pm_mover = transform.root.GetComponent<PlayerMover>();

    }

    public override void Use(Vector3 _v)
    {
        t = Time.deltaTime;
        if (f_currentFuel > 0)
        {
            if (!b_isSteaming)
            {
                b_isBeingUsed = true;
                v_targetV.x = rb.velocity.x;
                v_targetV.y = ac_jetPackForce.Evaluate(f_timeHeld) * f_force;
                v_targetV.z = rb.velocity.z;
                rb.velocity = Vector3.Lerp(rb.velocity, v_targetV, .5f);
                PlayParticles(true);
                f_timeHeld += t;

                f_currentFuel -= t;
                UpdateFuelMeter();
            }
        }
        else
        {
            PlayParticles(false);
            BecomeSteaming();
        }
    }


    private void Update()
    {
        if (!b_isBeingUsed)
        {
            if (f_currentFuel < f_maxFuel)
            {
                if (pm_mover.b_grounded)
                {
                    f_currentFuel += Time.deltaTime * f_rechargeRate;
                    UpdateFuelMeter();
                }
            }
            else
            {
                EndSteaming();
            }
        }
        anim?.SetBool("Jetpack", b_isBeingUsed);
    }

    public override void PlayParticles(bool val)
    {
        if(go_particles.Length != 0)
            foreach(GameObject partics in go_particles)
            {
                if(partics != null)
                    partics.SetActive(val);
            }
        if (!val)
        {
            b_isBeingUsed = false;
            f_timeHeld = 0;
            if (as_source.clip == ac_activationSound[0])
                as_source.Stop();
        }
        else
        {
            if (!as_source.isPlaying)
            {
                as_source.clip = ac_activationSound[0];
                as_source.Play();
            }
        }
    }

    private void UpdateFuelMeter()
    {
        go_fuelPool.transform.localPosition = new Vector3(0, (float)(-1 + (f_currentFuel / f_maxFuel)), 0);
        go_fuelPool.transform.localScale = new Vector3(1, (float)(f_currentFuel / f_maxFuel), 1);
    }

    private void BecomeSteaming()
    {
        f_currentFuel += 0.1f;
        b_isSteaming = true;
        ps_steamEffect.Play();
        go_fuelPool.GetComponent<Renderer>().material = m_steamingMat;

        as_source.Stop();
        //as_source.clip = ac_steamingSound;
        as_steamSource.PlayOneShot(ac_steamingSound);
    }

    private void EndSteaming()
    {
        b_isSteaming = false;
        ps_steamEffect.Stop();
        go_fuelPool.GetComponent<Renderer>().material = m_readyMat;
        foreach (AudioClip jetpackSound in ac_activationSound)
            as_source.clip = jetpackSound;
    }

    public override void StopAudio() { }

}
