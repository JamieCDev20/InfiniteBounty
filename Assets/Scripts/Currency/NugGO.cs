using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugGO : SubjectBase, IPoolable
{
    #region Serialized Variables
    [SerializeField] public Nug nug;
    [SerializeField] private float f_nugTimeout;
    [SerializeField] GameObject go_pickupParticles;
    #endregion

    #region Private Variables
    private int i_poolIndex;
    private bool b_inPool;
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            GameObject particlesToPlay = PoolManager.x.SpawnNewObject(go_pickupParticles, transform.position, transform.rotation);
            CurrencyEvent ce = new CurrencyEvent(other.GetComponent<PlayerController>().ID, nug.i_worth, true);
            Notify(ce);
            Die();
        }
    }

    private void TimeOut()
    {
        Die();
    }

    public bool GetInPool()
    {
        return b_inPool;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public int GetIndex()
    {
        return i_poolIndex;
    }

    public void SetIndex(int _i_index)
    {
        i_poolIndex = _i_index;
    }

    public void SetInPool(bool _b_delta)
    {
        b_inPool = _b_delta;
    }

    public void OnSpawn()
    {
        Invoke("TimeOut", f_nugTimeout);
    }

    public void Die()
    {
        if (PoolManager.x != null) PoolManager.x.ReturnInactiveToPool(gameObject, i_poolIndex);
        SetInPool(true);
    }
}
