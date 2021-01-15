using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementManager : MonoBehaviour
{

    public static ElementManager x;

    [Header("Goo")]
    public float gooDuration;
    public float gooDurationMultiplier;
    public int gooDamageMultiplier;
    [Header("Hydro")]
    public float hydroDuration;
    [Header("Tasty")]
    public int growMultiplier;
    [Header("Thunder")]
    public int shockDamage;
    public float shockDelay;
    public float shockRange;
    public float noShockBackDuration;
    public int maximumShockTargets;
    [Header("Boom")]
    public float boomRadius;
    public int boomDamage;
    [Header("Fire")]
    public float fireDuration;
    public int fireDamage;
    public float fireInterval;
    [Header("Lava")]
    public int lavaDamage;

    private void Awake()
    {
        x = this;
    }

}
