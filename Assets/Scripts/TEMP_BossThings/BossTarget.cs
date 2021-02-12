using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTarget : MonoBehaviour
{
    [SerializeField] private BossAI tb_boss;


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);

        Collider[] _cA = Physics.OverlapSphere(transform.position, 5);

        for (int i = 0; i < _cA.Length; i++)
            if (_cA[i].CompareTag("Player"))
            {
                transform.parent = _cA[i].transform;
                transform.localPosition = Vector3.up;
                tb_boss.tL_potentialTargets.Add(transform);
                break;
            }
    }
}