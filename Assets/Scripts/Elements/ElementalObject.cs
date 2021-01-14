using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElementalObject : MonoBehaviour, IElementable
{

    [SerializeField] private List<Element> eA_activeElements = new List<Element>();
    [SerializeField] private GameObject go_lrObject;

    //Goo     Hydro   Tasty   Thunder Boom    Fire    Lava
    private bool[] bA_statuses = new bool[7] { false, false, false, false, false, false, false };

    private delegate void ElementInteraction();
    private delegate void ElementActivation();

    private ElementActivation activated;

    private ElementInteraction[,] interactions;
    private ElementActivation[] activations;

    private bool b_doThunder = true;
    private LineRenderer lrend;
    private PoolableObject pO;

    private void Start()
    {
        pO = GetComponent<PoolableObject>();
        interactions = new ElementInteraction[,] {
            //Goo               Hydro               Tasty               Thunder             Boom                Fire                Lava
            {NullInteraction,   GooHydro,           NullInteraction,    GooThunder,         NullInteraction,    GooFire,            NullInteraction},   //Goo
            {GooHydro,          NullInteraction,    HydroTasty,         HydroThunder,       NullInteraction,    HydroFire,          HydroLava},         //Hydro
            {NullInteraction,   HydroTasty,         NullInteraction,    NullInteraction,    NullInteraction,    TastyFire,          NullInteraction},   //Tasty
            {GooThunder,        HydroThunder,       NullInteraction,    NullInteraction,     ThunderBoom,        NullInteraction,    NullInteraction},   //Thunder
            {NullInteraction,   NullInteraction,    NullInteraction,    ThunderBoom,        BoomBoom,           BoomFire,           BoomLava },         //Boom
            {GooFire,           HydroFire,          TastyFire,          NullInteraction,    BoomFire,           NullInteraction,    NullInteraction },  //Fire
            {NullInteraction,   HydroLava,          NullInteraction,    NullInteraction,    BoomLava,           NullInteraction,    NullInteraction}    //Lava
        };

        activations = new ElementActivation[7] { GooActivate, HydroActivate, TastyActivate, ThunderActivate, BoomActivate, FireActivate, LavaActivate };

        InitialiseActivations();

    }

    private void InitialiseActivations()
    {
        for (int i = 0; i < eA_activeElements.Count; i++)
        {
            activated += activations[(int)eA_activeElements[i]];
        }
    }

    public void RecieveElements(List<Element> _recieved)
    {
        int size = eA_activeElements.Count;
        for (int i = 0; i < _recieved.Count; i++)
        {
            for (int j = 0; j < size; j++)
            {
                interactions[(int)_recieved[i], (int)eA_activeElements[j]]();
            }
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
        ThunderActivate();
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

    }

    private void HydroActivate()
    {

    }

    private void TastyActivate()
    {

    }

    private void ThunderActivate()
    {
        if (!b_doThunder)
        {
            b_doThunder = true;
            return;
        }
        SetStatusEffect(Element.thunder, true, 2);
        Collider[] hits = Physics.OverlapSphere(transform.position, 10);
        Vector3[] verts = new Vector3[hits.Length];
        IElementable ie;
        int count = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            ie = hits[i].GetComponent<IElementable>();
            if (ie != null)
            {
                hits[i].GetComponent<IHitable>()?.TakeDamage(5, true, 0.3f);
                if(hits[i].transform.position == Vector3.zero)
                    Debug.Log(hits[i].gameObject.name, hits[i].gameObject);
                count += 1;
                verts[i] = hits[i].transform.position;
                ie?.RecieveElements(eA_activeElements);
            }
            if (count >= 3)
                break;
        }
        SetLineRendererPos(verts);
    }

    private void BoomActivate()
    {

    }

    private void FireActivate()
    {

    }

    private void LavaActivate()
    {

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
    void SetStatusEffect(Element _status, bool _val);
    void ActivateElement(bool activaesThunder);
    void AddRemoveElement(Element _elem, bool add);
}