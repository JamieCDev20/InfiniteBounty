using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{

    [SerializeField] private int i_explosionDamage;

    void Start()
    {

        Collider[] _cA = Physics.OverlapSphere(transform.position, 10);

        for (int i = 0; i < _cA.Length; i++)
        {
            Enemy _e = _cA[i].GetComponent<Enemy>();
            if (_e != null)
                _e.TakeDamage(i_explosionDamage);

            Systemic _s = _cA[i].GetComponent<Systemic>();
            if (_s != null)
                _s.ue_fireTrigger.Invoke();

        }
    }
}