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

    [Header("Hordes & Waves")]
    [SerializeField] private float f_timeBetweenHordes;
    [SerializeField] private int i_hordesPerWave;
    [SerializeField] private int i_maxNumberOfEnemies;
    private int i_numberOfEnemies;
    [SerializeField] private Vector2 v_secondsBetweenWave;

    [SerializeField, Tooltip("The number of per player to be spawned during a wave")]
    private Vector2 v_enemiesPerHorde;

    [Space, SerializeField] private bool b_spawnWaveAtStart;
    [SerializeField, Tooltip("The number of enemies spawned at the start of the level, split between the two first zones")]
    private int i_enemiesAtStart;

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
            SpawnEnemiesInZone(0, 1);
            SpawnEnemiesInZone(1, 1);
        }

        Invoke("CheckZonesForPlayers", Random.Range(v_secondsBetweenWave.x, v_secondsBetweenWave.y));
    }

    private void Update()
    {
        Debug.LogError(i_numberOfEnemies + " is how many enemies there are.");
    }

    private void CheckZonesForPlayers()
    {
        int _i_numberOfZonesActivated = 0;
        //Checks to see if there are players in any zones
        for (int i = 0; i < ziA_enemySpawnZones.Length; i++)
            if (Physics.OverlapSphere(ziA_enemySpawnZones[i].t_zone.position, ziA_enemySpawnZones[i].f_zoneRadius, lm_zoneCheckMask).Length > 0)
            {
                _i_numberOfZonesActivated++;
                StartCoroutine(SpawnEnemyWave(i));
            }

        if (_i_numberOfZonesActivated == 0)
            Invoke("CheckZonesForPlayers", Random.Range(v_secondsBetweenWave.x, v_secondsBetweenWave.y));
    }

    private IEnumerator SpawnEnemyWave(int _i_zoneIndex)
    {

        //print("SPAWNING ENEMY WAVE");
        //Creating a working list of weightings for the enemy types.
        float _f_max = 0;
        List<float> _fL = new List<float>();

        for (int i = 0; i < fA_enemyPerWaveWeighting.Length; i++)
            _f_max += fA_enemyPerWaveWeighting[i];

        for (int i = 0; i < fA_enemyPerWaveWeighting.Length; i++)
        {
            _fL.Add(_f_max);
            _f_max += fA_enemyPerWaveWeighting[i];
        }
        _fL.Add(_f_max * 2);

        //Actually Spawning Enemies
        for (int y = 0; y < i_hordesPerWave; y++)
        {
            //Determining how many enemies will spawn in a horde
            for (int x = 0; x < Random.Range(v_enemiesPerHorde.x, v_enemiesPerHorde.y); x++)
            {
                //Only spawn enemies up to the current limit
                if (i_numberOfEnemies < i_maxNumberOfEnemies)
                {
                    //print("SPAWNING ENEMY HORDE");

                    //Which enemy type to spawn
                    float rando = Random.Range(0, _f_max);

                    for (int i = 0; i < _fL.Count; i++)
                        if (rando >= _fL[i] && rando < _fL[i + 1]) //Is this the right enemy type.{
                        {
                            //print("Spawning enemies");
                            SpawnEnemy(goA_enemiesToSpawnDuringAWave[i], ziA_enemySpawnZones[_i_zoneIndex].t_zone.GetChild(Random.Range(0, ziA_enemySpawnZones[_i_zoneIndex].t_zone.childCount)).position + new Vector3(-5 + (Random.value * 10), 0, -5 + (Random.value * 10)));
                        }
                }

            }
            yield return new WaitForSeconds(f_timeBetweenHordes);
        }


        Invoke("CheckZonesForPlayers", Random.Range(v_secondsBetweenWave.x, v_secondsBetweenWave.y));
    }

    private void SpawnEnemiesInZone(int _i_zoneIndex, int _i_amountOfEnemiesToSpawn)
    {
        if (i_numberOfEnemies > i_maxNumberOfEnemies)
            return;

        float _f_max = 0;
        List<float> _fL = new List<float>();

        for (int i = 0; i < fA_enemyPerWaveWeighting.Length; i++)
            _f_max += fA_enemyPerWaveWeighting[i];


        for (int i = 0; i < fA_enemyPerWaveWeighting.Length; i++)
        {
            _fL.Add(_f_max);
            _f_max += fA_enemyPerWaveWeighting[i];
        }
        _fL.Add(_f_max * 2);


        for (int x = 0; x < _i_amountOfEnemiesToSpawn; x++)
            for (int i = 0; i < _fL.Count; i++)
            {
                float rando = Random.Range(0, _f_max);
                if (rando >= _fL[i] && rando < _fL[i + 1])
                    SpawnEnemy(goA_enemiesToSpawnAtStart[i], ziA_enemySpawnZones[_i_zoneIndex].t_zone.GetChild(Random.Range(0, ziA_enemySpawnZones[_i_zoneIndex].t_zone.childCount)).position + new Vector3(-3 + (Random.value * 6), 0, -3 + (Random.value * 6)));
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
