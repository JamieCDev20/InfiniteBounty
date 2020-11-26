using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MobilityTool
{
    [Header("Jetpack Info")]
    [SerializeField] private float f_force = 5;
    private Rigidbody rb;
    [SerializeField] private AnimationCurve ac_jetPackForce;
    private float f_timeHeld;
    [Space, SerializeField] private GameObject go_fuelPool;
    [SerializeField] private float f_maxFuel;
    private float f_currentFuel;
    private bool b_isBeingUsed;
    [SerializeField] private float f_rechargeRate;
    [SerializeField] private GameObject go_steamEffect;
    private bool b_isSteaming;

    private void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();
        f_currentFuel = f_maxFuel;
    }

    public override void Use(Vector3 _v)
    {
        if (f_currentFuel > 0)
        {
            if (!b_isSteaming)
            {
                b_isBeingUsed = true;
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(rb.velocity.x, f_force * ac_jetPackForce.Evaluate(f_timeHeld), rb.velocity.z), 0.3f);
                PlayParticles(true);
                f_timeHeld += Time.deltaTime;

                f_currentFuel -= Time.deltaTime;
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
                print("Regaining fuel");
                f_currentFuel += Time.deltaTime * f_rechargeRate;
                UpdateFuelMeter();
            }
            else
            {
                EndSteaming();
            }
        }
    }

    public override void PlayParticles(bool val)
    {
        go_particles.SetActive(val);
        if (!val)
        {
            b_isBeingUsed = false;
            f_timeHeld = 0;
        }
    }

    private void UpdateFuelMeter()
    {
        go_fuelPool.transform.localPosition = new Vector3(0, (float)(-1 + (f_currentFuel / f_maxFuel)), 0);
        go_fuelPool.transform.localScale = new Vector3(1, (float)(f_currentFuel / f_maxFuel), 1);
    }

    private void BecomeSteaming()
    {
        b_isSteaming = true;
        go_steamEffect.SetActive(true);
    }

    private void EndSteaming()
    {
        b_isSteaming = false;
        go_steamEffect.SetActive(false);
    }

}
