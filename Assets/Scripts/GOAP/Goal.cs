using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GOAP Goal", menuName = "New GOAP Goal")]
public class Goal : ScriptableObject
{

    [SerializeField] public SBDictionary goalConditions;

}
