using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    [SerializeField] private float f_firePower;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Rigidbody>()?.AddForce(transform.up * f_firePower);
    }
}
