using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TargetManager : MonoBehaviour
{

    public static TargetManager x;

    private void Awake()
    {
        x = this;
    }

    public GameObject GetTaggableInRange(string _tag, float _range, Vector3 _centre)
    {
        GameObject _toReturn = null;
        List<GameObject> temp = new List<GameObject>();
        foreach (GameObject g in TagManager.x.GetTagSet(_tag))
        {
            if ((g.transform.position - _centre).sqrMagnitude < (_range * _range))
            {
                temp.Add(g);
            }
        }

        if (temp.Count > 0)
            _toReturn = temp[Random.Range(0, temp.Count)];
        return _toReturn;
    }

}
