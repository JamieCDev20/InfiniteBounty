﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentGo : MonoBehaviour
{
    [SerializeField] Augment a_aug;
    [SerializeField] Material mat_ref;
    public Augment Aug { get { return a_aug; } set { a_aug = value; } }
    public Material Mat { set { mat_ref = value; } }
}
