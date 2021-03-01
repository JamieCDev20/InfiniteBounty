using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : MonoBehaviour
{

    public static PoolManager x;
    [SerializeField] private StringPoolDictionary pools;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Init();
    }
    /// <summary>
    /// Make this a singleton and initialise all the pools
    /// </summary>
    private void Init()
    {
        // SINGLETOOOOOON
        if (x != null)
            Destroy(gameObject);
        else
            x = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += SceneChange;

    }

    public void Reset()
    {
        ResetPools();
        //foreach (var i in pools)
        //{
        //    Debug.Log(i.Key);
        //}
    }

    private void SceneChange(Scene scene, LoadSceneMode mode)
    {

    }

    public void RemovePool(GameObject key)
    {
        pools.Remove(key.name);
    }

    private void ResetPools()
    {
        if(pools.Values != null)
        foreach (Pool p in pools.Values)
        {
            p.ResetPool();
        }
    }



    /// <summary>
    /// Make all the pools spawn load their assigned number of objects
    /// </summary>
    public void InitialisePools()
    {
        // Each pool is initialised during loading
        foreach (Pool p in pools.Values)
            p?.InitializePool();
    }

    /// <summary>
    /// Grab or create object from pool
    /// </summary>
    /// <param name="toSpawn">Type of object to spawn</param>
    /// <returns>The next available object</returns>
    public GameObject SpawnObject(GameObject toSpawn)
    {
        // Get the next available object
        foreach (string s in pools.Keys)
            if (s == toSpawn.name)
                return pools[s].SpawnObject();

        // Create a new pool
        CreateNewPool(toSpawn);
        return SpawnObject(toSpawn);

    }

    /// <summary>
    /// Grab or create an object from pool and set its parent
    /// </summary>
    /// <param name="toSpawn">Type of object to spawn</param>
    /// <param name="parent">Objects parent</param>
    /// <returns></returns>
    public GameObject SpawnObject(GameObject toSpawn, Transform parent)
    {
        GameObject ob = SpawnObject(toSpawn);
        ob.transform.SetParent(parent);
        return ob;
    }

    /// <summary>
    /// Grab or create object from the pool and set its position
    /// </summary>
    /// <param name="toSpawn">Type of object to spawn</param>
    /// <param name="position">Where to spawn it</param>
    /// <returns></returns>
    public GameObject SpawnObject(GameObject toSpawn, Vector3 position)
    {
        GameObject ob = SpawnObject(toSpawn);
        ob.transform.position = position;
        return ob;
    }

    /// <summary>
    /// Grab or create object from the pool, set its position and rotation
    /// </summary>
    /// <param name="toSpawn">Type of object to spawn</param>
    /// <param name="position">Where to spawn the object</param>
    /// <param name="rotation">Orientation to spawn it in</param>
    /// <returns></returns>
    public GameObject SpawnObject(GameObject toSpawn, Vector3 position, Quaternion rotation)
    {
        GameObject ob = SpawnObject(toSpawn);
        ob.transform.position = position;
        ob.transform.rotation = rotation;
        return ob;
    }

    /// <summary>
    /// Grab or create object from the pool, set its parent and rotation
    /// </summary>
    /// <param name="toSpawn">Type of object to spawn</param>
    /// <param name="parent">new parent</param>
    /// <param name="rotation">Orientation</param>
    /// <returns></returns>
    public GameObject SpawnObject(GameObject toSpawn, Transform parent, Quaternion rotation)
    {
        GameObject ob = SpawnObject(toSpawn);
        ob.transform.parent = parent;
        ob.transform.rotation = rotation;
        return ob;
    }

    public GameObject SpawnObject(string toSpawn)
    {
        // Get the next available object
        foreach (string s in pools.Keys)
            if (s == toSpawn)
                return pools[s].SpawnObject();
        return null;
    }

    public GameObject SpawnObject(string toSpawn, Transform parent, Quaternion rotation)
    {
        GameObject spawned = SpawnObject(toSpawn);
        if (spawned == null)
            return null;
        spawned.transform.parent = parent;
        spawned.transform.rotation = rotation;
        return spawned;
    }

    /// <summary>
    /// Set a new pool using the class name
    /// </summary>
    /// <param name="type">Class name</param>
    public void CreateNewPool(GameObject type)
    {
        pools.Add(type.name, new Pool(5, type));
    }
    /// <summary>
    /// Set a new pool to the stated size using the class name
    /// </summary>
    /// <param name="type">Class name</param>
    /// <param name="size">Size of pool</param>
    public void CreateNewPool(GameObject type, int size)
    {
        pools.Add(type.name, new Pool(size, type));
    }
    /// <summary>
    /// Set the object back to the pool
    /// </summary>
    /// <param name="type">Object to be set back</param>
    public void ReturnObjectToPool(GameObject type)
    {
        if (pools.ContainsKey(type.name))
            pools[type.name].ReturnToPool(type);
    }
    public void KillAllObjects(GameObject type)
    {
        if (pools.ContainsKey(type.name))
            pools[type.name].ResetPool();
    }
    /// <summary>
    /// Get the hashset of objects
    /// </summary>
    /// <param name="type">Type of pool</param>
    /// <returns></returns>
    public HashSet<IPoolable> GetPooledObjects(GameObject type)
    {
        if (pools.Contains(type.name))
            return pools[type.name].GetPooledObjects();
        else
        {
            CreateNewPool(type);
            return pools[type.name].GetPooledObjects();
        }
    }

    public bool CheckIfPoolExists(GameObject type)
    {
        foreach(string pool in pools.Keys)
        {
            if (pool == type.name)
                return true;
        }
        return false;
    }

    internal void SpawnObject(object go_augmentPrefab, object position, Quaternion quaternion)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetPools();
    }

}