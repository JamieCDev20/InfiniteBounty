using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpinner : MonoBehaviour
{
    [SerializeField] private Vector3 v_spin;
    [SerializeField] private bool b_shouldSinBackToStart;
    [SerializeField] private Vector3 v_unitsPerSecond;

    void Update()
    {
        transform.Rotate(v_spin * Time.deltaTime);
        if (b_shouldSinBackToStart)
            transform.position += (v_unitsPerSecond * Mathf.Sin(Time.realtimeSinceStartup));
    }
}
