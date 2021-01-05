using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sofa : MonoBehaviour, IInteractible
{

    [SerializeField] private bool b_isRightSide;
    [SerializeField] private Transform t_sitPosition;
    [SerializeField] private GameObject go_audioSourceObject;
    private bool b_isBeingUsed;

    public void Interacted()
    {

    }

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            PlayerMover pm = interactor.GetComponent<PlayerMover>();

            if (pm.transform.parent == null && pm.transform != transform.root)
            {
                pm.GetComponent<Rigidbody>().isKinematic = true;

                pm.GetComponent<PlayerAnimator>().DoSitDown(b_isRightSide, this);
                pm.transform.parent = t_sitPosition;
                pm.transform.localPosition = Vector3.zero;
                pm.transform.forward = t_sitPosition.forward;

                pm.enabled = false;
                if (go_audioSourceObject)
                    go_audioSourceObject.SetActive(true);
                b_isBeingUsed = true;
            }
        }
    }

    internal void EndSit()
    {
        if (go_audioSourceObject)
            go_audioSourceObject.SetActive(false);
        b_isBeingUsed = false;
    }
}