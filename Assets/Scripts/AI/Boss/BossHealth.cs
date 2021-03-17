using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviourPun, IHitable
{
    private BossAI boss;
    [SerializeField] private int i_maxHealth;
    private int i_currentHealth;
    private int i_maxHealthSCALED;
    [SerializeField] private GameObject go_deathParticles;
    [SerializeField] private RectTransform rt_healthBar;
    private Image i_healthBarImage;
    private Color c_healthBaseColour;
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
    private bool b_isDead;

    [Header("Armour Info")]
    [SerializeField] private LilyPad[] lpA_armourPlates = new LilyPad[0];
    [SerializeField] private int[] iA_armourHealths = new int[0];

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);

        boss = GetComponent<BossAI>();

        DifficultySet _ds = DifficultyManager.x.ReturnCurrentDifficulty();
        i_currentHealth = Mathf.RoundToInt(i_maxHealth * _ds.f_maxHealthMult) * PhotonNetwork.CurrentRoom.PlayerCount;
        i_maxHealthSCALED = Mathf.RoundToInt(i_maxHealth * _ds.f_maxHealthMult) * PhotonNetwork.CurrentRoom.PlayerCount;

        rt_healthBar.localScale = new Vector3(Mathf.Clamp((float)i_currentHealth / i_maxHealthSCALED, 0, Mathf.Infinity), 1, 1);
        rt_healthBarWhite.localScale = new Vector3(Mathf.Clamp((float)i_currentHealth / i_maxHealthSCALED, 0, Mathf.Infinity), 1, 1);
        i_healthBarImage = rt_healthBar.GetComponent<Image>();
        c_healthBaseColour = i_healthBarImage.color;

        for (int i = 0; i < 50; i++)
        {
            goL_bossHitEffectPool.Add(Instantiate(go_bossHitEffectPrefab, transform.position, Quaternion.identity, transform));
            goL_bossHitEffectPool[i].SetActive(false);
        }

        for (int i = 0; i < lpA_armourPlates.Length; i++)
            lpA_armourPlates[i].Setup(i);

    }

    private void Update()
    {
        if (f_timeSinceDamage >= f_timeToWauitBeforeUpdatingWhite)
            rt_healthBarWhite.localScale = Vector3.Lerp(rt_healthBarWhite.transform.localScale, new Vector3(Mathf.Clamp((float)i_currentHealth / i_maxHealthSCALED, 0, Mathf.Infinity), 1, 1), 5 * Time.deltaTime);
        f_timeSinceDamage += Time.deltaTime;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.X))
            TakeDamage(100000, false);
#endif
    }

    [PunRPC]
    public void Die()
    {
        GetComponentInChildren<Animator>().SetTrigger("Death");
        StartCoroutine(ActualDie());
        rt_healthBarWhite.transform.localScale = Vector3.zero;
        BossArenaManager.x.BossDied();

        foreach (BossProjectile bp in FindObjectsOfType<BossProjectile>())
            bp.Die();
    }
    private IEnumerator ActualDie()
    {
        yield return new WaitForSeconds(5);

        go_deathParticles.SetActive(true);
        go_deathParticles.transform.parent = null;

        yield return new WaitForSeconds(1);

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
    private IEnumerator TakeDamageRPC(int _i_newHealth)
    {
        i_currentHealth = _i_newHealth;
        if (i_currentHealth <= 0)
        {
            if (!b_isDead)
                photonView.RPC(nameof(Die), RpcTarget.All);
            b_isDead = true;
        }
        rt_healthBar.localScale = new Vector3(Mathf.Clamp((float)i_currentHealth / i_maxHealthSCALED, 0, Mathf.Infinity), 1, 1);
        f_timeSinceDamage = 0;

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.02f);
            i_healthBarImage.color = Color.white;
            yield return new WaitForSeconds(0.02f);
            i_healthBarImage.color = c_healthBaseColour;
        }
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay) { }

    [PunRPC]
    private void NuggetBurst(int _seed)
    {
        Random.InitState(_seed);

        for (int x = 0; x < i_nuggetsOnDeath; x++)
        {
            GameObject _go_nugget = PoolManager.x.SpawnObject(goA_nuggetPrefabs[Random.Range(0, goA_nuggetPrefabs.Length)], transform.position, transform.rotation);
            _go_nugget.transform.parent = null;
            _go_nugget.SetActive(true);
            _go_nugget.transform.position = new Vector3(transform.position.x, 10, transform.position.z);
            Rigidbody _rb = _go_nugget.GetComponent<Rigidbody>();
            _rb.AddForce(new Vector3(RandomMinusToPositive(), Mathf.Abs(RandomMinusToPositive()), RandomMinusToPositive()) * f_nuggetForce, ForceMode.Impulse);
            _go_nugget.transform.rotation = new Quaternion(RandomMinusToPositive(), RandomMinusToPositive(), RandomMinusToPositive(), RandomMinusToPositive());
        }
    }

    private float RandomMinusToPositive()
    {
        return (float)(-1 + (Random.value * 2));
    }


    internal void ArmourDamaged(int _i_armourIndex, int damage)
    {
        photonView.RPC(nameof(ArmourDamagedRPC), RpcTarget.All, _i_armourIndex, iA_armourHealths[_i_armourIndex] - damage);
    }

    [PunRPC]
    public void ArmourDamagedRPC(int _i_armourIndex, int _i_newHealth)
    {
        iA_armourHealths[_i_armourIndex] = _i_newHealth;

        if (iA_armourHealths[_i_armourIndex] < 0)
            lpA_armourPlates[_i_armourIndex].Die();
    }

}
