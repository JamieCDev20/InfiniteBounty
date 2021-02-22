using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugMagnet : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            other.gameObject.GetComponent<Rigidbody>().velocity = (transform.position - other.transform.position).normalized * 10;
        }
    }

}
