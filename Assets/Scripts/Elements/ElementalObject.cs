using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElementalObject : MonoBehaviour, IElementable
{
    [SerializeField] private List<Element> eL_activeElements = new List<Element>(); //The elements on this object
    [SerializeField] private List<Element> eL_elementImmunities = new List<Element>(); //The elemental immunities of this object
    [SerializeField] private Mesh mesh;

    private bool[] bA_statuses = new bool[7] 
    //Goo   Hydro   Tasty Thunder Boom    Fire  Lava
    { false, false, false, false, false, false, false }; //status effects on the object

    private GameObject[] goA_effects = new GameObject[7];

    private List<Element> InitElements = new List<Element>();

    private delegate void ElementInteraction(); //Delegates for Elemental Interactions
    private delegate void ElementActivation(); //Delegates for Elemental Activations

    private ElementActivation activated; // The delegate called when the object get's activated

    private ElementInteraction[,] interactions; //The big ass 2D array of interactions
    private ElementActivation[] activations; //The array of activation functions

    private bool b_doThunder = true; // bool to stop thunder infinitely repeating
    private bool b_doBoom = true; // bool to stop boom
    private LineRenderer lrend;
    private PoolableObject pO; //To store the line renderer object
    private bool b_activatedThisFrame = false; //only activate once per frame <<Not sure if i actually need this anymore.. but better safe than sorry
    private bool flag; // ^^
    [SerializeField] private GameObject hittableObject;
    private IHitable ourHitable;
    private bool b_shouldDie = true;
    private float lastCollided;

    private bool running = true;

    private int id;
    private List<int> usedIDs = new List<int>();

    private float[] starteds = new float[7];
    private float[] lastTook = new float[7];
    private bool doneInit = false;

    private void Start()
    {
        pO = GetComponent<PoolableObject>();

        if (ElementManager.x)
            id = ElementManager.x.GetID();

        if (hittableObject != null)
            ourHitable = hittableObject.GetComponent<IHitable>();

        if (mesh == null)
            mesh = GetComponentInChildren<MeshFilter>()?.mesh;

        if (doneInit)
            return;
        InitElements = eL_activeElements;
        InitInteractions();
        InitialiseActivations();

    }

    private void OnCollisionEnter(Collision col)
    {
        float t = Time.realtimeSinceStartup;
        if (t - lastCollided < 0.1f)
            return;
        IElementable ie = col.gameObject.GetComponentInChildren<IElementable>();
        if (ie != null)
        {
            if (!usedIDs.Contains(ie.ID()))
            {
                AddToUsed(ie.ID());
                ie.AddToUsed(id);

                ie.ReceiveElements(GetActiveElements());
                ReceiveElements(ie.GetActiveElements());
                ie.ActivateElement(true);
                ActivateElement(true);
            }
        }
        lastCollided = t;
    }

    private void OnEnable()
    {
        running = true;
        StartCoroutine(EOFCleanup());
    }

    private void OnDisable()
    {
        flag = false;
        b_activatedThisFrame = false;
        b_doThunder = true;
        b_doBoom = true;
        running = false;
        doneInit = false;

        //bA_statuses = new bool[7];
        //eL_activeElements = new List<Element>();

        StopAllCoroutines();
        //more only once per frame stuff
    }

    private void UnsetActivated()
    {
        b_activatedThisFrame = false;
        flag = false;
    }

    public void AddToUsed(int _id)
    {
        usedIDs.Add(_id);
    }

    public int ID()
    {
        if (id == 0)
            if (ElementManager.x != null)
                id = ElementManager.x.GetID();
        return id;
    }

    private void InitInteractions()
    {
        interactions = new ElementInteraction[,] {
            //Goo               Hydro               Tasty               Thunder             Boom                Fire                Lava
            {NullInteraction,   GooHydro,           NullInteraction,    GooThunder,         NullInteraction,    GooFire,            NullInteraction},   //Goo
            {GooHydro,          NullInteraction,    HydroTasty,         HydroThunder,       HydroBoom,          HydroFire,          NullInteraction},   //Hydro
            {NullInteraction,   HydroTasty,         NullInteraction,    NullInteraction,    NullInteraction,    TastyFire,          NullInteraction},   //Tasty
            {GooThunder,        HydroThunder,       NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction},   //Thunder
            {NullInteraction,   HydroBoom,          NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction},   //Boom
            {GooFire,           HydroFire,          TastyFire,          NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction},   //Fire
            {NullInteraction,   HydroLava,          NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction}    //Lava
        };

        activations = new ElementActivation[7] { GooActivate, HydroActivate, TastyActivate, ThunderActivate, BoomActivate, FireActivate, LavaActivate };
    }

    public void Init(params Element[] _startingElements)
    {
        doneInit = true;
        InitInteractions();
        eL_activeElements = new List<Element>();
        for (int i = 0; i < _startingElements.Length; i++)
        {
            Init(_startingElements[i]);
        }
        activated = null;
        InitialiseActivations();
    }
    private void Init(Element _startingElement)
    {
        if (!eL_activeElements.Contains(_startingElement))
            eL_activeElements.Add(_startingElement);
    }

    private IEnumerator EOFCleanup()
    {
        while (running)
        {
            yield return new WaitForEndOfFrame();

            CheckStatuses();
            CalculateDamages();
            SetStatusVisuals();

            usedIDs.Clear();
            yield return new WaitForSeconds(0.1f);
        }

    }

    private void CheckStatuses()
    {
        float t = Time.realtimeSinceStartup;
        for (int i = 0; i < starteds.Length - 1; i++)
        {
            bA_statuses[i] = t - starteds[i] < ElementManager.x?.durations[i];
        }
    }

    private void CalculateDamages()
    {
        float t = Time.realtimeSinceStartup;
        if (bA_statuses[(int)Element.Fire])
        {
            if (t - lastTook[5] > ElementManager.x.fireInterval)
            {
                TakeFireDamage();
                lastTook[5] = t;
            }
        }
    }

    private void SetStatusVisuals()
    {
        for (int i = 0; i < bA_statuses.Length; i++)
        {
            if (bA_statuses[i])
            {
                if (goA_effects[i] == null)
                {
                    goA_effects[i] = PoolManager.x.SpawnObject(ElementManager.x.effects[i], transform);
                    goA_effects[i].transform.localPosition = Vector3.zero;
                    ParticleSystem ps = goA_effects[i].GetComponent<ParticleSystem>();
                    ParticleSystem.ShapeModule sh = ps.shape;
                    sh.mesh = mesh;
                }
            }
            else
            {
                if (goA_effects[i] != null)
                {
                    goA_effects[i].GetComponent<IPoolable>()?.Die();
                    goA_effects[i] = null;
                }

            }
        }
    }

    private void InitialiseActivations() //add the intial activation stuff we should have
    {
        activated += ActivatedThisFrame;
        for (int i = 0; i < eL_activeElements.Count; i++)
        {
            activated += activations[(int)eL_activeElements[i]];
        }
    }

    private void ActivatedThisFrame()
    {
        //once per frame stufffff
        if (!b_activatedThisFrame && flag)
        {
            b_activatedThisFrame = true;
            Invoke(nameof(UnsetActivated), 0.1f);
        }
        else
            flag = true;

    }

    public void ReceiveElements(List<Element> _recieved)
    {
        //get affected by elements and carry out interactions
        int size = eL_activeElements.Count;
        for (int i = 0; i < _recieved.Count; i++)
        {
            if (eL_elementImmunities.Contains(_recieved[i]))
                continue;

            //Debug.Log($"I <b><color=red> {hittableObject?.name} </color></b> of ID: <b><color=red> {id} </color></b> have received: <b><color=red> {_recieved[i]} </color></b> ");

            ApplyStatus(_recieved[i]);
            ReceiveElements(_recieved[0]);
        }
    }
    public void ReceiveElements(Element _recieved)
    {
        //get affected by elements and carry out interactions
        int size = eL_activeElements.Count;
        for (int i = 0; i < size; i++)
        {
            interactions[(int)_recieved, (int)eL_activeElements[i]]();
        }
    }

    public void SetStatusEffect(Element _status, bool _val)
    {

        bA_statuses[(int)_status] = _val;
        if (_val)
            starteds[(int)_status] = Time.realtimeSinceStartup;
        return;

        if (_val)
        {
            goA_effects[(int)_status] = PoolManager.x.SpawnObject(ElementManager.x.effects[(int)_status], transform);
            goA_effects[(int)_status].transform.localPosition = Vector3.zero;
            ParticleSystem ps = goA_effects[(int)_status].GetComponent<ParticleSystem>();
            ParticleSystem.ShapeModule sh = ps.shape;

            sh.mesh = mesh;

        }
        else
        {
            goA_effects[(int)_status].GetComponent<IPoolable>().Die();
            goA_effects[(int)_status] = null;
        }
    }
    public void SetStatusEffect(Element _status, bool _val, float _time)
    {
        if (!gameObject || !gameObject.activeInHierarchy)
            return;
        SetStatusEffect(_status, _val);
        if (!gameObject || !gameObject.activeInHierarchy)
            return;
        StartCoroutine(TimedSetStatus(_status, !_val, _time));
    }
    private IEnumerator TimedSetStatus(Element _status, bool _val, float _time)
    {
        yield return new WaitForSeconds(_time);
        SetStatusEffect(_status, _val);
    }

    private void ApplyStatus(Element _el)
    {
        switch (_el)
        {
            case Element.Goo:
                if (bA_statuses[(int)_el])
                    return;
                SetStatusEffect(_el, true, ElementManager.x.gooDuration);
                AddRemoveElement(_el, true, ElementManager.x.gooDuration);
                break;
            case Element.Hydro:
                SetStatusEffect(_el, true, ElementManager.x.hydroDuration);
                AddRemoveElement(_el, true, ElementManager.x.hydroDuration);
                break;
            case Element.Tasty:
                break;
            case Element.Thunder:
                break;
            case Element.Boom:
                break;
            case Element.Fire:
                if (bA_statuses[(int)_el])
                    return;
                AddRemoveElement(_el, true, ElementManager.x.fireDuration);
                SetStatusEffect(_el, true, ElementManager.x.fireDuration);
                break;
            case Element.Lava:
                break;
            default:
                break;
        }
    }

    public void ActivateElement(bool activatesThunder)
    {
        b_doThunder = activatesThunder;
        activated?.Invoke();
    }
    public void AddRemoveElement(Element _elem, bool add)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(AddRemoveAtEOF(_elem, add));
    }
    public void AddRemoveElement(Element _elem, bool add, float _duration)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(AddRemoveAtEOF(_elem, add));
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayAddRemoveElement(_elem, !add, _duration));
    }

    IEnumerator DelayAddRemoveElement(Element _elem, bool add, float _duration)
    {
        yield return new WaitForSeconds(_duration);
        AddRemoveAtEOF(_elem, add);
    }

    IEnumerator AddRemoveAtEOF(Element _elem, bool add)
    {
        yield return null;
        if (add)
        {
            if (!eL_activeElements.Contains(_elem))
                eL_activeElements.Add(_elem);
            activated -= activations[(int)_elem];
            activated += activations[(int)_elem];
        }
        else
        {
            if (eL_activeElements.Contains(_elem))
                eL_activeElements.Remove(_elem);
            activated -= activations[(int)_elem];
        }
    }

    private void SetLineRendererPos(Vector3[] positions)
    {
        GameObject LR = PoolManager.x.SpawnObject(ElementManager.x.go_lrObject, transform.position);
        lrend = LR.GetComponent<LineRenderer>();
        if (!lrend)
            return;
        lrend.positionCount = (positions.Length * 2) + 1;
        int p = 1;
        lrend.SetPosition(0, transform.position);
        for (int i = 0; i < positions.Length; i++)
        {
            lrend.SetPosition(p, positions[i]);
            lrend.SetPosition(p + 1, transform.position);
            p += 2;
        }
        if (!gameObject)
            StartCoroutine(ResetLineRenderer(lrend, ElementManager.x.shockDelay * 0.5f));
    }

    IEnumerator ResetLineRenderer(LineRenderer lr, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        lr.SetPositions(new Vector3[0]);

    }

    private void TakeFireDamage()
    {
        ourHitable?.TakeDamage(ElementManager.x.fireDamage, true);
    }

    IEnumerator Explode(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        IHitable iH;
        IElementable iE;
        Collider[] hits = Physics.OverlapSphere(transform.position, ElementManager.x.boomRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            iH = hits[i].GetComponent<IHitable>();
            iE = hits[i].GetComponent<IElementable>();
            if (iH != null)
            {
                iE?.ReceiveElements(Element.Boom);
                iE?.AddRemoveElement(Element.Boom, true);
                iH.TakeDamage(ElementManager.x.boomDamage, true);
            }
        }
        ourHitable?.Die();
    }

    public void ResetElements()
    {
        eL_activeElements = new List<Element>();
        b_doThunder = true;
        b_doBoom = true;
        b_activatedThisFrame = false;

        Init(InitElements.ToArray());

    }

    public bool GetShouldDie()
    {
        return b_shouldDie;
    }

    #region ElementInteractions

    private void GooHydro()
    {
        AddRemoveElement(Element.Goo, false);
        AddRemoveElement(Element.Hydro, false);
    }

    private void GooThunder()
    {
        b_doThunder = false;
        SetStatusEffect(Element.Thunder, false);
    }

    private void GooFire()
    {
        SetStatusEffect(Element.Goo, true, ElementManager.x.gooDuration);
        SetStatusEffect(Element.Fire, true, ElementManager.x.fireDuration);
        FireActivate();
    }

    private void HydroBoom()
    {
        SetStatusEffect(Element.Boom, false);
        if (ElementManager.x != null)
            SetStatusEffect(Element.Hydro, true, ElementManager.x.hydroDuration);
    }

    private void HydroTasty()
    {
        //do a growing thing
    }

    private void HydroThunder()
    {
        if (!b_doThunder)
            return;
        b_doThunder = false;
        AddRemoveElement(Element.Thunder, true);
        ThunderActivate();
    }

    private void HydroFire()
    {
        SetStatusEffect(Element.Fire, false);
        SetStatusEffect(Element.Hydro, false);
    }

    private void HydroLava()
    {
        //do platform things
        Destroy(Instantiate(ElementManager.x.lavaPlatform, transform.position, Quaternion.identity), 5);
        AddRemoveElement(Element.Hydro, false);
        ourHitable?.TakeDamage(1, true);
    }

    private void TastyFire()
    {
        //do eating things
    }

    private void NullInteraction()
    {
        return;
    }

    #endregion

    #region Activations

    private void GooActivate()
    {
        if (b_activatedThisFrame)
            return;
        SetStatusEffect(Element.Goo, true, ElementManager.x.gooDuration);
        AddRemoveElement(Element.Goo, true, ElementManager.x.gooDuration);
    }

    private void HydroActivate()
    {
        if (b_activatedThisFrame)
            return;

        SetStatusEffect(Element.Hydro, true, ElementManager.x.hydroDuration);
        AddRemoveElement(Element.Hydro, true, ElementManager.x.hydroDuration);

    }

    private void TastyActivate()
    {
        if (b_activatedThisFrame)
            return;

    }

    private void ThunderActivate()
    {
        if (b_activatedThisFrame)
            return;
        if (!b_doThunder)
        {
            return;
        }

        SetStatusEffect(Element.Thunder, true, ElementManager.x.noShockBackDuration); //No tag backs

        //Waddid i hit
        Collider[] hits = Physics.OverlapSphere(transform.position, ElementManager.x.shockRange);
        List<Vector3> verts = new List<Vector3>();
        IElementable ie;
        IHitable ih;
        int count = 0;

        //shock it
        for (int i = 0; i < hits.Length; i++)
        {
            ie = hits[i].GetComponent<IElementable>();
            ih = hits[i].GetComponent<IHitable>();
            if (ih != null)
            {
                ie?.SetStatusEffect(Element.Thunder, true, ElementManager.x.noShockBackDuration);
                verts.Add(hits[i].transform.position); //remember the position for line renderer stuff
                ie?.ReceiveElements(Element.Thunder); //tell the target we're shocking it
                ih.TakeDamage(ElementManager.x.shockDamage, true, ElementManager.x.shockDelay); //if it can be damaged then dewit
                count += 1; //<<to limit the number of objects we can shock
            }
            if (count >= ElementManager.x.maximumShockTargets) //Stop if we're at max
                break;
        }
        SetLineRendererPos(verts.ToArray()); //Show the shock lines
        AddRemoveElement(Element.Thunder, false);
        AddRemoveElement(Element.Hydro, false);
    }

    private void BoomActivate()
    {
        b_shouldDie = false;
        if (b_activatedThisFrame || !b_doBoom)
            return;
        b_doBoom = false;
        if (gameObject.activeSelf)
        {
            SetStatusEffect(Element.Boom, true, ElementManager.x.boomFuse);
            StartCoroutine(Explode(ElementManager.x.boomFuse));
        }
        AddRemoveElement(Element.Boom, false);
    }

    private void FireActivate()
    {
        if (b_activatedThisFrame)
            return;

        SetStatusEffect(Element.Fire, true, ElementManager.x.fireDuration * (bA_statuses[(int)Element.Goo] ? ElementManager.x.gooDurationMultiplier : 1));
        AddRemoveElement(Element.Fire, true, ElementManager.x.fireDuration * (bA_statuses[(int)Element.Goo] ? ElementManager.x.gooDurationMultiplier : 1));
        //Debug.Log("Doing fire damage to " + gameObject.name, gameObject);


    }

    private void LavaActivate()
    {
        if (b_activatedThisFrame)
            return;
        //FireActivate();
    }

    #endregion

    public List<Element> GetActiveElements()
    {
        return eL_activeElements;
    }

    public bool[] GetStatuses()
    {
        return bA_statuses;
    }

}

public enum Element
{
    Goo = 0,
    Hydro = 1,
    Tasty = 2,
    Thunder = 3,
    Boom = 4,
    Fire = 5,
    Lava = 6
}

public interface IElementable
{
    void ReceiveElements(List<Element> _recieved);
    void ReceiveElements(Element _recieved);
    void SetStatusEffect(Element _status, bool _val);
    void SetStatusEffect(Element _status, bool _val, float _time);
    void ActivateElement(bool activaesThunder);
    void AddRemoveElement(Element _elem, bool add);
    List<Element> GetActiveElements();
    bool[] GetStatuses();
    void ResetElements();
    void AddToUsed(int _id);
    int ID();
}