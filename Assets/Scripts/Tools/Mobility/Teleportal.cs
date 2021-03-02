using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportal : MonoBehaviour
{
    private bool b_isOpen;
    [SerializeField] private Teleportal tp_otherPortal;
    private List<Rigidbody> rbL_recentlyTeleported = new List<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        if (b_isOpen)
        {
            Rigidbody _rb = other.GetComponent<Rigidbody>();
            if (_rb != null)
            {
                if (!rbL_recentlyTeleported.Contains(_rb))
                {
                    rbL_recentlyTeleported.Add(_rb);
                    StartCoroutine(RemoveFromPool(_rb));

                    other.transform.position = tp_otherPortal.transform.position;
                    tp_otherPortal.CloseForCooldown(_rb);
                }
            }
        }
    }

    private IEnumerator RemoveFromPool(Rigidbody _rb)
    {
        yield return new WaitForSeconds(0.5f);
        rbL_recentlyTeleported.Remove(_rb);
    }

    internal void CloseForCooldown(Rigidbody _rb)
    {
        rbL_recentlyTeleported.Add(_rb);
        StartCoroutine(RemoveFromPool(_rb));
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
