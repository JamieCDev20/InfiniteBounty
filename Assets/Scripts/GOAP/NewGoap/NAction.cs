using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NGoap Action", menuName = "New NGoap Action")]
public class NAction : ScriptableObject
{

    public new string name;

}

public enum ActionType
{
    move,
    attack,
    interact
}