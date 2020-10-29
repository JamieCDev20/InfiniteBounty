using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nugget : MonoBehaviour
{
    [SerializeField] private GameObject go_pickupParticles;
    [SerializeField] private int i_nuggetWorth;

    private void Start()
    {
        Invoke("TimeOut", 15);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.tag == "Player")
        {
            go_pickupParticles.SetActive(true);
            go_pickupParticles.transform.parent = null;
            gameObject.SetActive(false);
            LocationController.x.PickedUpNugget(i_nuggetWorth);
        }
    }

    private void TimeOut()
    {
        gameObject.SetActive(false);
    }

}
