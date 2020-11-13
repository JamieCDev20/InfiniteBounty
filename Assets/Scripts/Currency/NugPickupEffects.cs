using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugPickupEffects : MonoBehaviour, IPoolable
{
    public void Die()
    {
        if (PoolManager.x != null) PoolManager.x.ReturnObjectToPool(gameObject);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

}
