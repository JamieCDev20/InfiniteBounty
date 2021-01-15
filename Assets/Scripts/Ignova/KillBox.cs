using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private bool b_shouldCauseBurningBum;
    private AudioSource as_source;
    [SerializeField] private AudioClip ac_burnEffect;
    [SerializeField] private GameObject go_flamePrefab;
    private List<GameObject> goL_flames = new List<GameObject>();

    private void Start()
    {
        as_source = gameObject.AddComponent<AudioSource>();
        if (go_flamePrefab)
            for (int i = 0; i < 20; i++)
                goL_flames.Add(Instantiate(go_flamePrefab));
    }

    private void Update()
    {
        if (b_shouldSinBackToStart)
            transform.position += (v_unitsPerSecond * Mathf.Sin(Time.realtimeSinceStartup));
        else
            transform.position += v_unitsPerSecond * Time.deltaTime;

        f_time += Time.deltaTime;
        if (f_time >= f_timeBetweenDamages)
            DealDamage();

    }

    private void OnTriggerEnter(Collider other)
    {
        IHitable _h = other.GetComponent<IHitable>();

        if (_h != null)
            hL_thingsWithinCloud.Add(_h);

        if (b_dealDamageOnEntry)
            _h.TakeDamage(i_damageToDeal, false);

        other.GetComponent<ExplodeWhenExpsoedToFire>()?.Explode();
    }

    private void OnTriggerExit(Collider other)
    {
        IHitable _h = other.GetComponent<IHitable>();

        if (hL_thingsWithinCloud.Contains(_h))
            hL_thingsWithinCloud.Remove(_h);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.isTrigger)
            return;

        IHitable _h = collision.collider.GetComponent<IHitable>();
        _h?.TakeDamage(i_damageToDeal, false);
        as_source.PlayOneShot(ac_burnEffect);
        if (b_shouldCauseBurningBum && collision.transform.tag == "Player")
            collision.transform.GetComponent<PlayerHealth>().StartBurningBum(v_bounceDirection);
        if (goL_flames.Count > 0)
            PlaceFlameBurst(collision.GetContact(0).point);
    }

    private void DealDamage()
    {
        for (int i = 0; i < hL_thingsWithinCloud.Count; i++)
        {
            if (hL_thingsWithinCloud[i].IsDead())
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
        yield return new WaitForSeconds(3);
        goL_flames.Add(_go_toReturn);
        _go_toReturn.SetActive(false);
    }

}