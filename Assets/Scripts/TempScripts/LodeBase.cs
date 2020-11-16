using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LodeBase : Enemy, IPoolable, IPunObservable
{
    [Header("Lode Base")]
    [SerializeField] private GameObject go_nuggetPrefab;
    [SerializeField] private int i_nuggetToSpawn;
    [SerializeField] private int i_nuggetsPerBurst;
    [SerializeField] private float f_nuggetForce;
    [SerializeField] private int[] iA_healthIntervals = new int[3];
    [SerializeField] private GameObject[] goA_damageMeshes = new GameObject[3];
    [Space, SerializeField] private bool b_isNetworkedObject;
    [SerializeField] private string s_path;

    private int index;
    private PhotonView view;

    protected override void Start()
    {
        base.Start();
        view = GetComponent<PhotonView>();
    }

    protected void OnEnable()
    {
        transform.position += Vector3.one;
        transform.position -= Vector3.one;
    }

    internal override void TakeDamage(int _i_damage)
    {

        if (!PhotonNetwork.IsMasterClient)
            return;

        TakeDamage(_i_damage, true);

    }

    internal override void TakeDamage(int _i_damage, bool networked)
    {

        i_currentHealth -= _i_damage;
        for (int i = 0; i < iA_healthIntervals.Length; i++)
        {
            if (i_currentHealth <= iA_healthIntervals[i])
            {
                NuggetBurst();
                goA_damageMeshes[i].SetActive(false);
                iA_healthIntervals[i] = 0;
            }
        }

        //LodeSynchroniser.x.SyncHealth(i_currentHealth, index);
        Debug.Log("lode health: " + i_currentHealth);
        CheckHealth();

    }

    public void SetHealth(int health)
    {

        TakeDamage(i_currentHealth - health);

    }

    public void SetIndex(int i)
    {
        index = i;
    }

    private void CheckHealth()
    {
        if (i_currentHealth <= 0) Death();

    }

    [PunRPC]
    internal override void Death()
    {
        Debug.LogFormat("Setting {0} to inactive", name);
        gameObject.SetActive(false);
        NuggetBurst();

        if (!PhotonNetwork.IsMasterClient)
            view.RPC("Death", RpcTarget.Others);

    }

    private void NuggetBurst()
    {
        for (int i = 0; i < i_nuggetsPerBurst; i++)
        {
            GameObject _go_nugget = PoolManager.x.SpawnObject(go_nuggetPrefab, transform.position, transform.rotation);
            _go_nugget.SetActive(true);
            _go_nugget.transform.parent = null;
            _go_nugget.transform.position = transform.position + transform.localScale * (-1 + Random.value * 2);
            _go_nugget.transform.localScale = Vector3.one;
            Rigidbody _rb = _go_nugget.GetComponent<Rigidbody>();
            _rb.AddForce(new Vector3(-1 + Random.value * 2, Random.value * 2, -1 + Random.value * 2) * f_nuggetForce, ForceMode.Impulse);
            _go_nugget.transform.rotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Die()
    {
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(i_currentHealth);
        }
        else
        {
            if (stream.Count > 0)
                TakeDamage(i_currentHealth - (int)stream.ReceiveNext(), true);
        }

    }

}