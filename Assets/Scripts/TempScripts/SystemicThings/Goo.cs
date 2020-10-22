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
        GetComponent<Collider>().isTrigger = true;
        go_fireEffect.SetActive(true);
    }

    public void GetWashedAway()
    {
        GetComponent<Systemic>().b_fire = false;
        GetComponent<Collider>().isTrigger = false;
        go_fireEffect.SetActive(false);
    }


}