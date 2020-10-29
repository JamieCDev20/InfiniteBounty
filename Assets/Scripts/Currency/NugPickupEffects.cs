using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugPickupEffects : MonoBehaviour, IPoolable
{
    private bool b_inPool;
    private int i_poolIndex;
    public void Die()
    {
        if (PoolManager.x != null) PoolManager.x.ReturnInactiveToPool(gameObject, i_poolIndex);
        SetInPool(true);
    }

    public int GetIndex()
    {
        return i_poolIndex;
    }

    public bool GetInPool()
    {
        return b_inPool;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public void OnSpawn()
    {
        
    }

    public void SetIndex(int _i_index)
    {
        i_poolIndex = _i_index;
    }

    public void SetInPool(bool _b_delta)
    {
        b_inPool = _b_delta;
    }
}
