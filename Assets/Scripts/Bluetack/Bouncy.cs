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
        {
            Vector3 vel = collision.collider.attachedRigidbody.velocity;
            vel.y = 0;
            if (collision.collider.CompareTag("Player"))
                collision.collider.attachedRigidbody.velocity = vel + (Vector3.up * f_playerBounceForce);
            else
                collision.collider.attachedRigidbody.velocity = vel + (Vector3.up * f_otherBounceForce);

        }
    }
}