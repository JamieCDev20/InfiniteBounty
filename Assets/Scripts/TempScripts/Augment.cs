using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AugmentType
{
    Flame, Electric, Heavy, Size, Speed, Explosive, Gooey, Soaked
}

public class Augment : MonoBehaviour
{

    [Header("Augment Types")]
    [SerializeField] private AugmentType at_thisAugment;

    internal AugmentType GetAugment()
    {
        return at_thisAugment;
    }
}