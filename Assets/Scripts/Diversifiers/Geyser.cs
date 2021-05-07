using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    [SerializeField] private float f_firePower;
    [SerializeField] private float f_firePowerPlayer;
    private AudioSource as_source;
    [SerializeField] private AudioClip ac_jumpClip;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody _rb = other.GetComponent<Rigidbody>();
        as_source.PlayOneShot(ac_jumpClip);

        if (other.CompareTag("Player"))
            _rb.velocity = transform.up * f_firePowerPlayer;
        else if (_rb && !other.name.Contains("andyman"))
            _rb.velocity = transform.up * f_firePower;
    }
}