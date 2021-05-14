using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShieldController : MonoBehaviour
{

    [SerializeField] private MeshRenderer m_shieldRenderer;
    [SerializeField] private Transform scaleOffsetter;
    private float f_currentTime;
    [SerializeField] private Collider c_colliderToDisable;
    private Rigidbody rb;

    [Header("Audio")]
    [SerializeField] private AudioClip ac_startUpClip;
    private AudioSource as_source;
    [SerializeField] private AudioClip ac_closeClip;
    [SerializeField] private AudioClip ac_sustainClip;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        as_source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        m_shieldRenderer.transform.localScale = Vector3.zero;
        m_shieldRenderer.material.SetFloat("_CellDensity", 0);
        m_shieldRenderer.material.SetFloat("_Wiggle", Random.Range(10, 180));
        f_currentTime = 0;
        StartCoroutine(ChangeDensity());
        StartCoroutine(TurnColliderOn());
        transform.localScale = Vector3.one;
    }

    private IEnumerator TurnColliderOn()
    {
        c_colliderToDisable.enabled = false;
        yield return new WaitForSeconds(0.1f);
        c_colliderToDisable.enabled = true;
    }

    private IEnumerator ChangeDensity()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.01f);
            f_currentTime += 1;
            m_shieldRenderer.material.SetFloat("_CellDensity", Mathf.Clamp(f_currentTime, 0, 60));
            m_shieldRenderer.transform.localScale = Vector3.one * f_currentTime * 0.3f;
        }

        as_source.clip = ac_startUpClip;
        as_source.Play();

        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(0.01f);
            f_currentTime += 1;
            m_shieldRenderer.material.SetFloat("_CellDensity", Mathf.Clamp(f_currentTime, 0, 60));
            m_shieldRenderer.transform.localScale = Vector3.one * f_currentTime * 0.3f;
        }

        //yield return new WaitForSeconds(0.4f);

        as_source.clip = ac_sustainClip;
        as_source.Play();
        yield return new WaitForSeconds(13 - ac_startUpClip.length);

        as_source.clip = ac_closeClip;
        as_source.Play();

        for (int i = 0; i < 60; i++)
        {
            yield return new WaitForSeconds(0.01f);
            f_currentTime -= 1;
            m_shieldRenderer.material.SetFloat("_CellDensity", Mathf.Clamp(f_currentTime, 0, 60));
            m_shieldRenderer.transform.localScale = Vector3.one * f_currentTime * 0.3f;
        }

        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (transform.parent == null)
        {
            //Debug.Log(collision.transform.localScale, collision.transform.gameObject);
            transform.SetParent(collision.transform, true);
            transform.rotation = Quaternion.identity;
            rb.isKinematic = true;

            Vector3 newScale = new Vector3();
            newScale.x = 1f / (float)collision.transform.lossyScale.x;
            newScale.y = 1f / (float)collision.transform.lossyScale.y;
            newScale.z = 1f / (float)collision.transform.lossyScale.z;

            transform.localScale = Vector3.one;
            scaleOffsetter.localScale = newScale;

        }
    }
}