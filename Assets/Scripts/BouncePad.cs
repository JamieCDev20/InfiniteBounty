using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{

    [SerializeField] private LayerMask lm_layersToBounce;
    [SerializeField] private float f_bounceForce;
    [SerializeField] private AudioClip ac_useSound;
    private AudioSource as_source;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Jump();
    }

    public void Jump()
    {
        Collider[] _cA = Physics.OverlapSphere(transform.position, 1, lm_layersToBounce, QueryTriggerInteraction.Ignore);
        bool _b_sound = false;

        for (int i = 0; i < _cA.Length; i++)
        {
            if (_cA[i].attachedRigidbody != null)
            {
                _cA[i].attachedRigidbody.AddForce(transform.up * f_bounceForce, ForceMode.Impulse);
                _b_sound = true;
            }
        }

        if (_b_sound)
            as_source.PlayOneShot(ac_useSound);
    }



}
