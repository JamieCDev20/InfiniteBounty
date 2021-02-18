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
    private Vector2 v_actualSecondsBetweenWave;

    [SerializeField, Tooltip("The number of per player to be spawned during a wave")]
    private Vector2 v_enemiesPerHorde;
    private Vector2 v_actualEnemiesPerHorde;

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
        DifficultySet _ds = DifficultyManager.x.ReturnCurrentDifficulty();

        for (int i = 0; i < i_enemiesAtStart * 0.5f * _ds.f_spawnAmountMult; i++)
        {
            SpawnEnemiesInZone(0, 1);
            SpawnEnemiesInZone(1, 1);
        }

        v_actualEnemiesPerHorde = v_enemiesPerHorde * _ds.f_spawnAmountMult;
        v_actualSecondsBetweenWave = v_secondsBetweenWave * _ds.f_spawnFrequencyMult;

        Invoke("CheckZonesForPlayers", Random.Range(v_actualSecondsBetweenWave.x, v_actualSecondsBetweenWave.y));
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
            Invoke("CheckZonesForPlayers", Random.Range(v_actualSecondsBetweenWave.x, v_actualSecondsBetweenWave.y));
    }

    private IEnumerator SpawnEnemyWave(int _i_zoneIndex)
    {
        //Actually Spawning Enemies
        for (int y = 0; y < i_hordesPerWave; y++)
        {
            //Determining how many enemies will spawn in a horde
            for (int x = 0; x < Random.Range(v_actualEnemiesPerHorde.x, v_actualEnemiesPerHorde.y); x++)
                //Only spawn enemies up to the current limit
                if (i_numberOfEnemies < i_maxNumberOfEnemies)
                    SpawnEnemy(goA_enemiesToSpawnDuringAWave[Random.Range(0, goA_enemiesToSpawnDuringAWave.Length)],
                        ziA_enemySpawnZones[_i_zoneIndex].t_zone.GetChild(Random.Range(0, ziA_enemySpawnZones[_i_zoneIndex].t_zone.childCount)).position + new Vector3(-5 + (Random.value * 10), 0, -5 + (Random.value * 10)));

            yield return new WaitForSeconds(f_timeBetweenHordes);
        }


        Invoke("CheckZonesForPlayers", Random.Range(v_actualSecondsBetweenWave.x, v_actualSecondsBetweenWave.y));
    }

    private void SpawnEnemiesInZone(int _i_zoneIndex, int _i_amountOfEnemiesToSpawn)
    {
        for (int x = 0; x < _i_amountOfEnemiesToSpawn; x++)
            //Only spawn enemies up to the current limit
            if (i_numberOfEnemies < i_maxNumberOfEnemies)
                SpawnEnemy(goA_enemiesToSpawnAtStart[Random.Range(0, goA_enemiesToSpawnAtStart.Length)],
                    ziA_enemySpawnZones[_i_zoneIndex].t_zone.GetChild(Random.Range(0, ziA_enemySpawnZones[_i_zoneIndex].t_zone.childCount)).position + new Vector3(-5 + (Random.value * 10), 0, -5 + (Random.value * 10)));
    }

    private void SpawnEnemy(GameObject toSpawn, Vector3 spawnPos)
    {
        //GameObject ob = PoolManager.x.SpawnObject(toSpawn, spawnPos, Quaternion.identity);
        GameObject ob = PhotonNetwork.Instantiate(toSpawn.name, spawnPos, Quaternion.identity);
        ob.transform.Rotate(0, Random.Range(0, 359), 0);
        i_numberOfEnemies = TagManager.x.GetTagSet("Enemy").Count;
    }

    internal void EnemyDied()
    {
        if (PhotonNetwork.IsMasterClient)
            i_numberOfEnemies = TagManager.x.GetTagSet("Enemy").Count;
    }
}

[System.Serializable]
public struct ZoneInfo
{
    public Transform t_zone;
    public float f_zoneRadius;
}
