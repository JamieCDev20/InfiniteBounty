using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSuck : MonoBehaviour
{
    private bool b_shouldSuck;
    [SerializeField] private float f_suckForce;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        b_shouldSuck = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (b_shouldSuck)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (other.CompareTag("Player"))
                return;
            if (rb != null)
                if (other.transform.root != transform.root)
                    rb.velocity = ((transform.position - other.transform.position) * f_suckForce);
        }
    }

}
