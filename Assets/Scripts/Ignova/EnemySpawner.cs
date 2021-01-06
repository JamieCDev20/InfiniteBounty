using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public static EnemySpawner x;

    [Header("Enemy Stats")]
    [SerializeField] private GameObject[] goA_enemiesToSpawnAtStart;
    [Space, SerializeField] private GameObject[] goA_enemiesToSpawnDuringAWave;
    [SerializeField] private float[] fA_enemyPerWaveWeighting;

    [Header("Spawn Rate")]
    [SerializeField, Tooltip("The number of enemies spawned at the start of the level, split between the two first zones")] private int i_enemiesAtStart;
    [SerializeField] private Vector2 v_secondsBetweenWave;
    [SerializeField, Tooltip("The number of per player to be spawned during a wave")] private Vector2 v_enemiesPerWave;
    [SerializeField] private bool b_spawnWaveAtStart;
    private int i_numberOfEnemies;
    [SerializeField] private int i_maxNumberOfEnemies;

    [Header("Zone Things")]
    [SerializeField] private ZoneInfo[] ziA_enemySpawnZones = new ZoneInfo[0];
    [SerializeField] private LayerMask lm_zoneCheckMask;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            Destroy(gameObject);
        else x = this;

        for (int i = 0; i < i_enemiesAtStart * 0.5f; i++)
        {
            SpawnEnemiesInZone(0);
            SpawnEnemiesInZone(1);
        }

        Invoke("SpawnEnemyWave", Random.Range(v_secondsBetweenWave.x, v_secondsBetweenWave.y));
    }


    private void SpawnEnemyWave()
    {
        print("Trying to spawn enemy wave");
        for (int i = 0; i < ziA_enemySpawnZones.Length; i++)
        {
            int _i_playerCount = Physics.OverlapSphere(ziA_enemySpawnZones[i].t_zone.position, ziA_enemySpawnZones[i].f_zoneRadius, lm_zoneCheckMask).Length;
            for (int x = 0; x < _i_playerCount; x++)
            {
                print("DETECTED PLAYER IN SECTOR " + i + ". SENDING HOPDOGS.");
                SpawnEnemiesInZone(i);
            }
        }

        Invoke("SpawnEnemyWave", Random.Range(v_secondsBetweenWave.x, v_secondsBetweenWave.y));
    }

    private void SpawnEnemiesInZone(int _i_zoneIndex)
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

        for (int x = 0; x < Random.Range(v_enemiesPerWave.x, v_enemiesPerWave.y); x++)
            for (int i = 0; i < _fL.Count; i++)
            {
                float rando = Random.Range(0, _f_max);
                if (rando >= _fL[i] && rando < _fL[i + 1])
                    SpawnEnemy(goA_enemiesToSpawnDuringAWave[i], ziA_enemySpawnZones[_i_zoneIndex].t_zone.GetChild(Random.Range(0, ziA_enemySpawnZones[_i_zoneIndex].t_zone.childCount)).position + new Vector3(-3 + (Random.value * 6), 0, -3 + (Random.value * 6)));
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

[System.Serializable]
public struct ZoneInfo
{
    public Transform t_zone;
    public float f_zoneRadius;
}
