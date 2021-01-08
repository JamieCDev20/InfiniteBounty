using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NugType
{
    goo,
    hydro,
    tasty,
    thunder,
    boom,
    magma
}

[CreateAssetMenu(fileName = "Nug", menuName = "ScriptableObjects/Nug") ]
public class Nug : ScriptableObject
{
    public int i_worth;
    public NugType nt_type;
}
