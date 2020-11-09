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
            p.InitializePool(transform);
        }
    }

    public GameObject SpawnObject(GameObject toSpawn)
    {
        if (pools.ContainsKey(toSpawn.name))
        {
            return pools[toSpawn.name].SpawnObject();
        }

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
    }

    public void ReturnObjectToPool(GameObject type)
    {
        if (pools.ContainsKey(type.name))
            pools[type.name].ReturnToPool(type);
    }

}
