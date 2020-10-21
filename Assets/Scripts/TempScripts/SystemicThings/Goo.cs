using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goo : MonoBehaviour
{
    [Header("Ignition")]
    [SerializeField] private GameObject go_fireEffect;

    public void Ignite()
    {
        GetComponent<Systemic>().b_fire = true;
        go_fireEffect.SetActive(true);
    }



}