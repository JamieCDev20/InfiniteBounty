using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviourPun, IHitable
{
    private BossAI boss;
    [SerializeField] private int i_maxHealth;
    private int i_currentHealth;
    [SerializeField] private GameObject go_deathParticles;
    [SerializeField] private RectTransform rt_healthBar;
    [SerializeField] private RectTransform rt_healthBarWhite;
    private float f_timeSinceDamage;
    [SerializeField] private float f_timeToWauitBeforeUpdatingWhite = 1.5f;

    [Header("Exploding into Nugs")]
    [SerializeField] private GameObject[] goA_nuggetPrefabs = new GameObject[0];
    [SerializeField] private float f_nuggetForce;
    [SerializeField] private int i_nuggetsOnDeath;

    [Header("Damage Feedback")]
    [SerializeField] private GameObject go_bossHitEffectPrefab;
    private List<GameObject> goL_bossHitEffectPool = new List<GameObject>();

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);

        boss = GetComponent<BossAI>();
        i_maxHealth *= PhotonNetwork.CurrentRoom.PlayerCount;

        i_currentHealth = i_maxHealth;
        rt_healthBar.localScale = new Vector3(Mathf.Clamp((float)i_currentHealth / i_maxHealth, 0, Mathf.Infinity), 1, 1);
        rt_healthBarWhite.localScale = new Vector3(Mathf.Clamp((float)i_currentHealth / i_maxHealth, 0, Mathf.Infinity), 1, 1);

        for (int i = 0; i < 50; i++)
        {
            goL_bossHitEffectPool.Add(Instantiate(go_bossHitEffectPrefab, transform.position, Quaternion.identity, transform));
            goL_bossHitEffectPool[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (f_timeSinceDamage >= f_timeToWauitBeforeUpdatingWhite)
            rt_healthBarWhite.localScale = Vector3.Lerp(rt_healthBarWhite.transform.localScale, new Vector3(Mathf.Clamp((float)i_currentHealth / i_maxHealth, 0, Mathf.Infinity), 1, 1), 5 * Time.deltaTime);
        f_timeSinceDamage += Time.deltaTime;
    }

    [PunRPC]
    public void Die()
    {
        go_deathParticles.SetActive(true);
        go_deathParticles.transform.parent = null;
        Invoke(nameof(ActualDie), 1);
        rt_healthBarWhite.transform.localScale = Vector3.zero;
    }
    private void ActualDie()
    {
        foreach (BossProjectile bp in FindObjectsOfType<BossProjectile>())
            bp.Die();

        BossArenaManager.x.BossDied();
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(NuggetBurst), RpcTarget.All, Random.Range(0, 9999999));

        gameObject.SetActive(false);
    }

    public bool IsDead()
    {
        return !gameObject.activeInHierarchy;
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, i_currentHealth - damage);
    }
    [PunRPC]
    private void TakeDamageRPC(int _i_newHealth)
    {
        i_currentHealth = _i_newHealth;
        if (i_currentHealth <= 0)
            photonView.RPC(nameof(Die), RpcTarget.All);
        rt_healthBar.localScale = new Vector3(Mathf.Clamp((float)i_currentHealth / i_maxHealth, 0, Mathf.Infinity), 1, 1);
        f_timeSinceDamage = 0;
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay) { }

    [PunRPC]
    private void NuggetBurst(int _seed)
    {
        Random.InitState(_seed);

        //for (int i = 0; i < 10; i++)
        {
            //yield return new WaitForSeconds(0.1f);
            for (int x = 0; x < i_nuggetsOnDeath; x++)
            {
                GameObject _go_nugget = PoolManager.x.SpawnObject(goA_nuggetPrefabs[Random.Range(0, goA_nuggetPrefabs.Length)], transform.position, transform.rotation);
                _go_nugget.transform.parent = null;
                _go_nugget.SetActive(true);
                _go_nugget.transform.position = transform.position + transform.localScale * (RandomMinusToPositive()) + Vector3.up;
                Rigidbody _rb = _go_nugget.GetComponent<Rigidbody>();
                _rb.AddForce(new Vector3(RandomMinusToPositive(), Mathf.Abs(RandomMinusToPositive()), RandomMinusToPositive()) * f_nuggetForce, ForceMode.Impulse);
                _go_nugget.transform.rotation = new Quaternion(RandomMinusToPositive(), RandomMinusToPositive(), RandomMinusToPositive(), RandomMinusToPositive());
            }
        }
    }

    private float RandomMinusToPositive()
    {
        return (float)(-1 + (Random.value * 2));
    }

}
