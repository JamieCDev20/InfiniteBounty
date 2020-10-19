using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GOAP Action", menuName = "New GOAP Action")]
public class Action : ScriptableObject
{

    [SerializeField] public SBDictionary preconditions = null;
    [SerializeField] public SBDictionary postconditions = null;

}
