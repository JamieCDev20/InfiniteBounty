using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NugType
{
    goo = 0,
    hydro = 1,
    tasty = 2,
    thunder = 3,
    boom = 4,
    magma = 5
}

[CreateAssetMenu(fileName = "Nug", menuName = "ScriptableObjects/Nug") ]
public class Nug : ScriptableObject
{
    public int i_worth;
    public NugType nt_type;
}
