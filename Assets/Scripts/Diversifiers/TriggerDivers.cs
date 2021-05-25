using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDivers : MonoBehaviour
{

    internal LodeSpawnZone[] ziA_diversifiableZone = new LodeSpawnZone[0];

    void Awake()
    {
        ziA_diversifiableZone = FindObjectsOfType<LodeSpawnZone>();
        DiversifierManager.x.SnmdDivers();
    }
}
