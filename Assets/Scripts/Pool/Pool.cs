using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    [SerializeField] private GameObject go_poolType;
    [SerializeField] private int i_initialSize;
    private bool b_isNetworkedPool;
    private string s_path;
    private HashSet<IPoolable> p_objects;
    private int i_aliveObjects;

    /// <summary>
    /// Set the pools initial values and then initialise the pool
    /// </summary>
    /// <param name="size">Heft of the pool</param>
    /// <param name="type">Object of the pool</param>
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
        // Create a new pool
        p_objects = new HashSet<IPoolable>();
        if (go_poolType != null)
        {
            if (go_poolType.GetComponent<IPoolable>() != null)
            {
                b_isNetworkedPool = go_poolType.GetComponent<IPoolable>().IsNetworkedObject();
                s_path = go_poolType.GetComponent<IPoolable>().ResourcePath();

            }
        }
        i_aliveObjects = i_initialSize;
        // Create as many objects as your initial size
        for (int i = 0; i < i_initialSize; i++)
        {
            //Debug.Log($"Initialise {s_path}");
            AddNewObject();
        }
    }

    /// <summary>
    /// Pull an object from a pool or create a new one
    /// </summary>
    /// <returns>An object of the pools type</returns>
    public GameObject SpawnObject()
    {
        // Objects need to be alive to be spawned
        if (i_aliveObjects > 0)
        {
            // Objects need to exist in order to spawn
            GameObject returnable = GetFromPool();
            if (returnable != null)
            {
                // Remove the object from the pool
                i_aliveObjects--;
                returnable.transform.parent = null;
                return returnable;
            }
        }
        // Create a new object because you haven't found one
        GameObject newGo = AddNewObject();
        newGo.SetActive(true);
        return newGo;
    }

    /// <summary>
    /// Find an available object
    /// </summary>
    /// <returns>Next available object</returns>
    private GameObject GetFromPool()
    {
        // Check each object to see if it's currently active
        foreach (IPoolable poo in p_objects)
        {
            if (!poo.GetGameObject().activeInHierarchy)
            {
                // Pop that bidness from the pool
                poo.GetGameObject().SetActive(true);
                poo.GetGameObject().transform.parent = null;
                return poo.GetGameObject();
            }
        }
        // If ya here you didn't find anything
        return null;
    }

    /// <summary>
    /// Send objects to bed (Throw it back into the pool)
    /// </summary>
    /// <param name="go_objectToPool">Object to be set back into the pool</param>
    public void ReturnToPool(GameObject go_objectToPool)
    {
        // The object gets sent back to the pool
        go_objectToPool.transform.position = PoolManager.x.transform.position;
        go_objectToPool.transform.parent = PoolManager.x.transform;
        go_objectToPool.SetActive(false);
        // Iterate the objects back up
        i_aliveObjects++;
    }

    /// <summary>
    /// Add a new object to the pool
    /// </summary>
    /// <returns>a new game object</returns>
    private GameObject AddNewObject()
    {
        // Create a new pool when one doesn't exist
        if (p_objects == null)
            p_objects = new HashSet<IPoolable>();
        // Create a new object
        GameObject newGo = null;
        //If the object is a networked object then spawn it across the network. Otherwise, don't
        if (b_isNetworkedPool)
            newGo = PhotonNetwork.Instantiate(string.Format("{0}{1}", s_path, go_poolType.name), PoolManager.x.transform.position, Quaternion.identity);
        else
        {
            newGo = GameObject.Instantiate(go_poolType);
        }
        newGo.transform.parent = PoolManager.x.transform;
        // Make the object searchable
        newGo.name = newGo.name.Replace("(Clone)", "");
        IPoolable poo = newGo.GetComponent<IPoolable>();
        // Store the object in the pool
        if (poo != null)
            p_objects.Add(poo);
        // Disable it
        newGo.SetActive(false);
        return newGo;
    }

    public void ResetPool()
    {
        if (p_objects == null)
            return;
        foreach (IPoolable ip in p_objects)
        {
            ip.Die();
        }
    }

    public HashSet<IPoolable> GetPooledObjects()
    {
        return p_objects;
    }

}
