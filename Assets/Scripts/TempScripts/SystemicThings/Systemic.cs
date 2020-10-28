using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Systemic : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] internal bool b_fire;
    [SerializeField] internal bool b_electric;
    [SerializeField] internal bool b_water;
    [SerializeField] internal bool b_goo;
    [SerializeField] internal bool b_gas;

    [Header("Events")]
    [SerializeField] internal UnityEvent ue_fireTrigger;
    [SerializeField] internal UnityEvent ue_electricTrigger;
    [SerializeField] internal UnityEvent ue_waterTrigger;

    private void OnTriggerEnter(Collider other)
    {
        Systemic _s = other.GetComponent<Systemic>();

        if (_s != null)
        {
            if (b_fire) _s.ue_fireTrigger.Invoke();
            if (b_electric) _s.ue_electricTrigger.Invoke();
            if (b_water) _s.ue_waterTrigger.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Systemic _s = collision.transform.GetComponent<Systemic>();

        if (_s != null)
        {
            if (b_fire) _s.ue_fireTrigger.Invoke();
            if (b_electric) _s.ue_electricTrigger.Invoke();
            if (b_water) _s.ue_waterTrigger.Invoke();
        }
    }
}