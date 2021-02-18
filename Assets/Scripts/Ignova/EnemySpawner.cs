using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public static EnemySpawner x;
    private int i_numberOfEnemies;
    private DifficultySet ds_currentDifficulty;

    [Header("Waves")]
    [SerializeField] private GameObject[] goA_waveEnemies = new GameObject[2];
    [SerializeField] private int[] iA_enemyWeightings = new int[2];
    private List<GameObject> goL_enemyWeightedList = new List<GameObject>();
    [SerializeField] private Vector2 v_timeBetweenWaves = new Vector2(35, 45);
    [Space]
    [SerializeField] private int i_numberOfHordesPerWave = 7;
    [SerializeField] private float f_timeBetweenHordes = 5;
    [SerializeField] private Vector2 v_enemiesPerHorde = new Vector2(5, 7);

    [Header("Miniboss")]
    [SerializeField] private GameObject go_miniboss;
    private List<int> iL_minibossZones = new List<int>();

    [Header("Zone Things")]
    [SerializeField] private ZoneInfo[] ziA_enemySpawnZones = new ZoneInfo[0];
    [SerializeField] private LayerMask lm_zoneCheckMask;

    private void Start()
    {
        ds_currentDifficulty = DifficultyManager.x.ReturnCurrentDifficulty();

        for (int i = 0; i < goA_waveEnemies.Length; i++)
            for (int x = 0; x < iA_enemyWeightings[i]; x++)
                goL_enemyWeightedList.Add(goA_waveEnemies[i]);

        iL_minibossZones = new List<int>();
        for (int i = 0; i < ds_currentDifficulty.i_numberOfMiniBosses; i++)
            iL_minibossZones.Add(Random.Range(0, ziA_enemySpawnZones.Length));

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(CheckZoneForPlayers());

    }

    private IEnumerator CheckZoneForPlayers()
    {
        yield return new WaitForSeconds(Random.Range(v_timeBetweenWaves.x, v_timeBetweenWaves.y) * ds_currentDifficulty.f_spawnFrequencyMult);

        for (int i = 0; i < ziA_enemySpawnZones.Length; i++)
        {
            if (Physics.OverlapSphere(ziA_enemySpawnZones[i].t_zone.position, ziA_enemySpawnZones[i].f_zoneRadius, lm_zoneCheckMask).Length > 0)
            {
                print("Checking the zones for players, and I found one");
                StartCoroutine(SpawnWave(i));
                if (iL_minibossZones.Contains(i))
                {
                    SpawnEnemy(go_miniboss, i);
                    iL_minibossZones.Remove(i);
                }
            }
        }

        StartCoroutine(CheckZoneForPlayers());
    }

    private IEnumerator SpawnWave(int _i_zoneToSpawnEnemiesIn)
    {
        for (int i = 0; i < i_numberOfHordesPerWave; i++)
        {
            for (int x = 0; x < Random.Range(v_enemiesPerHorde.x, v_enemiesPerHorde.y) * ds_currentDifficulty.f_spawnAmountMult; x++)
                if (i_numberOfEnemies < ds_currentDifficulty.f_maxNumberOfEnemies)
                    SpawnEnemy(PickRandomWaveEnemy(), _i_zoneToSpawnEnemiesIn);

            yield return new WaitForSeconds(f_timeBetweenHordes);
        }
    }

    private void SpawnEnemy(GameObject _go_enemyToSpawn, int _i_zoneIndexToSpawnIt)
    {
        PhotonNetwork.Instantiate(_go_enemyToSpawn.name, GetPositionWithinZone(_i_zoneIndexToSpawnIt), new Quaternion(0, Random.value, 0, Random.value));
        i_numberOfEnemies = TagManager.x.GetTagSet("Enemy").Count;
    }
    private GameObject PickRandomWaveEnemy()
    {
        return goL_enemyWeightedList[Random.Range(0, goL_enemyWeightedList.Count)];
    }

    private Vector3 GetPositionWithinZone(int _i_zoneIndexToSpawnIt)
    {
        return ziA_enemySpawnZones[_i_zoneIndexToSpawnIt].t_zone.position + new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
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
