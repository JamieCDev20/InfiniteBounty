using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MobilityTool
{

    [SerializeField] private float f_force = 5;
    private Rigidbody rb;

    public override void SetInfo(object[] infos)
    {
        rb = (Rigidbody)infos[0];
    }

    public override void Use()
    {
        rb.AddForce(Vector3.up * f_force);
    }

}
