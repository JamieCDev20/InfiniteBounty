using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class NugGO : SubjectBase, IPoolable, ISuckable, IHitable
{
    #region Serialized Variables
    [SerializeField] public Nug nug;
    [SerializeField] private GameObject go_pickupParticles;
    [SerializeField] private GameObject go_destroyParticles;
    [SerializeField] private AudioClip ac_pickupSound;
    [SerializeField] private AudioMixer am_nugMixer;
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
    private bool b_canDie = false;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        eO_elem = GetComponentInChildren<ElementalObject>();

        SceneManager.sceneLoaded += OnSceneLoad;

        if (DiversifierManager.x.ReturnIfDiverIsActive(Diversifier.ZeroGNuggs))
            rb.useGravity = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (!other.GetComponent<PlayerInputManager>().CanPickUpNugs())
                return;

            CurrencyEvent ce = new CurrencyEvent(0, nug.i_worth, true, nug);

            b_collected = true;
            myLode?.NugCollected(i_lodeID, b_collected);
            Notify(ce);
            if (myLode == null)
                Die();
        }
    }

    public override void OnEnable()
    {
        b_canBeHit = false;
        b_collected = false;
        gameObject.layer = 8;
        Invoke(nameof(RemoveSpawnImmunity), f_spawnImmunityDuration);
        Invoke(nameof(Die), 60);
    }

    public override void OnDisable()
    {
        CancelInvoke(nameof(RemoveSpawnImmunity));
        CancelInvoke(nameof(Die));
        b_collected = false;
    }

    private void RemoveSpawnImmunity()
    {
        b_canBeHit = true;
        gameObject.layer = 9;
    }

    public void Die()
    {
        if (!b_canBeHit)
            return;
        CancelInvoke();
        StopAllCoroutines();
        GameObject particlesToPlay = PoolManager.x.SpawnObject((b_collected ? go_pickupParticles : go_destroyParticles), transform.position, Quaternion.identity);
        /*
        float vol = 0;
        if (am_nugMixer)
            am_nugMixer.GetFloat("Volume", out vol);
        vol = (vol + 80) / 80;
        if (ac_pickupSound)
            AudioSource.PlayClipAtPoint(ac_pickupSound, transform.position, vol);
        */
        if (rb != null)
            rb.velocity = Vector3.zero;
        eO_elem?.ResetElements();
        PoolManager.x.ReturnObjectToPool(gameObject);
    }

    public void SetCanDie(bool collected)
    {
        b_canBeHit = true;
        b_collected = collected;
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
        if (eO_elem && !b_canDie)
        {
            eO_elem.ActivateElement(activatesThunder);
            Collider[] cols = Physics.OverlapSphere(transform.position, 1.5f);
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].GetComponent<IElementable>()?.ReceiveElements(eO_elem.GetActiveElements());
            }
            if (!eO_elem.GetShouldDie())
                return;
        }

        if (b_canDie)
            myLode?.NugGotHit(i_lodeID, damage, activatesThunder);
        else
        {
            b_canDie = true;
            StartCoroutine(DelayedTakeDamage(damage, activatesThunder, 1f));
        }
    }

    public void DelayedSendHit(int damage, bool activatesThunder)
    {
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

    private void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        Die();
    }

}
