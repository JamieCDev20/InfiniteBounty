using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameManager[] enemy;

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        SpawnEnemies();

    }

    private void SpawnEnemies()
    {

    }

    private void SpawnEnemy(GameObject toSpawn, Vector3 spawnPos)
    {
        GameObject ob = PhotonNetwork.Instantiate($"Resources\\NetworkPrefabs\\{toSpawn.name}", spawnPos, Quaternion.identity);

    }

}
