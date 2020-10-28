using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    bool GetInPool();
    GameObject GetObject();
    int GetIndex();
    void SetIndex(int _i_index);
    void SetInPool(bool _b_delta);
    void OnSpawn();
    void Die();


}
