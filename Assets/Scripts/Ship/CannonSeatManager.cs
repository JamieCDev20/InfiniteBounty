using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonSeatManager : MonoBehaviourPun, IInteractible
{

    public static CannonSeatManager x;
    private int sittingCount;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform t_lockPosition;
    private float f_lerpTime = 2;
    [SerializeField] private Transform t_camParent;
    private bool b_hasLocalInteracted;


    private void Awake()
    {
        x = this;
        photonView.ViewID = 700556;
        PhotonNetwork.RegisterPhotonView(photonView);
    }

    public void StartedSitting()
    {
        photonView.RPC(nameof(RemoteSit), RpcTarget.All);
    }

    [PunRPC]
    public void RemoteSit()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        sittingCount++;
        if (sittingCount >= PhotonNetwork.CurrentRoom.PlayerCount)
            LoadingScreenManager.x.CallLoadLevel(ModeSelect.x.GetModeName());

    }

    [PunRPC]
    public void RemoteStopSitting()
    {
        if (PhotonNetwork.IsMasterClient)
            sittingCount--;
    }

    public void EndedSitting()
    {
        //photonView?.RPC(nameof(RemoteStopSitting), RpcTarget.All);
    }

    #region IInteractable

    public void Interacted() { }

    public void Interacted(Transform interactor)
    {
        if (b_hasLocalInteracted)
            return;

        b_hasLocalInteracted = true;
        StartCoroutine(DoTheWalkIntoThePod(interactor));
    }

    #endregion

    private IEnumerator DoTheWalkIntoThePod(Transform interactor)
    {
        PlayerInputManager pim = interactor.GetComponent<PlayerInputManager>();
        PlayerMover pm = interactor.GetComponent<PlayerMover>();
        //interactor.GetComponent<PlayerInputManager>().LocalGetOnChair();
        PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
        StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform));
        pim.GetCamera().CancelInputs();

        if (!pm.b_isSitting)
        {
            yield return new WaitForSeconds(0.5f);
            anim.SetTrigger("DoorOpen");
            pim.GetCamera().CancelInputs();

            if (pm.transform.parent == null && pm.transform != transform.root)
            {
                interactor.position = t_lockPosition.position;
                interactor.transform.forward = t_lockPosition.forward;

                pm.GetComponent<Rigidbody>().isKinematic = true;
                pim.b_shouldPassInputs = false;
                pm.enabled = false;

                _pa.SetShootability(false);
                _pa.StopWalking();
                _pa.WalkInDropPod();

                //This should be how long the player's animation is
                yield return new WaitForSeconds(2);
            }

            anim.SetTrigger("DoorClose");
            yield return new WaitForSeconds(0.5f);
            StartedSitting();
            _pa.StartWalking();
            _pa.SetShootability(true);
            yield return new WaitForSeconds(1);
            Camera.main.GetComponent<CameraRespectWalls>().enabled = true;
            Camera.main.transform.localEulerAngles = Vector3.right * 5;
        }
    }


    public IEnumerator MoveCamera(Transform _t_transformToMoveTo, Transform _t_cameraToMove)
    {
        Transform _t = Camera.main.transform;
        _t.GetComponent<CameraRespectWalls>().enabled = false;
        float t = 0;

        Vector3 start = _t.position;
        Quaternion iRot = _t.rotation;

        while (t < 1)
        {
            _t.position = Vector3.Lerp(start, _t_transformToMoveTo.position, t);
            _t.rotation = Quaternion.Lerp(iRot, _t_transformToMoveTo.rotation, t);
            t += (Time.deltaTime * (1 / f_lerpTime));
            yield return new WaitForEndOfFrame();
        }

        _t.position = _t_transformToMoveTo.position;
        _t.rotation = _t_transformToMoveTo.rotation;
    }


}