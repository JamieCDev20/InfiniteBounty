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

        TakeTrueDamage(_i_damage, true);

    }

    public void TakeTrueDamage(int _i_damage, bool networked)
    {

        //this is the networked take damage func, this is called by the host to sync health

        i_currentHealth -= _i_damage;
        for (int i = 0; i < iA_healthIntervals.Length; i++)
        {
            if (i_currentHealth <= iA_healthIntervals[i])
            {
                NuggetBurst();
                goA_damageMeshes[i].SetActive(false);
                iA_healthIntervals[i] = -10000000;
            }
        }

        //check if dead and stuff
        CheckHealth();

    }

    public void SetHealth(int health)
    {
        //it's a psuedo set health func so that thresholds are still respected
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
        //RPC death function so that all instances of a lode die together
        gameObject.SetActive(false);
        NuggetBurst();

        if (PhotonNetwork.IsMasterClient)
            view.RPC("Death", RpcTarget.Others);
    }

    private void NuggetBurst()
    {
        //Nick and byron did this
        for (int i = 0; i < i_nuggetsPerBurst; i++)
        {
            GameObject _go_nugget = PoolManager.x.SpawnObject(go_nuggetPrefab, transform.position, transform.rotation);
            nuggets[nugCount] = _go_nugget.GetComponent<NugGO>();
            nuggets[nugCount].SetLodeInfo(nugCount, this);
            nugCount += 1;
            _go_nugget.SetActive(true);
            _go_nugget.transform.parent = null;
            _go_nugget.transform.position = transform.position + transform.localScale * (-1 + Random.value * 2);
            //_go_nugget.transform.localScale = Vector3.one;
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

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
            
        //Sync your health
        if (stream.IsWriting)
        {
            stream.SendNext(i_currentHealth);
            for (int i = 0; i < nuggets.Length; i++)
            {
                stream.SendNext($"{i}#{(nuggets[i]? nuggets[i].transform.position : transform.position).ToString().Replace("(", "").Replace(")", "")}");
            }
        }
        else
        {
            TakeTrueDamage(i_currentHealth - (int)stream.ReceiveNext(), true);
            Vector3 v = Vector3.zero;
            while(stream.Count > 0)
            {
                Debug.Log(stream.Count);
                string t = (string)stream.ReceiveNext();
                Debug.Log(t);
                string[] tB = t.Split('#');
                string[] tA = tB[1].Split(',');
                v.x = float.Parse(tA[0]);
                v.y = float.Parse(tA[1]);
                v.z = float.Parse(tA[2]);
                nuggets[int.Parse(tB[0])].transform.position = v;
            }
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