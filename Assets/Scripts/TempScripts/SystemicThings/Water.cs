using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [Header("Water Effects")]
    [SerializeField] private GameObject go_electricEffect;

    public void Electrify()
    {
        go_electricEffect.SetActive(true);
        GetComponent<Systemic>().b_electric = true;

        Collider[] _cA = Physics.OverlapSphere(transform.position, 1.6f);


        for (int i = 0; i < _cA.Length; i++)
        {
            Systemic _s = _cA[i].GetComponent<Systemic>();
            if (_s != null)
                if (!_s.b_electric)
                    _s.ue_electricTrigger.Invoke();
        }


    }


}
