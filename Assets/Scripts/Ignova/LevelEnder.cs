using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnder : MonoBehaviourPun, IInteractible
{
    [SerializeField] private GameObject go_tractorBeam;
    [SerializeField] private Transform t_progressBar;
    [SerializeField] private Animator a_shipAnimator;
    [SerializeField] private float f_waitTime = 10;
    [SerializeField] private float f_animationLength = 15;
    private bool b_active;

    public void Interacted() { }

    public void Interacted(Transform interactor)
    {
        if (!b_active)
        {
            StartCoroutine(Countdown());
            b_active = true;
        }
    }

    private IEnumerator Countdown()
    {
        float _f_currentTime = 0;

        while (_f_currentTime < f_waitTime)
        {
            _f_currentTime += Time.deltaTime;
            t_progressBar.localPosition = new Vector3(0, (float)(-1 + (_f_currentTime / f_waitTime)), 0);
            t_progressBar.transform.localScale = new Vector3(1, (float)(_f_currentTime / f_waitTime), 1);

            if (_f_currentTime >= f_waitTime - f_animationLength)
                a_shipAnimator.SetBool("Entry", true);

            yield return new WaitForEndOfFrame();
        }

        go_tractorBeam.SetActive(true);
    }

}
