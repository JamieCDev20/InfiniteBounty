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

    }


}
