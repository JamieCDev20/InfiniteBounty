using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugGO : SubjectBase, IPoolable, ISuckable
{
    #region Serialized Variables
    [SerializeField] public Nug nug;
    [SerializeField] private float f_nugTimeout;
    [SerializeField] private GameObject go_pickupParticles;
    [SerializeField] private AudioClip ac_pickupSound;
    [SerializeField] private bool b_isNetworkedObject = true;
    [SerializeField] private string s_resourcePath;
    #endregion

    #region Private Variables
    private int i_poolIndex;
    private int i_lodeID;
    private bool b_inPool;
    private LodeBase myLode;
    private Rigidbody rb;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (!other.GetComponent<PlayerInputManager>().CanPickUpNugs())
                return;

            CurrencyEvent ce = new CurrencyEvent(0, nug.i_worth, true);

            myLode.NugCollected(i_lodeID);
            Notify(ce);
        }
    }

    private void TimeOut()
    {
        Die();
    }

    public void Die()
    {
        GameObject particlesToPlay = PoolManager.x.SpawnObject(go_pickupParticles, transform.position, Quaternion.identity);
        if (ac_pickupSound)
            AudioSource.PlayClipAtPoint(ac_pickupSound, transform.position);
        rb.velocity = Vector3.zero;
        PoolManager.x.ReturnObjectToPool(gameObject);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool IsNetworkedObject()
    {
        return b_isNetworkedObject;
    }

    public string ResourcePath()
    {
        return s_resourcePath;
    }

    public void SetLodeInfo(int id, LodeBase lode)
    {
        i_lodeID = id;
        myLode = lode;
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }
}
