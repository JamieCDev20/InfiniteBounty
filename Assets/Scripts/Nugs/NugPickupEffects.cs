using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugPickupEffects : MonoBehaviour, IPoolable
{

    [SerializeField] private bool b_isNetworkedObject = false;
    [SerializeField] private string s_resourcePath;

    public void Die()
    {
        if (PoolManager.x != null) PoolManager.x.ReturnObjectToPool(gameObject);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool IsNetworkedObject()
    {
        return b_isNetworkedObject;
    }

    public string ResourcePath()
    {
        return s_resourcePath;
    }

}
