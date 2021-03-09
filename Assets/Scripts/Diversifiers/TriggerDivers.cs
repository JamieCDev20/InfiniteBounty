using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDivers : MonoBehaviour
{

    private LodeSpawnZone[] ziA_diversifiableZone = new LodeSpawnZone[0];

    void Start()
    {
        ziA_diversifiableZone = FindObjectsOfType<LodeSpawnZone>();
        DiversifierManager.x.ApplyDiversifiers(ziA_diversifiableZone);
    }
}
