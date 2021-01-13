using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElementalObject : MonoBehaviour
{

    [SerializeField] private List<Element> eA_activeElements = new List<Element>();

    private bool[] bA_statuses = new bool[7] { false, false, false, false, false, false, false };

    private delegate void ElementInteraction();
    private delegate void ElementActivation();

    private ElementActivation activated;

    private ElementInteraction[,] interactions;
    private ElementActivation[] activations;

    private void Start()
    {

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

        activations = new ElementActivation[7] { GooActivate, HydroActivate, TastyActivate, ThunderActivate, BoomActivate, FireActivate, LavaActivate};

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

    public void Activate()
    {
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