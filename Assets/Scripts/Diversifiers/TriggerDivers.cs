using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDivers : MonoBehaviour
{
    void Start()
    {
        DiversifierManager.x.ApplyDiversifiers();
    }
}
