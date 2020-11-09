using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Pool
{
    [SerializeField] private GameObject go_poolType;
    [SerializeField] private int i_initialSize;
    Transform t_spawnPoint;
    private HashSet<IPoolable> p_objects;
    private int i_aliveObjects;

    public Pool(int size, GameObject type)
    {
        go_poolType = type;
        i_initialSize = size;
        t_spawnPoint = PoolManager.x.transform;
        p_objects = new HashSet<IPoolable>();
        i_aliveObjects = 0;
        InitializePool(t_spawnPoint);
    }

    /// <summary>
    /// Set the objects into the pool
    /// </summary>
    /// <param name="_go_objectType">Type of object to pool</param>
    /// <param name="_p_pooledObjects">If the object is poolable</param>
    /// <param name="_i_size">The initial size of the pool</param>
    /// <param name="_t_spawnPoint">The pools transform</param>
    public void InitializePool(Transform _t_spawnPoint)
    {
        p_objects = new HashSet<IPoolable>();
        i_aliveObjects = i_initialSize;
        t_spawnPoint = _t_spawnPoint;
        for(int i = 0; i < i_initialSize; i++)
        {
            AddNewObject();
        }
    }

    public GameObject SpawnObject()
    {
        if (i_aliveObjects > 0)
        {
            GameObject returnable = GetFromPool();
            if (returnable != null)
                return returnable;
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
        go_objectToPool.transform.position = t_spawnPoint.position;
        go_objectToPool.transform.parent = t_spawnPoint;
        go_objectToPool.SetActive(false);
        i_aliveObjects++;
    }

    private GameObject AddNewObject()
    {
        GameObject newGo = GameObject.Instantiate(go_poolType);
        newGo.name = newGo.name.Replace("(Clone)", "");
        IPoolable poo = newGo.GetComponent<IPoolable>();
        Debug.Log(poo.GetGameObject().name);
        if (poo != null)
            p_objects.Add(poo);
        else
            Debug.Log("Oops... 500!");
        newGo.SetActive(false);
        return newGo;
    }

}
