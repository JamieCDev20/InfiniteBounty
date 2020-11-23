using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeam : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.attachedRigidbody.velocity = new Vector3(0, other.attachedRigidbody.velocity.sqrMagnitude, 0);
    }
}
