using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Pool
{
    [SerializeField] private GameObject go_poolType;
    [SerializeField] private int i_initialSize;
    private HashSet<IPoolable> p_objects;
    private int i_aliveObjects;

    public Pool(int size, GameObject type)
    {
        go_poolType = type;
        i_initialSize = size;
        p_objects = new HashSet<IPoolable>();
        i_aliveObjects = size;
        InitializePool();
    }

    /// <summary>
    /// Set the objects into the pool
    /// </summary>
    /// <param name="_go_objectType">Type of object to pool</param>
    /// <param name="_p_pooledObjects">If the object is poolable</param>
    /// <param name="_i_size">The initial size of the pool</param>
    /// <param name="_t_spawnPoint">The pools transform</param>
    public void InitializePool()
    {
        p_objects = new HashSet<IPoolable>();
        i_aliveObjects = i_initialSize;
        Debug.Log("initial pool size: " + i_aliveObjects);
        for(int i = 0; i < i_initialSize; i++)
        {
            AddNewObject();
        }
    }

    public GameObject SpawnObject()
    {
        Debug.Log("number of alive objects: " + i_aliveObjects + " init size: " + i_initialSize);
        if (i_aliveObjects > 0)
        {
            GameObject returnable = GetFromPool();
            if (returnable != null)
            {
                i_aliveObjects--;
                Debug.Log(i_aliveObjects);
                returnable.transform.parent = null;
                return returnable;
            }
        }
        GameObject newGo = AddNewObject();
        newGo.SetActive(true);
        return newGo;
    }

    private GameObject GetFromPool()
    {
        foreach (IPoolable poo in p_objects)
        {
            if (!poo.GetGameObject().activeInHierarchy)
            {
                i_aliveObjects--;
                GameObject pooGo = poo.GetGameObject();
                pooGo.SetActive(true);
                pooGo.transform.parent = null;
                return pooGo;
            }
        }
        return null;
    }

    public void ReturnToPool(GameObject go_objectToPool)
    {
        go_objectToPool.transform.position = PoolManager.x.transform.position;
        go_objectToPool.transform.parent = PoolManager.x.transform;
        go_objectToPool.SetActive(false);
        i_aliveObjects++;
        Debug.Log(i_aliveObjects);
    }

    private GameObject AddNewObject()
    {
        if (p_objects == null)
            p_objects = new HashSet<IPoolable>();
        GameObject newGo = GameObject.Instantiate(go_poolType);
        newGo.name = newGo.name.Replace("(Clone)", "");
        newGo.transform.parent = PoolManager.x.transform;
        IPoolable poo = newGo.GetComponent<IPoolable>();
        if (poo != null)
            p_objects.Add(poo);
        else
            Debug.Log("Oops... 500!");
        newGo.SetActive(false);
        i_aliveObjects++;
        return newGo;
    }

}
