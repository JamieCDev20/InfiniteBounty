using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GOAP Action", menuName = "GOAP/New Action")]
public class Action : ScriptableObject
{

    [SerializeField] public SBDictionary preconditions = null;
    [Space]
    [SerializeField] public SBDictionary postconditions = null;

}
