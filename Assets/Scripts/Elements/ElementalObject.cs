using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElementalObject : MonoBehaviour, IElementable
{

    [SerializeField] private List<Element> eA_activeElements = new List<Element>(); //The elements on this object
    [SerializeField] private GameObject go_lrObject; //the line renderer for shocking
    [SerializeField] private Mesh mesh;

    //Goo  Hydro   Tasty Thunder Boom    Fire    Lava
    private bool[] bA_statuses = new bool[7] { false, false, false, false, false, false, false }; //status effects on the object

    private GameObject[] goA_effects = new GameObject[7];

    private delegate void ElementInteraction(); //Delegates for Elemental Interactions
    private delegate void ElementActivation(); //Delegates for Elemental Activations

    private ElementActivation activated; // The delegate called when the object get's activated

    private ElementInteraction[,] interactions; //The big ass 2D array of interactions
    private ElementActivation[] activations; //The array of activation functions

    private bool b_doThunder = true; // bool to stop thunder infinitely repeating
    private LineRenderer lrend;
    private PoolableObject pO; //To store the line renderer object
    private bool b_activatedThisFrame = false; //only activate once per frame <<Not sure if i actually need this anymore.. but better safe than sorry
    private bool flag; // ^^
    private ElementManager em;
    private IHitable ourHitable;

    private void Start()
    {
        em = ElementManager.x;
        pO = GetComponent<PoolableObject>();
        ourHitable = GetComponent<IHitable>();
        if(mesh == null)
            mesh = GetComponentInChildren<MeshFilter>().mesh;

        interactions = new ElementInteraction[,] {
            //Goo               Hydro               Tasty               Thunder             Boom                Fire                Lava
            {NullInteraction,   GooHydro,           NullInteraction,    GooThunder,         NullInteraction,    GooFire,            NullInteraction},   //Goo
            {GooHydro,          NullInteraction,    HydroTasty,         HydroThunder,       NullInteraction,    HydroFire,          HydroLava},         //Hydro
            {NullInteraction,   HydroTasty,         NullInteraction,    NullInteraction,    NullInteraction,    TastyFire,          NullInteraction},   //Tasty
            {GooThunder,        HydroThunder,       NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction},   //Thunder
            {NullInteraction,   NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction},   //Boom
            {GooFire,           HydroFire,          TastyFire,          NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction},   //Fire
            {NullInteraction,   HydroLava,          NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction,    NullInteraction}    //Lava
        };

        activations = new ElementActivation[7] { GooActivate, HydroActivate, TastyActivate, ThunderActivate, BoomActivate, FireActivate, LavaActivate };

        InitialiseActivations();

    }

    public void Init(Element[] _startingElements)
    {
        for (int i = 0; i < _startingElements.Length; i++)
        {
            Init(_startingElements[i]);
        }
        activated = null;
        InitialiseActivations();
    }
    private void Init(Element _startingElement)
    {
        eA_activeElements.Add(_startingElement);
    }

    private void OnDisable()
    {
        flag = false;
        b_activatedThisFrame = false;
        StopAllCoroutines();
        //more only once per frame stuff
    }

    private void InitialiseActivations() //add the intial activation stuff we should have
    {
        activated += ActivatedThisFrame;
        for (int i = 0; i < eA_activeElements.Count; i++)
        {
            activated += activations[(int)eA_activeElements[i]];
        }
    }

    private void ActivatedThisFrame()
    {
        //once per frame stufffff
        if (!b_activatedThisFrame && flag)
            b_activatedThisFrame = true;
        else
            flag = true;
    }

    public void RecieveElements(List<Element> _recieved)
    {
        //get affected by elements and carry out interactions
        int size = eA_activeElements.Count;
        for (int i = 0; i < _recieved.Count; i++)
        {
            RecieveElements(_recieved[0]);
        }
    }
    public void RecieveElements(Element _recieved)
    {
        //get affected by elements and carry out interactions
        int size = eA_activeElements.Count;
        for (int i = 0; i < size; i++)
        {
            interactions[(int)_recieved, (int)eA_activeElements[i]]();
        }
    }

    public void SetStatusEffect(Element _status, bool _val)
    {
        if (bA_statuses[(int)_status] == _val)
            return;
        if (em == null)
            em = ElementManager.x;
        bA_statuses[(int)_status] = _val;
        if (_val)
        {
            goA_effects[(int)_status] = PoolManager.x.SpawnObject(em.effects[(int)_status], transform);
            goA_effects[(int)_status].transform.localPosition = Vector3.zero;
            ParticleSystem ps = goA_effects[(int)_status].GetComponent<ParticleSystem>();
            ParticleSystem.ShapeModule sh = ps.shape;
            sh.shapeType = ParticleSystemShapeType.Mesh;
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
        if (!gameObject)
            return;
        SetStatusEffect(_status, _val);
        if (!gameObject || !gameObject.activeSelf)
            return;
        StartCoroutine(TimedSetStatus(_status, !_val, _time));
    }
    private IEnumerator TimedSetStatus(Element _status, bool _val, float _time)
    {
        yield return new WaitForSeconds(_time);
        SetStatusEffect(_status, _val);
    }

    public void ActivateElement(bool activatesThunder)
    {
        b_doThunder = activatesThunder;
        activated?.Invoke();
    }

    public void AddRemoveElement(Element _elem, bool add)
    {
        StartCoroutine(AddRemoveAtEOF(_elem, add));
    }
    public void AddRemoveElement(Element _elem, bool add, float _duration)
    {
        StartCoroutine(AddRemoveAtEOF(_elem, add));
        StartCoroutine(DelayAddRemoveElement(_elem, !add, _duration));
    }

    IEnumerator DelayAddRemoveElement(Element _elem, bool add, float _duration)
    {
        yield return new WaitForSeconds(_duration);
        AddRemoveAtEOF(_elem, add);
    }

    IEnumerator AddRemoveAtEOF(Element _elem, bool add)
    {
        yield return new WaitForEndOfFrame();
        if (add)
        {
            eA_activeElements.Add(_elem);
            activated += activations[(int)_elem];
        }
        else
        {
            eA_activeElements.Remove(_elem);
            activated -= activations[(int)_elem];
        }

    }

    private void SetLineRendererPos(Vector3[] positions)
    {
        GameObject LR = PoolManager.x.SpawnObject(go_lrObject, transform.position);
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
            StartCoroutine(ResetLineRenderer(lrend, em.shockDelay * 0.5f));

    }

    IEnumerator ResetLineRenderer(LineRenderer lr, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        lr.SetPositions(new Vector3[0]);

    }

    IEnumerator FireDamage(float _duration, int _damage)
    {
        ourHitable.TakeDamage(_damage, true);
        float t = 0;
        while (t < _duration)
        {
            t += em.fireInterval;
            yield return new WaitForSeconds(em.fireInterval);
            ourHitable.TakeDamage(_damage, true);

        }
        SetStatusEffect(Element.fire, false);
        AddRemoveElement(Element.fire, false);
        SetStatusEffect(Element.goo, false);
        AddRemoveElement(Element.goo, false);
    }

    #region ElementInteractions

    private void GooHydro()
    {
        AddRemoveElement(Element.goo, false);
        AddRemoveElement(Element.hydro, false);
    }

    private void GooThunder()
    {
        b_doThunder = false;
        SetStatusEffect(Element.thunder, false);
    }

    private void GooFire()
    {
        SetStatusEffect(Element.goo, true, em.gooDuration);
        SetStatusEffect(Element.fire, true, em.fireDuration);
        FireActivate();
    }

    private void HydroTasty()
    {
        //do a growing thing
    }

    private void HydroThunder()
    {
        AddRemoveElement(Element.thunder, true);
    }

    private void HydroFire()
    {
        SetStatusEffect(Element.fire, false);
    }

    private void HydroLava()
    {
        //do platform things
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
        SetStatusEffect(Element.goo, true, em.gooDuration);
        AddRemoveElement(Element.goo, true);
    }

    private void HydroActivate()
    {
        if (b_activatedThisFrame)
            return;
        AddRemoveElement(Element.hydro, true);
        SetStatusEffect(Element.hydro, true);
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
            b_doThunder = true;
            return;
        }

        SetStatusEffect(Element.thunder, true, em.noShockBackDuration); //No tag backs

        //Waddid i hit
        Collider[] hits = Physics.OverlapSphere(transform.position, em.shockRange);
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
                ie?.SetStatusEffect(Element.thunder, true, em.noShockBackDuration);
                verts.Add(hits[i].transform.position); //remember the position for line renderer stuff
                ie?.RecieveElements(Element.thunder); //tell the target we're shocking it
                ih.TakeDamage(em.shockDamage, true, em.shockDelay); //if it can be damaged then dewit
                count += 1; //<<to limit the number of objects we can shock
            }
            if (count >= em.maximumShockTargets) //Stop if we're at max
                break;
        }
        SetLineRendererPos(verts.ToArray()); //Show the shock lines
    }

    private void BoomActivate()
    {
        if (b_activatedThisFrame)
            return;
        Collider[] hits = Physics.OverlapSphere(transform.position, em.boomRadius);
        IHitable iH;
        IElementable iE;
        for (int i = 0; i < hits.Length; i++)
        {
            iH = hits[i].GetComponent<IHitable>();
            iE = hits[i].GetComponent<IElementable>();
            if (iH != null)
            {
                iE?.SetStatusEffect(Element.boom, true, em.boomFuse);
                iE?.RecieveElements(Element.boom);
                iH.TakeDamage(em.boomDamage, true, em.boomFuse);
            }
        }
    }

    private void FireActivate()
    {
        if (b_activatedThisFrame)
            return;

        SetStatusEffect(Element.fire, true);
        AddRemoveElement(Element.fire, true);
        StopCoroutine("FireDamage");
        StartCoroutine(FireDamage(em.fireDuration * (bA_statuses[(int)Element.goo] ? em.gooDurationMultiplier : 1), em.fireDamage * (bA_statuses[(int)Element.goo] ? em.gooDamageMultiplier : 1)));

    }

    private void LavaActivate()
    {
        if (b_activatedThisFrame)
            return;
        FireActivate();
    }

    #endregion

}

public enum Element
{
    goo = 0,
    hydro = 1,
    tasty = 2,
    thunder = 3,
    boom = 4,
    fire = 5,
    lava = 6
}

public interface IElementable
{
    void RecieveElements(List<Element> _recieved);
    void RecieveElements(Element _recieved);
    void SetStatusEffect(Element _status, bool _val);
    void SetStatusEffect(Element _status, bool _val, float _time);
    void ActivateElement(bool activaesThunder);
    void AddRemoveElement(Element _elem, bool add);
}