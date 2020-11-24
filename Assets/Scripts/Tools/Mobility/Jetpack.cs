using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MobilityTool
{

    [SerializeField] private float f_force = 5;
    [SerializeField] private GameObject go_particles;
    [SerializeField] private Rigidbody rb;

    private void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();
    }

    public override void Use(Vector3 _v)
    {
        Debug.Log("using!");
        rb?.AddForce(Vector3.up * f_force);
        PlayParticles(true);
    }

    public override void PlayParticles(bool val)
    {
        go_particles.SetActive(val);

    }

}
