using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentGo : MonoBehaviour
{
    [SerializeField] Augment a_aug;
    public Augment Aug { get { return a_aug; } set { a_aug = value; } }
}
