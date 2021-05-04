using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public static EnemySpawner x;
    private int i_numberOfEnemies;
    private DifficultySet ds_currentDifficulty;
    [SerializeField] private EnemySpawnZone[] eszA_allEnemyZones;
    [SerializeField] private LayerMask lm_notEnemyLayer;

    [Header("Enemies in Level at Start")]
    [SerializeField] private GameObject[] goA_startEnemies = new GameObject[1];
    [SerializeField] private int[] iA_numberOfEachStartingEnemyType = new int[1];

    [Header("Waves")]
    [SerializeField] private float f_startDelay = 50f;
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

    private IEnumerator Start()
    {
        x = this;
        yield return new WaitForEndOfFrame();
        ds_currentDifficulty = DifficultyManager.x.ReturnCurrentDifficulty();

        iL_minibossZones = new List<int>();
        for (int i = 0; i < ds_currentDifficulty.i_numberOfMiniBosses; i++)
            iL_minibossZones.Add(Random.Range(0, eszA_allEnemyZones.Length));
        //Adds the middle arena to the list
        iL_minibossZones.Add(7);

        if (PhotonNetwork.IsMasterClient)
        {
            PlaceStartingEnemiesInZones();
            yield return new WaitForSeconds(f_startDelay);
            StartCoroutine(CheckZoneForPlayers());
        }
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
            ForceWaveToSpawn();

    }
#endif


    private void PlaceStartingEnemiesInZones()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int x = 0; x < iA_numberOfEachStartingEnemyType[0] * 0.2f; x++)
                SpawnEnemy(goA_startEnemies[0], eszA_allEnemyZones[i].ReturnSpawnPoint(), false);
        }
    }

    private void ForceWaveToSpawn()
    {
        for (int i = 0; i < eszA_allEnemyZones.Length; i++)
            if (eszA_allEnemyZones[i].CheckForPlayersAndSpawnWave())
            {
                if (iL_minibossZones.Contains(i))
                {
                    SpawnEnemy(go_miniboss, eszA_allEnemyZones[i].ReturnSpawnPoint(), false);
                    iL_minibossZones.RemoveAt(i);
                }
            }

    }

    private IEnumerator CheckZoneForPlayers()
    {
        yield return new WaitForSeconds(Random.Range(v_timeBetweenWaves.x, v_timeBetweenWaves.y) * ds_currentDifficulty.f_spawnFrequencyMult);
        bool _b_spawnedWave = false;

        for (int i = 0; i < eszA_allEnemyZones.Length; i++)
            if (eszA_allEnemyZones[i].CheckForPlayersAndSpawnWave())
            {
                _b_spawnedWave = true;
                if (iL_minibossZones.Contains(i))
                {
                    SpawnBoss(i);
                    iL_minibossZones.RemoveAt(i);
                }
            }

        while (i_numberOfEnemies > 5)
            yield return new WaitForSeconds(1);

        EndWave();

        if (_b_spawnedWave)
            yield return new WaitForSeconds(f_timeBetweenHordes * i_numberOfHordesPerWave);
        StartCoroutine(CheckZoneForPlayers());
    }

    private void SpawnBoss(int _i_area)
    {
        SpawnEnemy(go_miniboss, eszA_allEnemyZones[_i_area].ReturnSpawnPoint(), false);
        DynamicAudioManager.x.StartBoss();
    }

    private void EndWave()
    {
        DynamicAudioManager.x.EndCombat();
    }

    private GameObject PickRandomGroundWaveEnemy()
    {
        return goA_groundWaveEnemies[Random.Range(0, goA_groundWaveEnemies.Length)];
    }
    private GameObject PickRandomFlyingWaveEnemy()
    {
        return goA_flyingWaveEnemies[Random.Range(0, goA_flyingWaveEnemies.Length)];
    }

    internal void SpawnEnemy(GameObject _go_enemyToSpawn, Vector3 _v_spawnPos, bool _b_isFlyingEnemy)
    {
        if (i_numberOfEnemies < ds_currentDifficulty.f_maxNumberOfEnemies)// || _go_enemyToSpawn == go_miniboss)
        {
            PhotonNetwork.Instantiate(_go_enemyToSpawn.name, _v_spawnPos, new Quaternion(0, Random.value, 0, Random.value));
            i_numberOfEnemies = TagManager.x.GetTagSet("Enemy").Count;

            Collider[] _cA = Physics.OverlapSphere(_v_spawnPos, 3, lm_notEnemyLayer);
            for (int i = 0; i < _cA.Length; i++)
                _cA[i].GetComponent<IHitable>()?.TakeDamage(10, false);

        }
    }

    internal void EnemyDied(bool _b_isBoss)
    {
        //if (PhotonNetwork.IsMasterClient)
        i_numberOfEnemies = TagManager.x.GetTagSet("Enemy").Count;

        if (i_numberOfEnemies < 5)
            DynamicAudioManager.x.EndCombat();

        if (_b_isBoss)
            DynamicAudioManager.x.EndBoss();
    }
}