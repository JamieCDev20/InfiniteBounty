using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The struct for an object pool
/// </summary>
[System.Serializable]
public struct Pool
{

    //Variables
    #region Serialised
    /// <summary>
    /// Current size of the pool
    /// </summary>
    [SerializeField] private int i_size;
    /// <summary>
    /// Object that the pool is pooling
    /// </summary>
    [SerializeField] private GameObject type;

    #endregion

    #region Privates

    /// <summary>
    /// The array of "IPoolables" called the pool
    /// </summary>
    private IPoolable[] IA_objectsInPool;

    #endregion

    #region GetSet
    /// <summary>
    /// Get for the size variable
    /// </summary>
    public int i_Size { get { return i_size; } }

    #endregion

    //Methods
    #region UnityStandards



    #endregion

    #region Private Voids

    /// <summary>
    /// Gets the first available item in the array. if none are availabe then -1 is returned to signify that all the objects are being used
    /// </summary>
    /// <returns>
    /// returns the index of the first available item or -1 if no items are available
    /// </returns>
    private int GetAvailabeIndex()
    {
        // Check each object in the pool and return the next active objects index.
        foreach (IPoolable active in IA_objectsInPool)
            if (active.GetInPool())
            {
                return active.GetIndex();
            }
        return -1;
    }

    /// <summary>
    /// Loops through the pool and sets the index of all of the items
    /// </summary>
    private void InitialiseObjectIndexes()
    {
        for (int i = 0; i < i_size; i++)
        {
            IA_objectsInPool[i].SetIndex(i);
        }
    }

    #endregion

    #region Public Voids
    
    /// <summary>
    /// Sets the array to its default size
    /// </summary>
    public void ResizeArray()
    {
        // Set the array to its i_size.
        IPoolable[] newPool = new IPoolable[i_size];
        IA_objectsInPool = newPool;
    }

    /// <summary>
    /// Increases the array size by one and adds a new object to the pool
    /// </summary>
    /// <param name="newObject">
    /// The new object to be added to the pool
    /// </param>
    public void ResizeArray(IPoolable newObject)
    {
        // Add 1 to the size of the pool and then set a new pool to that size
        i_size += 1;
        IPoolable[] newPool = new IPoolable[i_size];
        // Read in the data from the old pool
        for (int i = 0; i < IA_objectsInPool.Length; i++)
        {
            newPool[i] = IA_objectsInPool[i];
        }
        // Add the new object to the end of the resized pool
        newPool[i_size - 1] = newObject;
        // Set the objects parameters
        newObject.SetIndex(i_size - 1);
        newObject.SetInPool(true);
        newObject.GetObject().SetActive(false);
        // Set the old pool to the new pool.
        IA_objectsInPool = newPool;

        List<IPoolable> tempList = new List<IPoolable>(newPool);

    }

    /// <summary>
    /// Changes an object at an index to a new object
    /// </summary>
    /// <param name="_i_index">
    /// index to change
    /// </param>
    /// <param name="_I_delta">
    /// new object
    /// </param>
    public void AddAtIndex(int _i_index, IPoolable _I_delta)
    {
        if (_i_index < i_size)
            IA_objectsInPool[_i_index] = _I_delta;
    }

    /// <summary>
    /// Sets the next available object to active.
    /// </summary>
    /// <param name="_v_spawnPoint">Place to spawn the object</param>
    /// <param name="_q_rotation">Rotation of the object</param>
    public void SetActiveAtIndex(Vector3 _v_spawnPoint, Quaternion _q_rotation)
    {
        // Gets the next available index
        int index = GetAvailabeIndex();
        if(index != -1)
        {
            // So long as there is an available index, set the objects parameters.
            IA_objectsInPool[index].SetInPool(false);
            IA_objectsInPool[index].GetObject().SetActive(true);
            IA_objectsInPool[index].GetObject().transform.position = _v_spawnPoint;
            IA_objectsInPool[index].GetObject().transform.rotation = _q_rotation;
            IA_objectsInPool[index].OnSpawn();
        }
    }

    /// <summary>
    /// Set the type of pool
    /// </summary>
    /// <param name="newType">Type to set the pool.</param>
    public void SetType(GameObject newType)
    {
        type = newType;
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns

    /// <summary>
    /// Spawns the first available object in the pool or returns null if none are available
    /// </summary>
    /// <returns></returns>
    public GameObject Spawn()
    {
        // Get the next available object
        int index = GetAvailabeIndex();
        if (index != -1)
        {
            // Spawn it
            return IA_objectsInPool[index].GetObject();
        }
        else
            return null;
    }

    /// <summary>
    /// returns the the object that the pool is pooling
    /// </summary>
    /// <returns></returns>
    public GameObject GetPoolObject()
    {
        return type;
    }


    #endregion


}