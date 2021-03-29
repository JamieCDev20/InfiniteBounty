using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsGolfBall : MonoBehaviour
{
    [SerializeField] private MeshRenderer mr_renderer;
    private Rigidbody rb;
    private Vector3 _v_startPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _v_startPos = transform.position;
        StartCoroutine(ReturnToCentre());
    }

    private void Update()
    {
        if (transform.position.y < -100)
            StartCoroutine(ReturnToCentre());
    }

    private IEnumerator ReturnToCentre()
    {
        mr_renderer.material.SetFloat("Visibility", 1);
        transform.position = _v_startPos;
        rb.isKinematic = true;

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            mr_renderer.material.SetFloat("Visibility", 1 - (i * 0.02f));
        }
        rb.isKinematic = false;
    }

}
