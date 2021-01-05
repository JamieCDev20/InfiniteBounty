using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public static EnemySpawner x;

    [SerializeField] private Transform[] tA_spawnPoints;
    [SerializeField] private GameObject[] goA_enemiesToSpawnAtStart;
    [Space, SerializeField] private GameObject[] goA_enemiesToSpawnDuringAWave;
    [SerializeField] private float[] fA_enemyPerWaveWeighting;
    //[SerializeField] private float f_timeBetweenSpawns = 3;

    private float spawnCount;
    [Header("Spawn Rate")]
    [SerializeField] private int i_enemiesAtStart;
    [SerializeField] private Vector2 v_secondsBetweenWave;
    [SerializeField] private Vector2 v_enemiesPerWave;
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


        if (!PhotonNetwork.IsMasterClient)
            Invoke("SpawnEnemyWave", Random.Range(v_secondsBetweenWave.x, v_secondsBetweenWave.y));
    }


    private void SpawnEnemyWave()
    {
        float _f_max = 0;
        List<float> _fL = new List<float>();

        for (int i = 0; i < fA_enemyPerWaveWeighting.Length; i++)
            _f_max += fA_enemyPerWaveWeighting[i];


        if (i_numberOfEnemies < i_maxNumberOfEnemies)
        {
            for (int i = 0; i < fA_enemyPerWaveWeighting.Length; i++)
            {
                _fL.Add(_f_max);
                _f_max += fA_enemyPerWaveWeighting[i];
            }
            _fL.Add(_f_max * 2);
        }

        for (int i = 0; i < _fL.Count; i++)
        {
            float rando = Random.Range(0, _f_max);
            if (rando >= _fL[i] && rando < _fL[i + 1])
                SpawnEnemy(goA_enemiesToSpawnDuringAWave[i], tA_spawnPoints[Random.Range(0, tA_spawnPoints.Length)].position + new Vector3(-3 + (Random.value * 6), 0, -3 + (Random.value * 6)));
        }

        Invoke("SpawnEnemyWave", Random.Range(v_secondsBetweenWave.x, v_secondsBetweenWave.y));
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
