using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeWhenExpsoedToFire : MonoBehaviour
{
    [SerializeField] private GameObject go_explosion;

    internal void Explode()
    {
        PoolManager.x.SpawnObject(go_explosion, transform.position, Quaternion.identity);
        GetComponent<NugGO>().Die();
    }


}
