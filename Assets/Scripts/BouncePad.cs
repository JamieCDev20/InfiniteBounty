using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{

    [SerializeField] private LayerMask lm_layersToBounce;
    [SerializeField] private float f_bounceForce;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    public void Jump()
    {
        Collider[] _cA = Physics.OverlapSphere(transform.position, 1, lm_layersToBounce, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < _cA.Length; i++)
        {
            _cA[i].GetComponent<Rigidbody>().AddForce(transform.up * f_bounceForce, ForceMode.Impulse);
        }
    }



}
