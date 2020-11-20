using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 v_unitsPerSecond;


    private void Update()
    {
        transform.position += v_unitsPerSecond;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().TakeDamage(10000);
        }
    }

}
