using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager x;

    [SerializeField] private StringPoolDictionary pools;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        x = this;

        InitialisePools();

    }

    private void InitialisePools()
    {
        foreach (Pool p in pools.Values)
        {
            p.InitializePool();
        }
    }

    public GameObject SpawnObject(GameObject toSpawn)
    {
        Debug.Log("attempting to spawn: " + toSpawn.name);
        if (pools.ContainsKey(toSpawn.name))
        {
            Debug.Log("Contains key: " + toSpawn.name);
            return pools[toSpawn.name].SpawnObject();
        }
        Debug.Log("did not contain key: " + toSpawn.name + " creating new pool");
        CreateNewPool(toSpawn);
        return SpawnObject(toSpawn);

    }

    public GameObject SpawnObject(GameObject toSpawn, Transform parent)
    {
        GameObject ob = SpawnObject(toSpawn);
        ob.transform.SetParent(parent);
        return ob;
    }

    public GameObject SpawnObject(GameObject toSpawn, Vector3 position)
    {
        GameObject ob = SpawnObject(toSpawn);
        ob.transform.position = position;
        return ob;
    }

    public GameObject SpawnObject(GameObject toSpawn, Vector3 position, Quaternion rotation)
    {
        GameObject ob = SpawnObject(toSpawn);
        ob.transform.position = position;
        ob.transform.rotation = rotation;
        return ob;
    }

    public GameObject SpawnObject(GameObject toSpawn, Transform parent, Quaternion rotation)
    {
        GameObject ob = SpawnObject(toSpawn);
        ob.transform.parent = parent;
        ob.transform.rotation = rotation;
        return ob;
    }

    private void CreateNewPool(GameObject type)
    {
        pools.Add(type.name, new Pool(5, type));
        Debug.Log("New pool of type: " + type.name + " added");
    }

    public void ReturnObjectToPool(GameObject type)
    {
        Debug.Log("returning object to pool");
        if (pools.ContainsKey(type.name))
        {
            Debug.Log("Containts key: " + type.name);
            pools[type.name].ReturnToPool(type);
        }
        else
        {
            Debug.Log("did not contain key: " + type.name);
        }
    }

}
