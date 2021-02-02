using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    [SerializeField] private float f_firePower;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody _rb = other.GetComponent<Rigidbody>();
        if (_rb)
            _rb.AddForce(transform.up * f_firePower);
    }
}
