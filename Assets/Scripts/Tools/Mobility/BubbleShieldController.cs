using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShieldController : MonoBehaviour
{

    [SerializeField] private MeshRenderer m_shieldRenderer;
    private float f_currentTime;
    [SerializeField] private Collider c_colliderToDisable;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        m_shieldRenderer.transform.localScale = Vector3.zero;
        m_shieldRenderer.material.SetFloat("_CellDensity", 0);
        m_shieldRenderer.material.SetFloat("_Wiggle", Random.Range(10, 180));
        f_currentTime = 0;
        StartCoroutine(ChangeDensity());
        StartCoroutine(TurnColliderOn());
    }

    private IEnumerator TurnColliderOn()
    {
        c_colliderToDisable.enabled = false;
        yield return new WaitForSeconds(0.1f);
        c_colliderToDisable.enabled = true;
    }

    private IEnumerator ChangeDensity()
    {

        for (int i = 0; i < 60; i++)
        {
            yield return new WaitForSeconds(0.01f);
            f_currentTime += 1;
            m_shieldRenderer.material.SetFloat("_CellDensity", Mathf.Clamp(f_currentTime, 0, 60));
            m_shieldRenderer.transform.localScale = Vector3.one * f_currentTime * 0.2f;
        }

        yield return new WaitForSeconds(13);

        for (int i = 0; i < 60; i++)
        {
            yield return new WaitForSeconds(0.01f);
            f_currentTime -= 1;
            m_shieldRenderer.material.SetFloat("_CellDensity", Mathf.Clamp(f_currentTime, 0, 60));
            m_shieldRenderer.transform.localScale = Vector3.one * f_currentTime * 0.2f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (transform.parent == null)
        {
            transform.SetParent(collision.transform, true);
            rb.isKinematic = true;
        }
    }
}