﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour, IPoolable
{

    [SerializeField] private string s_resourcePath;
    [SerializeField] private bool b_isNetworkedObject;
    [Tooltip("0 for infinite")]
    [SerializeField] private float f_lifetime;

    public void Die()
    {
        Debug.Log("DYING");
        if (PoolManager.x != null) PoolManager.x.ReturnObjectToPool(gameObject);
    }

    private void OnEnable()
    {
        if(f_lifetime > 0)
        {
            Invoke("Die", f_lifetime);
        }
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
