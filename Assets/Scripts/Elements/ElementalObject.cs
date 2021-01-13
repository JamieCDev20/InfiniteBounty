using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalObject : MonoBehaviour
{

    [SerializeField] private Element[] eA_activeElements;

    public void RecieveElements(Element[] _recieved)
    {

    }

}

public enum Element
{
    fire,
    thunder,
    goo,
    hydro,
    tasty,
    boom,
    lava
}