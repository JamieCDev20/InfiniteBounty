using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTarget : MonoBehaviour
{
    [SerializeField] private LayerMask lm_playerLayer;

    void Start()
    {
        Collider[] _cA = Physics.OverlapSphere(transform.position, 5, lm_playerLayer);

        for (int i = 0; i < _cA.Length; i++)
            if (_cA[i].CompareTag("Player"))
            {
                transform.parent = _cA[i].transform;
                transform.localPosition = Vector3.up;
                return;
            }

        Destroy(gameObject);
    }
}