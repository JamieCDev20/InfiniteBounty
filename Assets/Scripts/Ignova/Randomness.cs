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


    #endregion

    #region Private

    private LodeSpawnZone[] ldzA_zoneSpawns = new LodeSpawnZone[0];
    private int i_numberOfZoneDone;
    private List<GameObject> goL_allLodes = new List<GameObject>();

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        ldzA_zoneSpawns = FindObjectsOfType<LodeSpawnZone>();

        //if the you're not the master client then don't spawn anything
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (randomSeed)
            seed = Random.Range(0, 1000000);
        photonView.RPC("RecieveSeed", RpcTarget.Others, seed);


        //sew that seed into the fabrik of reality
        Random.InitState(seed);
        //Debug.Log("Seed: " + seed);

        //spawn the lodes
        SpawnLodes(seed);
        photonView.RPC("SpawnLodes", RpcTarget.Others, seed);

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
    private void SpawnLodes(int seed)
    {
        print("Spawning Lodes");

        for (int i = 0; i < ldzA_zoneSpawns.Length; i++)
            ldzA_zoneSpawns[i].SpawnLode(this, seed);


    }

    #endregion

    #region Public Voids

    internal void LodeSpawned(GameObject _go_lode)
    {
        i_numberOfZoneDone++;

        if (i_numberOfZoneDone >= ldzA_zoneSpawns.Length)
            for (int i = 0; i < goL_allLodes.Count; i++)
            {
                PhotonView view = goL_allLodes[i].GetComponent<PhotonView>();
                view.ViewID = 600000 + i;
                PhotonNetwork.RegisterPhotonView(view);
            }
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion



}
