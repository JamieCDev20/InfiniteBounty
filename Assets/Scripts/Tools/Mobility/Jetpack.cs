using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MobilityTool
{
    [Header("Jetpack Info")]
    [SerializeField] private float f_force = 5;
    private Rigidbody rb;
    [SerializeField] private AnimationCurve ac_jetPackForce;
    [Space, SerializeField] private float f_timeHeld;

    private void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();
    }

    public override void Use(Vector3 _v)
    {
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(rb.velocity.x, f_force * ac_jetPackForce.Evaluate(f_timeHeld), rb.velocity.z), 0.3f);
        PlayParticles(true);
        f_timeHeld += Time.deltaTime;
    }

    public override void PlayParticles(bool val)
    {
        go_particles.SetActive(val);
        if (!val) f_timeHeld = 0;
    }

}
