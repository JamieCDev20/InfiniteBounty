using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Randomness : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private bool randomSeed;
    [SerializeField] private int seed = 42;
    [SerializeField] private Vector2 v2_lodeCountRange = new Vector2(180, 220);
    [SerializeField] private List<Transform> Lt_lodeSpawns = new List<Transform>();
    [SerializeField] private GameObject[] lodes;

    #endregion

    #region Private

    private int i_lodeCount;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if(randomSeed)
            seed = Random.Range(0, 1000000);
        Random.InitState(seed);
        Debug.Log("Seed: " + seed);

        i_lodeCount = Mathf.RoundToInt(RandomValue(v2_lodeCountRange.y - v2_lodeCountRange.x) + v2_lodeCountRange.x);

        SpawnLodes(i_lodeCount);

    }

    #endregion

    #region Private Voids

    private float RandomValue(float range)
    {
        return Random.value * range;
    }

    private void SpawnLodes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int num = Mathf.RoundToInt(Lt_lodeSpawns.Count - 1);
            PoolManager.x.SpawnObject(lodes[Mathf.RoundToInt(RandomValue(lodes.Length - 1))], Lt_lodeSpawns[num].position, Quaternion.AngleAxis(RandomValue(360), Vector3.up));
            Lt_lodeSpawns.RemoveAt(num);
        }
    }

    #endregion

    #region Public Voids


    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
