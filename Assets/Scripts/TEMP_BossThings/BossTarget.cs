using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTarget : MonoBehaviour
{
    [SerializeField] private LayerMask lm_playerLayer;
    [SerializeField] private float f_delay;
    [SerializeField] private TEMP_Boss tb_boss;

    void Start()
    {
        StartCoroutine(AddSelfToTargets());
    }

    private IEnumerator AddSelfToTargets()
    {
        yield return new WaitForSeconds(f_delay);
        Collider[] _cA = Physics.OverlapSphere(transform.position, 5, lm_playerLayer);

        for (int i = 0; i < _cA.Length; i++)
            if (_cA[i].CompareTag("Player"))
            {
                transform.parent = _cA[i].transform;
                transform.localPosition = Vector3.up;
                tb_boss.tL_potentialTarget.Add(transform);
                break;
            }        
    }

}