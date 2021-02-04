using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTarget : MonoBehaviour
{
    [SerializeField] private LayerMask lm_playerLayer;
    [SerializeField] private float f_delay;
    [SerializeField] private TEMP_Boss tb_boss;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        Collider[] _cA = Physics.OverlapSphere(transform.position, 5);

        for (int i = 0; i < _cA.Length; i++)
            if (_cA[i].CompareTag("Player"))
            {
                print("I, " + name + " have obtained a PLAYER. Gaze upon it in awe & fear.");
                transform.parent = _cA[i].transform;
                transform.localPosition = Vector3.up;
                tb_boss.tL_potentialTarget.Add(transform);
                break;
            }
    }
}