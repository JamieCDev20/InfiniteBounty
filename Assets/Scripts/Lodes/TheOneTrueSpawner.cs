using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheOneTrueSpawner : MonoBehaviourPun
{

    [SerializeField] private LodeSpawnZone[] lszA_zones;

    private void Start()
    {
        photonView.RPC(nameof(SyncSeed), RpcTarget.All, 1);
    }

    [PunRPC]
    private void SyncSeed(int seed)
    {
        Random.InitState(1);
        StartCoroutine(DoASpawn());

    }

    IEnumerator DoASpawn()
    {
        for (int i = 0; i < lszA_zones.Length; i++)
        {
            lszA_zones[i].SpawnLode();
            yield return new WaitForEndOfFrame();
        }
    }

}
