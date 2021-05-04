using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheOneTrueSpawner : MonoBehaviourPun
{

    [SerializeField] private LodeSpawnZone[] lszA_zones;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(SyncSeed), RpcTarget.All, Random.Range(0, 999999));
    }

    [PunRPC]
    private void SyncSeed(int seed)
    {
        //Debug.Log("seeding");
        Random.InitState(seed);
        DoASpawn();

    }

    private void DoASpawn()
    {
        for (int i = 0; i < lszA_zones.Length; i++)
        {
            lszA_zones[i].SpawnLode();
            //Debug.LogError("done with: " + lszA_zones[i].gameObject.name + " #" + i);
        }
        //yield return new WaitForEndOfFrame();
        //Debug.Log("done");
    }

}
