using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LodeBase : Enemy, IPoolable, IPunObservable, IHitable
{
    [Header("Lode Base")]
    [SerializeField] private GameObject go_nuggetPrefab;
    //[SerializeField] private int i_nuggetToSpawn;
    [SerializeField] private int i_nuggetsPerBurst;
    [SerializeField] private float f_nuggetForce;
    [SerializeField] private int[] iA_healthIntervals = new int[3];
    [SerializeField] private GameObject[] goA_damageMeshes = new GameObject[3];
    [Space, SerializeField] private bool b_isNetworkedObject;
    [SerializeField] private string s_path;

    private int index;
    private int nugCount;
    private bool burst = true;
    private PhotonView view;
    private NugGO[] nuggets;

    protected override void Start()
    {
        nuggets = new NugGO[i_nuggetsPerBurst * (iA_healthIntervals.Length + 3)];
        base.Start();
        view = GetComponent<PhotonView>();
    }

    public override void TakeDamage(int _i_damage)
    {
        //only take damage if you are the master client
        if (!PhotonNetwork.IsMasterClient)
            return;

        TakeTrueDamage(_i_damage);

    }

    public void TakeTrueDamage(int _i_damage)
    {

        //this is the networked take damage func, this is called by the host to sync health
        Debug.Log("lode taking damage");
        i_currentHealth -= _i_damage;


        //check if dead and stuff
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("SetHealth", RpcTarget.Others, i_currentHealth);
        CheckHealth();

    }

    [PunRPC]
    public void SetHealth(int health)
    {
        //it's a psuedo set health func so that thresholds are still respected
        Debug.Log("SettingHealth");
        TakeTrueDamage(i_currentHealth - health);
        CheckHealth();

    }

    public void SetIndex(int i)
    {
        index = i;
    }

    private void CheckHealth()
    {
        for (int i = 0; i < iA_healthIntervals.Length; i++)
        {
            if (i_currentHealth <= iA_healthIntervals[i])
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    for (int j = 0; j < i_nuggetsPerBurst; j++)
                    {
                        float[] v = new float[] { Random.value, Random.value, Random.value, Random.value, Random.value, Random.value, Random.value, Random.value };
                        NuggetBurst(v);
                        view.RPC("NuggetBurst", RpcTarget.Others, v);

                    }
                }
                goA_damageMeshes[i].SetActive(false);
                iA_healthIntervals[i] = -10000000;
            }
        }
        if (i_currentHealth <= 0) Death();

    }

    [PunRPC]
    internal override void Death()
    {
        gameObject?.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            if (burst)
            {
                for (int i = 0; i < i_nuggetsPerBurst; i++)
                {
                    float[] v = new float[] { Random.value, Random.value, Random.value, Random.value, Random.value, Random.value, Random.value, Random.value };
                    NuggetBurst(v);
                    view.RPC("NuggetBurst", RpcTarget.Others, v);

                }

            }
            burst = true;
            //RPC death function so that all instances of a lode die together
            view.RPC("Death", RpcTarget.Others);

        }

    }

    [PunRPC]
    private void NuggetBurst(params float[] v)
    {
        //Nick and byron did this

        GameObject _go_nugget = PoolManager.x.SpawnObject(go_nuggetPrefab, transform.position, transform.rotation);
        nuggets[nugCount] = _go_nugget.GetComponent<NugGO>();
        nuggets[nugCount].SetLodeInfo(nugCount, this);
        nugCount += 1;
        _go_nugget.SetActive(true);
        _go_nugget.transform.parent = null;
        _go_nugget.transform.position = transform.position + transform.localScale * (-1 + v[0] * 2);
        //_go_nugget.transform.localScale = Vector3.one;
        Rigidbody _rb = _go_nugget.GetComponent<Rigidbody>();
        _rb.AddForce(new Vector3(-1 + v[1] * 2, v[2] * 2, -1 + v[3] * 2) * f_nuggetForce, ForceMode.Impulse);
        _go_nugget.transform.rotation = new Quaternion(v[4], v[5], v[6], v[7]);

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

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Sync your health
        if (stream.IsWriting)
        {
            //stream.SendNext(i_currentHealth);
        }
        else
        {
            //if(stream.Count > 0)
            //TakeTrueDamage(i_currentHealth - (int)stream.ReceiveNext(), true);
        }

    }

    public void NugCollected(int id)
    {

        view.RPC("DestroyNug", RpcTarget.All, id);
    }

    [PunRPC]
    public void DestroyNug(int id)
    {
        nuggets[id]?.Die();
        nuggets[id] = null;
    }

}