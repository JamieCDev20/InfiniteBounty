using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager of all the pools the scene will need ABV: PM
/// </summary>
public class OldPoolManager : SubjectBase
{

    /// <summary>
    /// PoolManager singleton
    /// </summary>
    public static OldPoolManager x = null;

    //Variables
    #region Serialised

    /// <summary>
    /// The array of pools
    /// </summary>
    [SerializeField] private OldPool[] pA_pools;

    #endregion

    #region Privates

    /// <summary>
    /// The dictionary for looking up pool index based on an object
    /// </summary>
    private Dictionary<string, int> D_poolIndexes = new Dictionary<string, int>();

    #endregion

    #region GetSet


    #endregion

    //Methods
    #region UnityStandards

    void Awake()
    {
        // Initialize all objects specified.
        InitializeAllObjects();

        // Make this a singleton.
        if (x == null)
            x = this;
        else if (x != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Private Voids

    /// <summary>
    /// Spawns every object for all of the pools and adds the pool to the D_pools
    /// </summary>
    private void InitializeAllObjects()
    {
        for (int i = 0; i < pA_pools.Length; i++)
        {
            // Set each array to their own size
            pA_pools[i].ResizeArray();
            D_poolIndexes.Add(pA_pools[i].GetPoolObject().name.Replace("(Clone)", ""), i);
            for (int j = 0; j < pA_pools[i].i_Size; j++)
            {
                // Add object to the pool and set its parameters.
                GameObject NewObj = Instantiate(pA_pools[i].GetPoolObject());
                NewObj.name = NewObj.name.Replace("(Clone)", "");
                pA_pools[i].AddAtIndex(j, NewObj.GetComponent<OldIPoolable>());
                NewObj.GetComponent<OldIPoolable>().SetInPool(true);
                NewObj.GetComponent<OldIPoolable>().SetIndex(j);
                NewObj.transform.position = transform.position;
                NewObj.SetActive(false);
            }
        }
    }

    #endregion

    #region Public Voids

    /// <summary>
    /// Adds a new pool and/or object to that pool and then spawns it.
    /// </summary>
    /// <param name="_go_objectToSpawn">The type of prefab that will be stored/spawned.</param>
    /// <param name="_v_spawnPoint">Where to place the object.</param>
    /// <param name="_q_rotation">The orientation of the object.</param>
    /// <param name="hasAKey">Check to see if there is an existing pool.</param>
    private void AddNewToPoolAndSpawn(GameObject _go_objectToSpawn, Vector3 _v_spawnPoint, Quaternion _q_rotation, bool hasAKey)
    {
        // Create a new pool if there isn't already one.
        if (!hasAKey)
        {
            OldPool newPool = new OldPool();
            newPool.SetType(_go_objectToSpawn);
            newPool.ResizeArray();
            int newSize = pA_pools.Length + 1;
            OldPool[] newPoolArr = new OldPool[newSize];
            for (int i = 0; i < pA_pools.Length; i++)
            {
                newPoolArr[i] = pA_pools[i];
            }
            newPoolArr[newSize - 1] = newPool;
            pA_pools = newPoolArr;
            D_poolIndexes.Add(_go_objectToSpawn.name, pA_pools.Length - 1);
        }

        // Add a new object to the pool.
        pA_pools[D_poolIndexes[_go_objectToSpawn.name.Replace("(Clone)", "")]].ResizeArray(_go_objectToSpawn.GetComponent<OldIPoolable>());
        int index = D_poolIndexes[_go_objectToSpawn.name];
        _go_objectToSpawn = pA_pools[index].Spawn();
        RemoveFromPool(index, _v_spawnPoint, _q_rotation);
    }

    /// <summary>
    /// Takes an object and sets it to active, thereby placing it in the scene.
    /// </summary>
    /// <param name="index">Object to set active.</param>
    /// <param name="_v_spawnPoint">Where to place the object.</param>
    /// <param name="_q_rotation">The orientation of the object.</param>
    private void RemoveFromPool(int index, Vector3 _v_spawnPoint, Quaternion _q_rotation)
    {
        pA_pools[index].SetActiveAtIndex(_v_spawnPoint, _q_rotation);
    }

    /// <summary>
    /// Returns an inactive/"dead" game object to the pool
    /// </summary>
    /// <param name="_go_type">
    /// The object to return
    /// </param>
    /// <param name="_i_poolIndex">
    /// The index to return it to
    /// </param>
    public void ReturnInactiveToPool(GameObject _go_type, int _i_poolIndex)
    {
        // Ensuring that the object requesting to return to the pool is actually pooled.
        if (D_poolIndexes.ContainsKey(_go_type.name))
        {
            _go_type.transform.position = transform.position;
            // Turn the object off
            _go_type.SetActive(false);
        }
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns

    /// <summary>
    /// Spawns a new object from the pool if one is available otherwise it creates a new pool
    /// </summary>
    /// <param name="_go_objectToSpawn">The object to spawn</param>
    /// <param name="_v_spawnPoint">The location to spawn the object at</param>
    /// <param name="_q_rotation">The rotation of the object to spawn</param>
    public GameObject SpawnNewObject(GameObject _go_objectToSpawn, Vector3 _v_spawnPoint, Quaternion _q_rotation)
    {
        // Set up values to be used in the function
        GameObject objectToSpawn = null;
        bool hasKey = false;
        int indx = 0;
        // If the pool contains the key set the object to be spawned and the pool to spawn it from.
        if (D_poolIndexes.ContainsKey(_go_objectToSpawn.name))
        {
            hasKey = true;
            indx = D_poolIndexes[_go_objectToSpawn.name];
            objectToSpawn = pA_pools[indx].Spawn();
        }

        // If the object has correctly been obtained, retrieve it from the pool and return it.
        if (objectToSpawn != null)
        {
            RemoveFromPool(indx, _v_spawnPoint, _q_rotation);
            return objectToSpawn;
        }
        else
        {
            // If we have the key but there is no available object, create a new object in the pool and return the new object.
            if (hasKey)
            {
                GameObject newOb = Instantiate(_go_objectToSpawn);
                newOb.name = newOb.name.Replace("(Clone)", "");
                AddNewToPoolAndSpawn(newOb, _v_spawnPoint, _q_rotation, hasKey);
                return newOb;
            }

            // If we don't have the key, create a new pool of that type, and add a new object to it and return the new object
            if (_go_objectToSpawn.GetComponent<OldIPoolable>() != null)
            {
                GameObject newOb = Instantiate(_go_objectToSpawn);
                newOb.name = newOb.name.Replace("(Clone)", "");
                AddNewToPoolAndSpawn(newOb, _v_spawnPoint, _q_rotation, hasKey);
                return newOb;
                
            }
            else
                Debug.LogError("You didn't pass a poolable object DINGUS!");
        }
        // If all paths have failed, return null
        return null;
    }

    #endregion

}
