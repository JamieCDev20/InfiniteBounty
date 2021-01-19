using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AugmentPhysicals
{
    public float f_trWidth;
    public float f_trLifetime;
    public Color[] A_trKeys;
    public GameObject go_projectile;

    public AugmentPhysicals(float _f_trWidth, float _f_trLifetime, Color[] _A_keys, GameObject _go_proj)
    {
        f_trWidth = _f_trWidth;
        f_trLifetime = _f_trLifetime;
        A_trKeys = _A_keys;
        go_projectile = _go_proj;
    }
}
