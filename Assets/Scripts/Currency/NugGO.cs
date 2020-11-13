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
        if (other.transform.tag == "Player")
        {
            GameObject particlesToPlay = OldPoolManager.x.SpawnNewObject(go_pickupParticles, transform.position, transform.rotation);
            if (other.GetComponent<PlayerController>() != null)
            {
                CurrencyEvent ce = new CurrencyEvent(other.GetComponent<PlayerController>().ID, nug.i_worth, true);
                Notify(ce);
            }
            Die();
        }
    }

    private void TimeOut()
    {
        Die();
    }

    public void Die()
    {
        PoolManager.x.ReturnObjectToPool(gameObject);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
