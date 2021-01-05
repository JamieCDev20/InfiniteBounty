using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sofa : MonoBehaviour, IInteractible
{

    [SerializeField] private bool b_isRightSide;
    [SerializeField] private Transform t_sitPosition;

    public void Interacted()
    {

    }

    public void Interacted(Transform interactor)
    {
        PlayerMover pm = interactor.GetComponent<PlayerMover>();

        if (pm)
        {
            pm.enabled = false;
            pm.GetComponent<Rigidbody>().isKinematic = true;

            pm.GetComponent<PlayerAnimator>().DoSitDown(b_isRightSide);
            pm.transform.position = t_sitPosition.position;
            pm.transform.forward = t_sitPosition.forward;
        }
    }



}