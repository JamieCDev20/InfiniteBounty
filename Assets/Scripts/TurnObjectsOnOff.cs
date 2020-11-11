using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnObjectsOnOff : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private PathBlockades[] pbA_blockades = new PathBlockades[0];

    [Header("Lode Placements")]
    [SerializeField] private GameObject[] goA_lodePrefabs = new GameObject[6];
    [SerializeField] private List<Transform> tL_lodePositions = new List<Transform>();
    [SerializeField] private int i_lodesToSpawn;

    private void Start()
    {
        for (int i = 0; i < pbA_blockades.Length; i++)
        {
            for (int x = 0; x < Random.Range(1, pbA_blockades[i].i_maxNumberToTurnOff); x++)
            {
                int _i_rando = Random.Range(0, pbA_blockades[i].goL_Obstacles.Count);                
                pbA_blockades[i].goL_Obstacles[_i_rando].SetActive(false);
                pbA_blockades[i].goL_Obstacles.RemoveAt(_i_rando);
            }
        }

        for (int i = 0; i < i_lodesToSpawn; i++)
        {
            int _i_rando = Random.Range(0, tL_lodePositions.Count);
            Instantiate(goA_lodePrefabs[Random.Range(0, goA_lodePrefabs.Length)], tL_lodePositions[_i_rando].transform.position, new Quaternion(Random.value, Random.value, Random.value, Random.value));
            tL_lodePositions.RemoveAt(_i_rando);
        }

    }
}

[System.Serializable]
public struct PathBlockades
{
    public int i_maxNumberToTurnOff;
    public List<GameObject> goL_Obstacles;
}
