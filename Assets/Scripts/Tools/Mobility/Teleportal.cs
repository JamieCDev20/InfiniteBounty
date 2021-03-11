using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportal : MonoBehaviour
{
    private bool b_isOpen;
    [SerializeField] private Teleportal tp_otherPortal;
    private List<Rigidbody> rbL_recentlyTeleported = new List<Rigidbody>();
    [SerializeField] private float f_hyuckForce;

    private void OnTriggerEnter(Collider other)
    {
        if (b_isOpen)
        {
            Rigidbody _rb = other.GetComponent<Rigidbody>();
            if (_rb != null)
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

        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForSeconds(0.01f);
            _go_object.transform.position = Vector3.Lerp(_go_object.transform.position, tp_otherPortal.transform.position, 0.3f);
        }

        _go_object.transform.position = tp_otherPortal.transform.position;
        _rb.velocity *= 2;// (transform.forward * f_hyuckForce, ForceMode.Impulse);
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

    internal void Setup(float _f_lifeSpan)
    {
        StartCoroutine(ClosePortal(_f_lifeSpan));
        GetComponentInChildren<ParticleSystem>().Play();
    }

    private IEnumerator ClosePortal(float _f_lifeSpan)
    {
        yield return new WaitForSeconds(1);
        b_isOpen = true;

        yield return new WaitForSeconds(_f_lifeSpan);
        GetComponentInChildren<ParticleSystem>().Stop();
        b_isOpen = false;


        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }


}
