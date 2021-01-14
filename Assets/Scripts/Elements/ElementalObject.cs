using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElementalObject : MonoBehaviour, IElementable
{

    [SerializeField] private List<Element> eA_activeElements = new List<Element>(); //The elements on this object
    [SerializeField] private GameObject go_lrObject; //the line renderer for shocking

    //Goo  Hydro   Tasty Thunder Boom    Fire    Lava
    private bool[] bA_statuses = new bool[7] { false, false, false, false, false, false, false }; //status effects on the object

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

    private void Start()
    {
        em = ElementManager.x;
        pO = GetComponent<PoolableObject>();
        interactions = new ElementInteraction[,] {
            //Goo               Hydro               Tasty               Thunder             Boom                Fire                Lava
            {NullInteraction,   GooHydro,           NullInteraction,    GooThunder,         NullInteraction,    GooFire,            NullInteraction},   //Goo
            {GooHydro,          NullInteraction,    HydroTasty,         HydroThunder,       NullInteraction,    HydroFire,          HydroLava},         //Hydro
            {NullInteraction,   HydroTasty,         NullInteraction,    NullInteraction,    NullInteraction,    TastyFire,          NullInteraction},   //Tasty
            {GooThunder,        HydroThunder,       NullInteraction,    NullInteraction,    ThunderBoom,        NullInteraction,    NullInteraction},   //Thunder
            {NullInteraction,   NullInteraction,    NullInteraction,    ThunderBoom,        BoomBoom,           BoomFire,           BoomLava },         //Boom
            {GooFire,           HydroFire,          TastyFire,          NullInteraction,    BoomFire,           NullInteraction,    NullInteraction },  //Fire
            {NullInteraction,   HydroLava,          NullInteraction,    NullInteraction,    BoomLava,           NullInteraction,    NullInteraction}    //Lava
        };

        activations = new ElementActivation[7] { GooActivate, HydroActivate, TastyActivate, ThunderActivate, BoomActivate, FireActivate, LavaActivate };

        InitialiseActivations();

    }

    private void OnDisable()
    {
        flag = false;
        b_activatedThisFrame = false;
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
            for (int j = 0; j < size; j++)
            {
                interactions[(int)_recieved[i], (int)eA_activeElements[j]]();
            }
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
        bA_statuses[(int)_status] = _val;
    }
    public void SetStatusEffect(Element _status, bool _val, float _time)
    {
        bA_statuses[(int)_status] = _val;
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

        StartCoroutine(ResetLineRenderer(lrend, 0.3f));

    }

    IEnumerator ResetLineRenderer(LineRenderer lr, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        lr.SetPositions(new Vector3[0]);

    }

    #region ElementInteractions

    private void GooHydro()
    {

    }

    private void GooThunder()
    {

    }

    private void GooFire()
    {

    }

    private void HydroTasty()
    {

    }

    private void HydroThunder()
    {
        AddRemoveElement(Element.thunder, true);
    }

    private void HydroFire()
    {

    }

    private void HydroLava()
    {

    }

    private void TastyFire()
    {

    }

    private void ThunderBoom()
    {

    }

    private void BoomBoom()
    {

    }

    private void BoomFire()
    {

    }

    private void BoomLava()
    {

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
    }

    private void HydroActivate()
    {
        if (b_activatedThisFrame)
            return;

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
        Vector3[] verts = new Vector3[hits.Length];
        IElementable ie;
        int count = 0;

        //shock it
        for (int i = 0; i < hits.Length; i++)
        {
            ie = hits[i].GetComponent<IElementable>();
            if (ie != null)
            {
                ie?.RecieveElements(Element.thunder); //tell the target we're shocking it
                hits[i].GetComponent<IHitable>()?.TakeDamage(em.shockDamage, true, em.shockDelay); //if it can be damaged then dewit
                count += 1; //<<to limit the number of objects we can shock
                verts[i] = hits[i].transform.position; //remember the position for line renderer stuff
            }
            if (count >= em.maximumShockTargets) //Stop if we're at max
                break;
        }
        SetLineRendererPos(verts); //Show the shock lines
    }

    private void BoomActivate()
    {
        if (b_activatedThisFrame)
            return;

    }

    private void FireActivate()
    {
        if (b_activatedThisFrame)
            return;

    }

    private void LavaActivate()
    {
        if (b_activatedThisFrame)
            return;

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
    void ActivateElement(bool activaesThunder);
    void AddRemoveElement(Element _elem, bool add);
}