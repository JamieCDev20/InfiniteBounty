using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSuckable : MonoBehaviour, ISuckable
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }
}
