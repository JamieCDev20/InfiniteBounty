using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDivers : MonoBehaviour
{
    [Header("Zone Info")]
    [SerializeField] private ZoneInfo[] ziA_diversifiableZone = new ZoneInfo[0];

    void Start()
    {
        DiversifierManager.x.ApplyDiversifiers(ziA_diversifiableZone);
    }
}
