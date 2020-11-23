using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject[] goA_enemy;
    [SerializeField] private Transform[] tA_spawnPoints;
    [SerializeField] private float f_timeBetweenSpawns = 3;

    private float spawnCount;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        SpawnEnemies();

    }

    private void SpawnEnemies()
    {
        spawnCount += Time.deltaTime;

        if (spawnCount >= (f_timeBetweenSpawns + Random.Range(-(f_timeBetweenSpawns * 0.1f), (f_timeBetweenSpawns * 0.1f))))
        {
            spawnCount = 0;
            SpawnEnemy(goA_enemy[Random.Range(0, goA_enemy.Length)], tA_spawnPoints[Random.Range(0, tA_spawnPoints.Length)].position + new Vector3(-3 + (Random.value * 6), 0, -3 + (Random.value * 6)));
        }

    }

    private void SpawnEnemy(GameObject toSpawn, Vector3 spawnPos)
    {
        GameObject ob = PhotonNetwork.Instantiate($"NetworkPrefabs\\{toSpawn.name}", spawnPos, Quaternion.identity);

    }

}
