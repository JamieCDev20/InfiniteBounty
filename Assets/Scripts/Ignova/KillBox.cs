using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 v_unitsPerSecond;
    [SerializeField] private bool b_shouldSinBackToStart;

    private void Update()
    {
        if (b_shouldSinBackToStart)
            transform.position += (v_unitsPerSecond * Mathf.Sin(Time.realtimeSinceStartup));
        else
            transform.position += v_unitsPerSecond * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        //other.GetComponent<PlayerHealth>().TakeDamage(10000);
        other.GetComponent<IHitable>()?.TakeDamage(10000);

    }

}
