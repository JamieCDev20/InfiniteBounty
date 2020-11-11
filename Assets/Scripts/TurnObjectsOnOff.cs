using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnObjectsOnOff : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private PathBlockades[] pbA_blockades = new PathBlockades[0];

    private void Start()
    {
        for (int i = 0; i < pbA_blockades.Length; i++)
        {
            pbA_blockades[i].goA_Obstacles[Random.Range(0, pbA_blockades[i].goA_Obstacles.Length)].SetActive(false);
        }
    }
}

[System.Serializable]
public struct PathBlockades
{
    public GameObject[] goA_Obstacles;
}
