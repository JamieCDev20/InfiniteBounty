using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncy : MonoBehaviour
{
    [SerializeField] private float f_otherBounceForce = 50;
    [SerializeField] private float f_playerBounceForce = 150;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.attachedRigidbody != null)
            if (collision.collider.CompareTag("Player"))
                collision.collider.attachedRigidbody.AddForce(transform.up * f_playerBounceForce, ForceMode.VelocityChange);
            else
                collision.collider.attachedRigidbody.AddForce(transform.up * f_otherBounceForce, ForceMode.VelocityChange);
    }
}