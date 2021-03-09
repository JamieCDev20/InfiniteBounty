using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public static EnemySpawner x;
    private int i_numberOfEnemies;
    private DifficultySet ds_currentDifficulty;

    [Header("Enemies in Level at Start")]
    [SerializeField] private GameObject[] goA_startEnemies = new GameObject[1];
    [SerializeField] private int[] iA_numberOfEachStartingEnemyType = new int[1];

    [Header("Waves")]
    [SerializeField] private GameObject[] goA_groundWaveEnemies = new GameObject[1];
    [SerializeField] private GameObject[] goA_flyingWaveEnemies = new GameObject[1];
    [SerializeField] private Vector2 v_timeBetweenWaves = new Vector2(35, 45);
    [Space]
    [SerializeField] private int i_numberOfHordesPerWave = 7;
    [SerializeField] private float f_timeBetweenHordes = 5;
    [SerializeField] private Vector2 v_groundEnemiesPerHorde = new Vector2(5, 7);
    [SerializeField] private Vector2 v_flyingEnemiesPerHorde = new Vector2(5, 7);

    [Header("Miniboss")]
    [SerializeField] private GameObject go_miniboss;
    private List<int> iL_minibossZones = new List<int>();

    [Header("Zone Things")]
    [SerializeField] private ZoneInfo[] ziA_enemySpawnZones = new ZoneInfo[0];
    [SerializeField] private LayerMask lm_zoneCheckMask;


    private void Start()
    {
        x = this;
        ds_currentDifficulty = DifficultyManager.x.ReturnCurrentDifficulty();

        /*for (int i = 0; i < ziA_enemySpawnZones.Length; i++)
            ziA_enemySpawnZones[i].f_zoneRadius = ziA_enemySpawnZones[i].t_zone.GetComponent<SphereCollider>().radius;*/

        iL_minibossZones = new List<int>();
        for (int i = 0; i < ds_currentDifficulty.i_numberOfMiniBosses; i++)
            iL_minibossZones.Add(Random.Range(0, ziA_enemySpawnZones.Length));

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CheckZoneForPlayers());
            PlaceStartingEnemiesInZones();
        }

    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha0))
            SpawnEnemy(go_miniboss, 0, false);
#endif
    }

    private void PlaceStartingEnemiesInZones()
    {
        for (int i = 0; i < iA_numberOfEachStartingEnemyType.Length; i++)
        {
            int _i_zone = Random.Range(0, ziA_enemySpawnZones.Length);

            for (int x = 0; x < iA_numberOfEachStartingEnemyType[i]; x++)
            {
                SpawnEnemy(goA_startEnemies[i], _i_zone, false);
            }
        }
    }

    private IEnumerator CheckZoneForPlayers()
    {
        yield return new WaitForSeconds(Random.Range(v_timeBetweenWaves.x, v_timeBetweenWaves.y) * ds_currentDifficulty.f_spawnFrequencyMult);
        bool _b_spawnedWave = false;

        for (int i = 0; i < ziA_enemySpawnZones.Length; i++)
        {
            if (Physics.OverlapSphere(ziA_enemySpawnZones[i].t_zone.position, ziA_enemySpawnZones[i].f_zoneRadius, lm_zoneCheckMask).Length > 0)
            {
                _b_spawnedWave = true;
                StartCoroutine(SpawnWave(i));
                if (iL_minibossZones.Contains(i))
                {
                    DynamicAudioManager.x.StartBoss();
                    SpawnEnemy(go_miniboss, i, false);
                    iL_minibossZones.Remove(i);
                }
            }
        }
        if (_b_spawnedWave)
            yield return new WaitForSeconds(f_timeBetweenHordes * i_numberOfHordesPerWave);

        StartCoroutine(CheckZoneForPlayers());
    }

    private IEnumerator SpawnWave(int _i_zoneToSpawnEnemiesIn)
    {
        DynamicAudioManager.x.StartCombat();

        for (int i = 0; i < i_numberOfHordesPerWave; i++)
        {
            for (int x = 0; x < Random.Range(v_groundEnemiesPerHorde.x, v_groundEnemiesPerHorde.y) * ds_currentDifficulty.f_spawnAmountMult; x++)
                SpawnEnemy(PickRandomGroundWaveEnemy(), _i_zoneToSpawnEnemiesIn, false);

            for (int x = 0; x < Random.Range(v_flyingEnemiesPerHorde.x, v_flyingEnemiesPerHorde.y) * ds_currentDifficulty.f_spawnAmountMult; x++)
                SpawnEnemy(PickRandomFlyingWaveEnemy(), _i_zoneToSpawnEnemiesIn, true);

            yield return new WaitForSeconds(f_timeBetweenHordes);
        }
    }
    private GameObject PickRandomGroundWaveEnemy()
    {
        return goA_groundWaveEnemies[Random.Range(0, goA_groundWaveEnemies.Length)];
    }
    private GameObject PickRandomFlyingWaveEnemy()
    {
        return goA_flyingWaveEnemies[Random.Range(0, goA_flyingWaveEnemies.Length)];
    }

    private void SpawnEnemy(GameObject _go_enemyToSpawn, int _i_zoneIndexToSpawnIt, bool _b_isFlyingEnemy)
    {
        if (i_numberOfEnemies < ds_currentDifficulty.f_maxNumberOfEnemies)// || _go_enemyToSpawn == go_miniboss)
        {
            PhotonNetwork.Instantiate(_go_enemyToSpawn.name, GetPositionWithinZone(_i_zoneIndexToSpawnIt, _b_isFlyingEnemy), new Quaternion(0, Random.value, 0, Random.value));
            i_numberOfEnemies = TagManager.x.GetTagSet("Enemy").Count;
        }
    }

    private Vector3 GetPositionWithinZone(int _i_zoneIndexToSpawnIt, bool _b_isFlyingEnemy)
    {
        if (_b_isFlyingEnemy)
            return ziA_enemySpawnZones[_i_zoneIndexToSpawnIt].tA_flyingEnemySpawns[Random.Range(0, ziA_enemySpawnZones[_i_zoneIndexToSpawnIt].tA_flyingEnemySpawns.Length)].position;
        else
            return ziA_enemySpawnZones[_i_zoneIndexToSpawnIt].tA_groundEnemySpawns[Random.Range(0, ziA_enemySpawnZones[_i_zoneIndexToSpawnIt].tA_groundEnemySpawns.Length)].position;
    }

    internal void EnemyDied(bool _b_isBoss)
    {
        //if (PhotonNetwork.IsMasterClient)
        i_numberOfEnemies = TagManager.x.GetTagSet("Enemy").Count;

        if (i_numberOfEnemies < 5)
        {
            DynamicAudioManager.x.EndCombat();
            if (_b_isBoss)
                DynamicAudioManager.x.EndBoss();
        }
    }
}

[System.Serializable]
public struct ZoneInfo
{
    public Transform t_zone;
    public float f_zoneRadius;
    public Transform[] tA_groundEnemySpawns;
    public Transform[] tA_flyingEnemySpawns;
    public Transform t_bossSpawn;
}
