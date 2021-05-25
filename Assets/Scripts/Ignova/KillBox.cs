using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class KillBox : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 v_unitsPerSecond;
    [SerializeField] private bool b_shouldSinBackToStart;
    [SerializeField] private int i_damageToDeal;
    private List<IHitable> hL_thingsWithinCloud = new List<IHitable>();
    private float f_time;
    [SerializeField] private float f_timeBetweenDamages;
    [SerializeField] private bool b_dealDamageOnEntry;

    [Header("Bouncing")]
    [SerializeField] private Vector3 v_bounceDirection;
    [SerializeField] private bool b_shouldCauseKnockback;
    [SerializeField] private GameObject go_flamePrefab;
    private List<GameObject> goL_flames = new List<GameObject>();
    [SerializeField] private bool b_dealsFire = true;

    [Header("Getting Neutralized")]
    [SerializeField] private Color c_neutralColour;
    [SerializeField] private MeshRenderer mr_renderer;

    [Header("Audio")]
    [SerializeField] private AudioMixer am_nugMixer;
    private AudioSource as_source;
    [SerializeField] private AudioClip ac_burnEffect;


    private void Start()
    {
        as_source = gameObject.GetComponent<AudioSource>();
        if (go_flamePrefab)
            for (int i = 0; i < 20; i++)
                goL_flames.Add(Instantiate(go_flamePrefab));
    }

    private void Update()
    {
        /*
        if (b_shouldSinBackToStart)
            transform.position += (v_unitsPerSecond * Mathf.Sin(Time.realtimeSinceStartup));
        else
        */
        transform.position += v_unitsPerSecond * Time.deltaTime;

        f_time += Time.deltaTime;
        if (f_time >= f_timeBetweenDamages)
            DealDamage();

    }


    private void OnTriggerEnter(Collider other)
    {
        if (enabled)
        {
            IHitable _h = other.GetComponent<IHitable>();

            if (_h != null && !_h.IsNug())
                hL_thingsWithinCloud.Add(_h);

            _h?.TakeDamage(i_damageToDeal, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IHitable _h = other.GetComponent<IHitable>();

        if (hL_thingsWithinCloud.Contains(_h))
            hL_thingsWithinCloud.Remove(_h);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (b_dealDamageOnEntry && enabled)
        {
            if (collision.collider.isTrigger)
                return;

            IHitable _h = collision.collider.GetComponent<IHitable>();

            RaycastHit hit;
            if (Physics.Raycast(collision.transform.position, Vector3.down, out hit))
                if (hit.collider.transform != transform)
                    return;

            if (!collision.transform.CompareTag("Lilypad"))
                _h?.TakeDamage(Mathf.RoundToInt(i_damageToDeal * DiversifierManager.x.ReturnLavaScaler() * (DiversifierManager.x.ReturnIfDiverIsActive(Diversifier.LethalLava) ? 3 : 1)), false);

            if (b_shouldCauseKnockback && collision.transform.tag == "Player")
                collision.transform.GetComponent<PlayerHealth>().StartBurningBum(v_bounceDirection, b_dealsFire);

            if (am_nugMixer)
            {
                float vol = 0;
                am_nugMixer.GetFloat("Volume", out vol);
                vol = (vol + 80) / 80;
                AudioSource.PlayClipAtPoint(ac_burnEffect, collision.contacts[0].point, vol);
            }

            if (goL_flames.Count > 0 && _h != null)
                PlaceFlameBurst(collision.GetContact(0).point);
        }
    }

    private void DealDamage()
    {
        if (enabled)
            for (int i = 0; i < hL_thingsWithinCloud.Count; i++)
            {
                if (hL_thingsWithinCloud[i].IsDead() || hL_thingsWithinCloud[i].IsNug())
                {
                    hL_thingsWithinCloud.RemoveAt(i);
                    i -= 1;
                    continue;
                }
                hL_thingsWithinCloud[i].TakeDamage(i_damageToDeal, false);
            }
        f_time = 0;
    }

    private void PlaceFlameBurst(Vector3 _v_posToPlace)
    {
        GameObject _go = goL_flames[0];
        goL_flames.RemoveAt(0);
        _go.SetActive(true);
        _go.transform.position = _v_posToPlace;
        StartCoroutine(ReturnToPool(_go));
    }
    private IEnumerator ReturnToPool(GameObject _go_toReturn)
    {
        yield return new WaitForSeconds(1.3f);
        goL_flames.Add(_go_toReturn);
        _go_toReturn.SetActive(false);
    }


    internal void Neutralize()
    {
        StartCoroutine(NeutralizeCoroutine());
    }
    private IEnumerator NeutralizeCoroutine()
    {
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            mr_renderer.material.SetFloat("TimeOffset", Mathf.Lerp(mr_renderer.material.GetFloat("TimeOffset"), 0, 0.2f));
        }

        enabled = false;

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            mr_renderer.material.SetFloat("LavaActive", Mathf.Lerp(1, 0, 0.2f));
            mr_renderer.material.SetFloat("LavaNeutral", Mathf.Lerp(0, 1, 0.2f));
        }
        mr_renderer.material.SetFloat("LavaActive", 0);
        mr_renderer.material.SetFloat("LavaNeutral", 1);
    }
}