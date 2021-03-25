using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LodeBase : Enemy, IHitable
{
    [Header("Lode Base")]
    [SerializeField] private GameObject go_nuggetPrefab;
    //[SerializeField] private int i_nuggetToSpawn;
    [SerializeField] private int i_nuggetsPerBurst;
    [SerializeField] private float f_nuggetForce;
    private List<int> iL_healthIntervals = new List<int>();
    [Space, SerializeField] private bool b_isNetworkedObject;
    [SerializeField] private string s_path;
    [SerializeField] private MeshRenderer mr_mainRenderer;
    [SerializeField] private float f_baseEmission = 7.5f;

    private int index;
    private bool burst = true;
    private List<NugGO> nuggets = new List<NugGO>();
    private List<int> iL_chunkableThreshold = new List<int>();
    [SerializeField] private GameObject[] goA_chunkables = new GameObject[0];
    [SerializeField] private int i_damageBetweenBursts = 35;
    [Space]
    [SerializeField] private ParticleSystem p_hitEffect;
    [SerializeField] private ParticleSystem p_chunkEffect;

    [Header("Audio")]
    [SerializeField] private AudioClip ac_takeDamageClip;
    [SerializeField] private AudioClip ac_destroyedClip;
    private AudioSource as_source;
    private int i_myID;

    protected override void Start()
    {
        i_maxHealth = Mathf.RoundToInt(i_maxHealth * transform.localScale.y);
        for (int i = 0; i < i_maxHealth; i++)
            iL_healthIntervals.Add(i_maxHealth - (i_damageBetweenBursts * i));

        for (int i = 0; i < goA_chunkables.Length; i++)
            iL_chunkableThreshold.Add((i_maxHealth / goA_chunkables.Length) * i);


        //nuggets = new NugGO[i_nuggetsPerBurst * (2*(i_maxHealth / i_damageBetweenBursts) + 3)];
        base.Start();
        as_source = GetComponent<AudioSource>();


        //if (mr_mainRenderer)
        //   mr_mainRenderer.material.SetFloat("_emissionMult", f_baseEmission);

    }

    public void SetID(int _i_id)
    {
        i_myID = _i_id;
    }

    public override void TakeDamage(int _i_damage, bool activatesThunder)
    {
        //only take damage if you are the master client
        if (!PhotonNetwork.IsMasterClient)
            return;

        TakeTrueDamage(_i_damage);

    }

    public void TakeTrueDamage(int _i_damage)
    {
        LodeSynchroniser.x.LodeTookDamage(i_myID, _i_damage);
    }

    public void SetHealth(int health)
    {
        //it's a psuedo set health func so that thresholds are still respected
        //Debug.Log("SettingHealth");
        i_currentHealth = health;
        CheckHealth();

    }

    public void SetIndex(int i)
    {
        index = i;
    }

    private void CheckHealth()
    {
        as_source.PlayOneShot(ac_takeDamageClip);

        for (int i = 0; i < iL_healthIntervals.Count; i++)
            if (i_currentHealth <= iL_healthIntervals[i])
            {
                p_hitEffect.Play();
                transform.localScale *= 0.97f;

                if (PhotonNetwork.IsMasterClient)
                    LodeSynchroniser.x.SpawnNuggsFromLode(i_myID);

                if (as_source != null)
                    as_source.PlayOneShot(ac_takeDamageClip);

                iL_healthIntervals.RemoveAt(i);
            }

        for (int i = 0; i < iL_chunkableThreshold.Count; i++)
            if (i_currentHealth <= iL_chunkableThreshold[i])
            {
                p_chunkEffect.Play();
                goA_chunkables[i].SetActive(false);

                iL_chunkableThreshold.RemoveAt(i);
                iL_chunkableThreshold.RemoveAt(i);
            }

        if (i_currentHealth <= 0) Death();

        if (mr_mainRenderer)
            mr_mainRenderer.material.SetFloat("_emissionMult", (((float)i_currentHealth / i_maxHealth)) * 7.5f);

    }

    public void SpawnNuggs(int newSeed)
    {
        for (int j = 0; j < i_nuggetsPerBurst; j++)
            NuggetBurst(newSeed);
    }

    private void NuggetBurst(int _seed)
    {
        //Nick and byron did this
        Random.InitState(_seed);
        //Debug.LogError("THIS IS MY SEED NOW " + _seed);


        GameObject _go_nugget = PoolManager.x.SpawnObject(go_nuggetPrefab, transform.position, transform.rotation);
        NugGO ngo = _go_nugget.GetComponent<NugGO>();
        nuggets.Add(ngo);
        ngo.SetLodeInfo(nuggets.Count - 1, this);

        _go_nugget.SetActive(true);
        _go_nugget.transform.parent = null;
        _go_nugget.transform.position = transform.position + transform.localScale * (-1 + Random.value * 2) + Vector3.up;
        //_go_nugget.transform.localScale = Vector3.one;
        _go_nugget.transform.rotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);
        Rigidbody _rb = _go_nugget.GetComponent<Rigidbody>();
        _rb.AddForce(new Vector3(-1 + Random.value * 2, Random.value * 2, -1 + Random.value * 2) * f_nuggetForce, ForceMode.Impulse);
        _go_nugget.transform.rotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);

    }


    internal override void Death()
    {
        SpawnNuggs(6750);

        p_hitEffect.Play();

        AudioSource.PlayClipAtPoint(ac_destroyedClip, transform.position);
        gameObject.SetActive(false);
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Die()
    {
        burst = false;
        Death();
    }

    public bool IsNetworkedObject()
    {
        return b_isNetworkedObject;
    }

    public string ResourcePath()
    {
        return s_path;
    }

    public void NugCollected(int id, bool coll)
    {
        LodeSynchroniser.x.DestroyUpNugg(i_myID, id, coll);
    }


    public void DestroyNug(int id, bool coll)
    {
        nuggets[id]?.SetCanDie(coll);
        nuggets[id]?.Die();
        nuggets[id] = null;
    }

    public void NugGotHit(int _index, int _dmg, bool _thunderActivate)
    {
        LodeSynchroniser.x.DestroyUpNugg(i_myID, _index, false);
    }

    [PunRPC]
    public void RemoteNugHit(int _index, int _dmg, bool _thunderActivate)
    {
        if (nuggets[_index] != null)
            nuggets[_index].TakeDamageFromRemote(_dmg, _thunderActivate);
    }

    public int GetHealth()
    {
        return i_currentHealth;
    }

}