using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementManager : MonoBehaviour
{

    public static ElementManager x;

    public GameObject go_lrObject;
    public GameObject[] effects;
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
    public float boomFuse;
    public int boomDamage;
    [Header("Fire")]
    public float fireDuration;
    public int fireDamage;
    public float fireInterval;
    [Header("Lava")]
    public int lavaDamage;
    public GameObject lavaPlatform;

    private List<ElementalObject> activs = new List<ElementalObject>();
    public float[] durations = new float[7];

    private int cID = 0;

    private bool playing = true;

    private void Awake()
    {
        if (x != null)
        {
            if (x != this)
                Destroy(gameObject);
        }
        else
            x = this;

        durations = new float[7] { gooDuration, hydroDuration, 0, noShockBackDuration, boomFuse, fireDuration, Mathf.Infinity };
    }

    public int GetID()
    {
        cID += 1;
        return cID;
    }

}
