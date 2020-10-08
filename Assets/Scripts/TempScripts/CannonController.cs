using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour, IUseable
{
    internal bool b_isReady;

    [SerializeField] private Vector3[] vA_landingPositions = new Vector3[1];

    public void OnUse()
    {
        if (b_isReady)
        {
            for (int i = 0; i < LocationController.x.goA_playerObjects.Length; i++)            
                LocationController.x.goA_playerObjects[i].transform.position = vA_landingPositions[i];            

        }
    }
}
