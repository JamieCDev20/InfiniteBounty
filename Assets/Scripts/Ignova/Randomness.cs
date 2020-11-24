using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Randomness : MonoBehaviourPunCallbacks
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

        if (randomSeed)
            seed = Random.Range(0, 1000000);
        photonView.RPC("RecieveSeed", RpcTarget.Others, seed);


        //sew that seed into the fabrik of reality
        Random.InitState(seed);
        Debug.Log("Seed: " + seed);

        //generate how many lodes we want
        i_lodeCount = Mathf.RoundToInt(RandomValue(v2_lodeCountRange.y - v2_lodeCountRange.x) + v2_lodeCountRange.x);

        //spawn the lodes
        SpawnLodes(i_lodeCount);
        photonView.RPC("SpawnLodes", RpcTarget.Others, i_lodeCount);

    }

    #endregion

    #region Private Voids

    private float RandomValue(float range)
    {
        //generate a random value between 0-range
        return Random.value * range;
    }

    [PunRPC]
    public void RecieveSeed(int seed)
    {
        Random.InitState(seed);
        float burnTheFirst = Random.value;
        Debug.Log(seed);
    }

    [PunRPC]
    private void SpawnLodes(int count)
    {

        GameObject parent = new GameObject("Lodes");
        //spwan a random lode at a random spawn point at a random rotation and add it to lists
        //spawn points are removed from the list to prevent duplicate spawning
        LodeSynchroniser.x.InitialiseLodeArrayLength(count);
        for (int i = 0; i < count; i++)
        {
            if (Lt_lodeSpawns.Count <= 0)
                return;
            float rand = RandomValue(lodes.Length - 1);
            int num = Mathf.RoundToInt(RandomValue(Lt_lodeSpawns.Count - 1));
            GameObject ob = PoolManager.x.SpawnObject(lodes[Mathf.RoundToInt(rand)], Lt_lodeSpawns[num].position, Quaternion.AngleAxis(RandomValue(360), Vector3.up));
            ob.transform.parent = parent.transform;
            ob.GetComponent<PhotonView>().ViewID = 6000 + i;
            PhotonNetwork.RegisterPhotonView(ob.GetComponent<PhotonView>());
            LodeSynchroniser.x.AddLode(ob.GetComponent<LodeBase>(), i);
            ob.name += Lt_lodeSpawns[num].position;
            Lt_lodeSpawns.RemoveAt(num);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Public Voids


    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
