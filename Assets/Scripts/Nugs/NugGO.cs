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
    [SerializeField] private float f_spawnImmunityDuration = 0.5f;
    #endregion

    #region Private Variables
    private int i_poolIndex;
    private int i_lodeID;
    private bool b_inPool;
    private LodeBase myLode;
    private Rigidbody rb;
    private ElementalObject eO_elem;
    private bool b_collected;
    private bool b_canBeHit = false;
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
        b_canBeHit = false;
        Invoke("RemoveSpawnImmunity", f_spawnImmunityDuration);
        Invoke("Die", 60);
    }

    private void OnDisable()
    {
        CancelInvoke("Die");
    }

    private void RemoveSpawnImmunity()
    {
        b_canBeHit = true;
    }

    public void Die()
    {
        if (!b_canBeHit)
            return;
        CancelInvoke();
        StopAllCoroutines();
        GameObject particlesToPlay = PoolManager.x.SpawnObject((b_collected? go_pickupParticles : go_destroyParticles), transform.position, Quaternion.identity);
        if (ac_pickupSound)
            AudioSource.PlayClipAtPoint(ac_pickupSound, transform.position);
        if(rb != null)
            rb.velocity = Vector3.zero;
        eO_elem.ResetElements();
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
        if (!b_canBeHit)
            return;
        if (eO_elem)
        {
            eO_elem.ActivateElement(activatesThunder); 
            Collider[] cols = Physics.OverlapSphere(transform.position, 1.5f);
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].GetComponent<IElementable>()?.RecieveElements(eO_elem.GetActiveElements());
            }
        }
        myLode.NugGotHit(i_lodeID, damage, activatesThunder);
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

    public void TakeDamageFromRemote(int _i_damage, bool activatesThunder)
    {
        if (eO_elem)
            eO_elem.ActivateElement(activatesThunder);
    }

}
