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
            for (int x = 0; x < Random.Range(1, pbA_blockades[i].i_maxNumberToTurnOff + 1); x++)
            {
                int _i_rando = Random.Range(0, pbA_blockades[i].goL_Obstacles.Count);
                pbA_blockades[i].goL_Obstacles[_i_rando].SetActive(false);
                pbA_blockades[i].goL_Obstacles.RemoveAt(_i_rando);
            }
        }
    }
}

[System.Serializable]
public struct PathBlockades
{
    public int i_maxNumberToTurnOff;
    public List<GameObject> goL_Obstacles;
}
