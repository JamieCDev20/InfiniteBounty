using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public static EnemySpawner x;

    [SerializeField] private GameObject[] goA_enemiesToSpawnAtStart;
    [SerializeField] private GameObject[] goA_enemiesToSpawnDuringAWave;
    [SerializeField] private Transform[] tA_spawnPoints;
    //[SerializeField] private float f_timeBetweenSpawns = 3;

    private float spawnCount;
    [Header("Spawn Rate")]
    [SerializeField] private int i_enemiesAtStart;
    [SerializeField] private int i_secondsBetweenWave;
    [SerializeField] private int i_enemiesPerWave;
    [SerializeField] private bool b_spawnWaveAtStart;
    private int i_numberOfEnemies;
    [SerializeField] private int i_maxNumberOfEnemies;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            Destroy(gameObject);
        else x = this;

        for (int i = 0; i < i_enemiesAtStart; i++)
        {
            SpawnEnemy(goA_enemiesToSpawnAtStart[0], tA_spawnPoints[Random.Range(0, tA_spawnPoints.Length)].position + new Vector3(-3 + (Random.value * 6), 0, -3 + (Random.value * 6)));
        }

        if (b_spawnWaveAtStart)
            SpawnEnemyWave();

    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        SpawnEnemyWave();

    }

    private void SpawnEnemyWave()
    {
        spawnCount += Time.deltaTime;

        if (spawnCount >= i_secondsBetweenWave)
        {
            spawnCount = 0;

            if (i_numberOfEnemies < i_maxNumberOfEnemies)
            {
                for (int i = 0; i < i_enemiesPerWave; i++)
                    SpawnEnemy(goA_enemiesToSpawnDuringAWave[0], tA_spawnPoints[Random.Range(0, tA_spawnPoints.Length)].position + new Vector3(-3 + (Random.value * 6), 0, -3 + (Random.value * 6)));
            }
        }

    }

    private void SpawnEnemy(GameObject toSpawn, Vector3 spawnPos)
    {
        //GameObject ob = PoolManager.x.SpawnObject(toSpawn, spawnPos, Quaternion.identity);
        GameObject ob = PhotonNetwork.Instantiate(toSpawn.name, spawnPos, Quaternion.identity);
        i_numberOfEnemies++;
    }

    internal void EnemyDied()
    {
        i_numberOfEnemies--;
    }
}
