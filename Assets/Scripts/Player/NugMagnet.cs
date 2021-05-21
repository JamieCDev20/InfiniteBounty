using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugMagnet : MonoBehaviour
{

    [SerializeField] private float f_magnetStrength = 20f;

    private void Start()
    {
        if (NetworkedPlayer.x.GetPlayer() != transform.root)
            this.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            other.gameObject.GetComponent<Rigidbody>().velocity = (transform.position - other.transform.position).normalized * f_magnetStrength;
        }
    }

}
