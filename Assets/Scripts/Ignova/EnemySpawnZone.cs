﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnZone : MonoBehaviour
{

    [SerializeField] private int i_numberOfHordesPerWave;
    [SerializeField] private int f_timeBetweenHordes;
    [Space]
    [SerializeField] private float f_zoneRadius;
    private List<Transform> tL_groundEnemySpawns = new List<Transform>();
    private List<Transform> tL_flyingEnemySpawns = new List<Transform>();
    private Transform t_bossSpawn;
    [SerializeField] private LayerMask lm_playerLayer;
    [Space]
    [Space]
    [SerializeField] private GameObject[] goA_groundEnemies = new GameObject[0];
    [SerializeField] private Vector2Int v_groundEnemiesPerHorde;
    [Space]
    [SerializeField] private GameObject[] goA_flyingEnemies = new GameObject[0];
    [SerializeField] private Vector2Int v_flyingEnemiesPerHorde;
    private DifficultySet ds_currentDifficulty;

    private void Start()
    {
        ds_currentDifficulty = DifficultyManager.x.ReturnCurrentDifficulty();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("Ground"))
                tL_groundEnemySpawns.Add(transform.GetChild(i));
            else if (transform.GetChild(i).name.Contains("Flying"))
                tL_flyingEnemySpawns.Add(transform.GetChild(i));
            else if (transform.GetChild(i).name.Contains("Boss"))
                t_bossSpawn = (transform.GetChild(i));
        }

    }

    internal bool CheckForPlayersAndSpawnWave()
    {
        if (Physics.OverlapSphere(transform.position, f_zoneRadius, lm_playerLayer).Length > 0)
        {
            StartCoroutine(SpawnWave());
            return true;
        }
        return false;
    }

    private IEnumerator SpawnWave()
    {
        DynamicAudioManager.x.StartCombat();

        for (int i = 0; i < i_numberOfHordesPerWave; i++)
        {
            for (int x = 0; x < Random.Range(v_groundEnemiesPerHorde.x, v_groundEnemiesPerHorde.y) * ds_currentDifficulty.f_spawnAmountMult; x++)
                EnemySpawner.x.SpawnEnemy(goA_groundEnemies[Random.Range(0, goA_groundEnemies.Length)], tL_groundEnemySpawns[Random.Range(0, tL_groundEnemySpawns.Count)].position + RandomVector3(), false);

            for (int x = 0; x < Random.Range(v_flyingEnemiesPerHorde.x, v_flyingEnemiesPerHorde.y) * ds_currentDifficulty.f_spawnAmountMult; x++)
                EnemySpawner.x.SpawnEnemy(goA_flyingEnemies[Random.Range(0, goA_flyingEnemies.Length)], tL_flyingEnemySpawns[Random.Range(0, tL_flyingEnemySpawns.Count)].position + RandomVector3(), true);

            yield return new WaitForSeconds(f_timeBetweenHordes);
        }
    }

    private Vector3 RandomVector3()
    {
        return new Vector3(Random.Range(-15, 15), 0, Random.Range(-15, 15));
    }
}
