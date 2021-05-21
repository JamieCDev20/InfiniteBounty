using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnder : MonoBehaviourPun, IInteractible
{
    [SerializeField] private GameObject go_tractorBeam;
    [SerializeField] private Transform t_progressBar;
    [SerializeField] private Animator a_shipAnimator;
    [SerializeField] private float f_waitTime = 10;
    [SerializeField] private float f_animationLength = 15;
    private bool b_active;

    [Header("Enemy Thangs")]
    [SerializeField] private float f_timeBetweenWaves;
    [Space]
    [SerializeField] private Vector2Int vi_groobersPerWave;
    [SerializeField] private GameObject[] goA_grooberTypes = new GameObject[0];
    [SerializeField] private float f_grooberSpawnDistance;
    [Space]
    [SerializeField] private Vector2Int vi_skyratsPerWave;
    [SerializeField] private GameObject[] goA_skyratTypes = new GameObject[0];
    [SerializeField] private float f_skyratSpawnDistance;
    private GameObject go_looker;

    [Header("Sound")]
    [SerializeField] private AudioClip ac_buttonPressed;
    [SerializeField] private AudioClip ac_shipArriving;
    private AudioSource as_source;
    [SerializeField] private ParticleSystem ps_signalParticleEffect;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        go_looker = new GameObject("EndLevelLooker");
        go_looker.transform.position = transform.position;

        t_progressBar.localPosition = new Vector3(0, -1, 0);
        t_progressBar.transform.localScale = new Vector3(1, 0, 1);
    }

    #region Interactions

    public void Interacted() { }

    public void Interacted(Transform interactor)
    {
        if (!b_active)
            photonView.RPC(nameof(Countdown), RpcTarget.All, Random.Range(0, 999999), PhotonNetwork.NickName);
    }

    #endregion

    [PunRPC]
    private IEnumerator Countdown(int _i_seed, string _s_nickName)
    {
        InfoText.x?.OnNotify(new InfoTextEvent(_s_nickName + " has called your ship for pickup!"));

        ps_signalParticleEffect.Play();

        as_source.PlayOneShot(ac_buttonPressed);
        b_active = true;
        float _f_currentTime = 0;
        float _f_enemyWaveTime = 0;
        Random.InitState(_i_seed);
        bool _b_shouldShip = true;

        while (_f_currentTime < f_waitTime)
        {
            _f_currentTime += Time.deltaTime;
            _f_enemyWaveTime += Time.deltaTime;

            if (_f_enemyWaveTime >= f_timeBetweenWaves)
            {
                _f_enemyWaveTime = 0;
                SpawnWave();
            }

            t_progressBar.localPosition = new Vector3(0, (float)(-1 + (_f_currentTime / f_waitTime)), 0);
            t_progressBar.transform.localScale = new Vector3(1, (float)(_f_currentTime / f_waitTime), 1);

            if (_f_currentTime >= f_waitTime - f_animationLength && _b_shouldShip)
            {
                a_shipAnimator.SetBool("Entry", true);
                as_source.PlayOneShot(ac_shipArriving);
                _b_shouldShip = false;
                InfoText.x?.OnNotify(new InfoTextEvent("Your ship is entering the atmosphere now."));
                ps_signalParticleEffect.Stop();
            }

            yield return new WaitForEndOfFrame();
        }

        go_tractorBeam.SetActive(true);
    }

    private void SpawnWave()
    {
        float _f_enemyCount = Random.Range(vi_groobersPerWave.x, vi_groobersPerWave.y) * DifficultyManager.x.ReturnCurrentDifficulty().f_spawnAmountMult;
        go_looker.transform.rotation = new Quaternion(0, Random.value, 0, Random.value);
        for (int i = 0; i < _f_enemyCount; i++)
        {
            EnemySpawner.x.SpawnEnemy(goA_grooberTypes[Random.Range(0, goA_grooberTypes.Length)], (transform.position + go_looker.transform.forward * f_grooberSpawnDistance) + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)), false);
            go_looker.transform.Rotate(Vector3.up * 3);
        }

        _f_enemyCount = Random.Range(vi_skyratsPerWave.x, vi_skyratsPerWave.y) * DifficultyManager.x.ReturnCurrentDifficulty().f_spawnAmountMult;
        go_looker.transform.rotation = new Quaternion(0, Random.value, 0, Random.value);
        for (int i = 0; i < _f_enemyCount; i++)
        {
            EnemySpawner.x.SpawnEnemy(goA_skyratTypes[Random.Range(0, goA_skyratTypes.Length)], (transform.position + go_looker.transform.forward * f_grooberSpawnDistance) + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) + (Vector3.up * 3), true);
            go_looker.transform.Rotate(Vector3.up * 3);
        }

    }
}
