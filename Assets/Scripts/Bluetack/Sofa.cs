using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sofa : MonoBehaviourPunCallbacks, IInteractible
{

    [SerializeField] private bool b_isRightSide;
    [SerializeField] private Transform t_sitPosition;
    [SerializeField] private GameObject go_audioSourceObject;
    private bool b_isBeingUsed;
    private Transform sitter = null;
    private object pm;

    public void Interacted()
    {

    }

    private void Update()
    {
        if (sitter != null)
        {
            sitter.position = t_sitPosition.position;
            sitter.rotation = t_sitPosition.rotation;
        }
    }

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            PlayerMover pm = interactor.GetComponent<PlayerMover>();
            interactor.GetComponent<PlayerInputManager>().LocalGetOnChair();
            if (pm.b_isSitting)
                return;

            if (pm.transform.parent == null && pm.transform != transform.root)
            {

                pm.enabled = false;
                pm.GetComponent<Rigidbody>().isKinematic = true;
                pm.GetComponent<PlayerAnimator>().DoSitDown(b_isRightSide, this);
                pm.b_isSitting = true;

                pm.transform.position = t_sitPosition.position;
                pm.transform.forward = t_sitPosition.forward;
                sitter = interactor;
                if (go_audioSourceObject)
                    go_audioSourceObject.SetActive(true);

                /*
                pm.enabled = false;
                pm.GetComponent<Collider>().enabled = false;

                pm.GetComponent<PlayerAnimator>().DoSitDown(b_isRightSide, this);
                pm.transform.forward = t_sitPosition.forward;

                pm.transform.localPosition = Vector3.zero;

                pm.GetComponent<Rigidbody>().rotation = t_sitPosition.rotation;
                pm.GetComponent<Rigidbody>().isKinematic = true;

                if (go_audioSourceObject)
                    go_audioSourceObject.SetActive(true);
                */
                b_isBeingUsed = true;
            }
        }
    }

    public Transform GetChairTrasnsform()
    {
        return t_sitPosition;
    }

    internal void EndSit()
    {
        if (go_audioSourceObject)
            go_audioSourceObject.SetActive(false);
        b_isBeingUsed = false;
        sitter.GetComponent<PlayerMover>().b_isSitting = false;
        sitter = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        EndSit();
    }

}