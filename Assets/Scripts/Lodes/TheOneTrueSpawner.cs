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
        foreach (LodeSpawnZone item in GetComponentsInChildren<LodeSpawnZone>())
            item.SpawnLode();
    }
}