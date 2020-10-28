using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct EnergyGauge
{
    [SerializeField] public float f_capacity;
    [SerializeField] public float f_coolDownRate;
    [SerializeField] public float f_heatingSpeed;
    [SerializeField] public Canvas can_Gauge;
}
