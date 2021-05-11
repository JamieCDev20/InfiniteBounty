using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private void Start()
    {
        foreach (PlayerHealth h in FindObjectsOfType<PlayerHealth>())
        {
            h.FullRespawn();
        }
    }
}
