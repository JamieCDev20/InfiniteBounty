using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour, IPoolable
{

    [SerializeField] private string s_resourcePath;
    [SerializeField] private bool b_isNetworkedObject;

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
