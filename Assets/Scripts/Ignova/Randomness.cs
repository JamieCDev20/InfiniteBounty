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
        //if the you're not the master client then don't spawn anything
        if (!PhotonNetwork.IsMasterClient)
            return;

        //generate a random seed
        if(randomSeed)
            seed = Random.Range(0, 1000000);

        //sow that seed into the fabrik of reality
        Random.InitState(seed);
        Debug.Log("Seed: " + seed);

        //generate how many lodes we want
        i_lodeCount = Mathf.RoundToInt(RandomValue(v2_lodeCountRange.y - v2_lodeCountRange.x) + v2_lodeCountRange.x);

        //spawn the lodes
        SpawnLodes(i_lodeCount);

    }

    #endregion

    #region Private Voids

    private float RandomValue(float range)
    {
        //generate a random value between 0-range
        return Random.value * range;
    }

    private void SpawnLodes(int count)
    {
        //spwan a random lode at a random spawn point at a random rotation and add it to lists
        //spawn points are removed from the list to prevent duplicate spawning
        LodeSynchroniser.x.InitialiseLodeArrayLength(count);
        for (int i = 0; i < count; i++)
        {
            int num = Mathf.RoundToInt(RandomValue(Lt_lodeSpawns.Count - 1));
            GameObject ob = PoolManager.x.SpawnObject(lodes[Mathf.RoundToInt(RandomValue(lodes.Length - 1))], Lt_lodeSpawns[num].position, Quaternion.AngleAxis(RandomValue(360), Vector3.up));
            LodeSynchroniser.x.AddLode(ob.GetComponent<LodeBase>(), i);
            ob.name += Lt_lodeSpawns[num].position;
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
