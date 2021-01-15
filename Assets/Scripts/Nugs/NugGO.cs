using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugGO : SubjectBase, IPoolable, ISuckable, IHitable
{
    #region Serialized Variables
    [SerializeField] public Nug nug;
    [SerializeField] private GameObject go_pickupParticles;
    [SerializeField] private GameObject go_destroyParticles;
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
    private ElementalObject eO_elem;
    private bool b_collected;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        eO_elem = GetComponent<ElementalObject>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (!other.GetComponent<PlayerInputManager>().CanPickUpNugs())
                return;

            CurrencyEvent ce = new CurrencyEvent(0, nug.i_worth, true, nug);

            b_collected = true;
            myLode.NugCollected(i_lodeID);
            Notify(ce);
        }
    }

    public void OnEnable()
    {
        Invoke("Die", 60);
        
    }

    public void Die()
    {
        CancelInvoke();
        StopAllCoroutines();
        GameObject particlesToPlay = PoolManager.x.SpawnObject((b_collected? go_pickupParticles : go_destroyParticles), transform.position, Quaternion.identity);
        if (ac_pickupSound)
            AudioSource.PlayClipAtPoint(ac_pickupSound, transform.position);
        if(rb != null)
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

    public void TakeDamage(int damage, bool activatesThunder)
    {
        if (eO_elem)
            eO_elem.ActivateElement(activatesThunder);
        Die();
    }

    public bool IsDead()
    {
        return false;
    }

    public void TakeDamage(int damage, bool activatesThunder, float _delay)
    {
        StartCoroutine(DelayedTakeDamage(damage, activatesThunder, _delay));
    }

    IEnumerator DelayedTakeDamage(int damage, bool activatesThunder, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        TakeDamage(damage, activatesThunder);
    }
}
