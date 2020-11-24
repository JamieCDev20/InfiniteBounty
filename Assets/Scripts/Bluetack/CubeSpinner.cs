using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpinner : MonoBehaviour
{
    [SerializeField] private Vector3 v_spin;

    void Update()
    {
        transform.Rotate(v_spin * Time.deltaTime);
    }
}
