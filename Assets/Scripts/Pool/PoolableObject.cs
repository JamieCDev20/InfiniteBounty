using System.Collections;
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
        CancelInvoke(nameof(Die));
        if (PoolManager.x != null) PoolManager.x.ReturnObjectToPool(gameObject);
    }

    private void OnEnable()
    {
        if (f_lifetime > 0)
        {
            Invoke(nameof(Die), f_lifetime);
        }
    }

    public GameObject GetGameObject()
    {
        try
        {
            return gameObject;
        }
        catch
        {
            return null;
        }
    }

    public bool IsNetworkedObject()
    {
        return b_isNetworkedObject;
    }

    public string ResourcePath()
    {
        return s_resourcePath;
    }
    
    public void DelayedDie(float f_timer)
    {
        Invoke(nameof(Die), f_timer);
    }
}
