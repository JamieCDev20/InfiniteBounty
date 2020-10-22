using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownLode : MonoBehaviour
{

    [Header("Thrown Lode different types")]
    [SerializeField] private GameObject[] goA_nugTypes = new GameObject[2];

    void Start()
    {
        int _i_ = Random.Range(0, goA_nugTypes.Length);
        goA_nugTypes[_i_].SetActive(true);

        if (_i_ == 1)
            GetComponent<Systemic>().b_fire = true;
        else if (_i_ == 2)
            GetComponent<Systemic>().b_electric = true;

    }
}