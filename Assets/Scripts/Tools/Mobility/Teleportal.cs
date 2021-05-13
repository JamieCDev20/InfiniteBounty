using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportal : MonoBehaviour
{
    private bool b_isOpen;
    private Teleportal tp_otherPortal;
    private List<Rigidbody> rbL_recentlyTeleported = new List<Rigidbody>();    
    [SerializeField] private float travelTime = 0.3f;
    private AudioSource as_source;


    private void Start()
    {
        as_source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (b_isOpen)
        {
            Rigidbody _rb = other.GetComponent<Rigidbody>();
            if (_rb != null && !other.name.Contains("Augment"))
                if (!rbL_recentlyTeleported.Contains(_rb))
                {
                    StartCoroutine(TeleportObject(other.gameObject, _rb));
                    tp_otherPortal.CloseForCooldown(_rb);
                }
        }
    }

    private IEnumerator TeleportObject(GameObject _go_object, Rigidbody _rb)
    {
        if (_go_object.CompareTag("Player"))
            _go_object.GetComponent<PlayerMover>().GetTeleported();
        else
            _go_object.SetActive(false);

        as_source.Play();

        Vector3 vel = _rb.velocity;

        Vector3 start = _go_object.transform.position;
        Vector3 end = tp_otherPortal.transform.position;

        float t = 0;
        while (t < 1)
        {
            _go_object.transform.position = Vector3.Lerp(start, end, t);
            t += Time.deltaTime * (1 / travelTime);
            yield return new WaitForEndOfFrame();
        }

        _go_object.transform.position = tp_otherPortal.transform.position;
        _rb.velocity = vel;
        yield return new WaitForEndOfFrame();

        rbL_recentlyTeleported.Add(_rb);
        StartCoroutine(RemoveFromPool(_rb, false));

        _go_object.SetActive(true);
    }


    private IEnumerator RemoveFromPool(Rigidbody _rb, bool _b)
    {
        yield return new WaitForSeconds(1);
        if (_b)
            yield return new WaitForSeconds(0.3f);
        rbL_recentlyTeleported.Remove(_rb);
    }

    internal void CloseForCooldown(Rigidbody _rb)
    {
        rbL_recentlyTeleported.Add(_rb);
        StartCoroutine(RemoveFromPool(_rb, true));
    }

    internal void Setup(float _f_lifeSpan, Teleportal _tp_endPortal)
    {
        StartCoroutine(ClosePortal(_f_lifeSpan));
        GetComponentInChildren<ParticleSystem>().Play();

        tp_otherPortal = _tp_endPortal;
    }

    private IEnumerator ClosePortal(float _f_lifeSpan)
    {
        yield return new WaitForSeconds(0.5f);
        b_isOpen = true;

        yield return new WaitForSeconds(_f_lifeSpan);
        GetComponentInChildren<ParticleSystem>().Stop();
        b_isOpen = false;


        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }


}
