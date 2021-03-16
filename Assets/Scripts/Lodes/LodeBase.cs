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
    private int nugCount;
    private bool burst = true;
    private PhotonView view;
    private NugGO[] nuggets;
    [SerializeField] private ParticleSystem p_chunkEffect;
    private List<int> iL_chunkableThreshold = new List<int>();
    [SerializeField] private GameObject[] goA_chunkables = new GameObject[0];
    [SerializeField] private int i_damageBetweenBursts = 35;

    [Header("Audio")]
    [SerializeField] private AudioClip ac_takeDamageClip;
    [SerializeField] private AudioClip ac_destroyedClip;
    private AudioSource as_source;

    protected override void Start()
    {
        i_maxHealth = Mathf.RoundToInt(i_maxHealth * transform.localScale.y);
        for (int i = 0; i < i_maxHealth; i++)
            iL_healthIntervals.Add(i_maxHealth - (i_damageBetweenBursts * i));

        for (int i = 0; i < goA_chunkables.Length; i++)
            iL_chunkableThreshold.Add((i_maxHealth / goA_chunkables.Length) * i);


        nuggets = new NugGO[i_nuggetsPerBurst * ((i_maxHealth / i_damageBetweenBursts) + 3)];
        base.Start();
        view = GetComponent<PhotonView>();
        as_source = GetComponent<AudioSource>();


        //if (mr_mainRenderer)
        //   mr_mainRenderer.material.SetFloat("_emissionMult", f_baseEmission);

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

        //this is the networked take damage func, this is called by the host to sync health
        i_currentHealth -= _i_damage;

        //check if dead and stuff
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(SetHealth), RpcTarget.Others, i_currentHealth);
        CheckHealth();

    }

    [PunRPC]
    public void SetHealth(int health)
    {
        //it's a psuedo set health func so that thresholds are still respected
        //Debug.Log("SettingHealth");
        TakeTrueDamage(i_currentHealth - health);
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
                p_chunkEffect.Play();
                transform.localScale *= 0.97f;

                if (PhotonNetwork.IsMasterClient)
                {
                    view.RPC(nameof(SpawnNuggs), RpcTarget.All, Mathf.RoundToInt(Random.value * 10000));
                }
                if (as_source != null)
                    as_source.PlayOneShot(ac_takeDamageClip);

                iL_healthIntervals.RemoveAt(i);
            }

        for (int i = 0; i < iL_chunkableThreshold.Count; i++)
            if (i_currentHealth <= iL_chunkableThreshold[i])
            {
                goA_chunkables[i].SetActive(false);
            }

        if (i_currentHealth <= 0) Death();

        if (mr_mainRenderer)
            mr_mainRenderer.material.SetFloat("_emissionMult", (((float)i_currentHealth / i_maxHealth)) * 7.5f);

    }

    [PunRPC]
    public void SpawnNuggs(int newSeed)
    {
        for (int j = 0; j < i_nuggetsPerBurst; j++)
            NuggetBurst(newSeed);
    }

    private void NuggetBurst(int _seed)
    {
        //Nick and byron did this
        Random.InitState(_seed);

        GameObject _go_nugget = PoolManager.x.SpawnObject(go_nuggetPrefab, transform.position, transform.rotation);
        nuggets[nugCount] = _go_nugget.GetComponent<NugGO>();
        nuggets[nugCount].SetLodeInfo(nugCount, this);
        nugCount += 1;
        _go_nugget.SetActive(true);
        _go_nugget.transform.parent = null;
        _go_nugget.transform.position = transform.position + transform.localScale * (-1 + Random.value * 2) + Vector3.up;
        //_go_nugget.transform.localScale = Vector3.one;
        Rigidbody _rb = _go_nugget.GetComponent<Rigidbody>();
        _rb.AddForce(new Vector3(-1 + Random.value * 2, Random.value * 2, -1 + Random.value * 2) * f_nuggetForce, ForceMode.Impulse);
        _go_nugget.transform.rotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);

    }

    [PunRPC]
    internal override void Death()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (burst)
            {
                for (int i = 0; i < i_nuggetsPerBurst; i++)
                {
                    int newSeed = Mathf.RoundToInt(Random.value * 10000);

                    view.RPC(nameof(SpawnNuggs), RpcTarget.Others, newSeed);
                }

            }
            burst = true;
            //RPC death function so that all instances of a lode die together
            view.RPC(nameof(Death), RpcTarget.Others);
        }

        p_chunkEffect.Play();

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
        view.RPC(nameof(DestroyNug), RpcTarget.All, id, coll);
    }

    [PunRPC]
    public void DestroyNug(int id, bool coll)
    {
        nuggets[id]?.SetCanDie(coll);
        nuggets[id]?.Die();
        nuggets[id] = null;
    }

    public void NugGotHit(int _index, int _dmg, bool _thunderActivate)
    {
        photonView.RPC(nameof(RemoteNugHit), RpcTarget.Others, _index, _dmg, _thunderActivate);
    }

    [PunRPC]
    public void RemoteNugHit(int _index, int _dmg, bool _thunderActivate)
    {
        if (nuggets[_index] != null)
            nuggets[_index].TakeDamageFromRemote(_dmg, _thunderActivate);
    }

}