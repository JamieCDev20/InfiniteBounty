using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InductionTour : MonoBehaviour, IInteractible
{
    private bool b_isBeingUsed;
    private PlayerInputManager pim;
    [SerializeField] private Transform t_playerPos;
    private Transform t_camPositionToReturnTo;
    [SerializeField] private Canvas c_infoCanvas;
    [SerializeField] private Text t_text;
    [SerializeField] private float f_cameraSpeed;

    [Space, SerializeField] private TourStop[] tsA_tourStops = new TourStop[0];
    private float f_currentTime;
    private int i_currentStop;
    private Camera c_cam;

    #region Interactions

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            interactor.position = t_playerPos.position;
            interactor.transform.forward = t_playerPos.forward;

            pim = interactor.GetComponent<PlayerInputManager>();
            pim.enabled = false;
            PlayerMover pm = pim.GetComponent<PlayerMover>();
            c_cam = pim.GetCamera().GetComponentInChildren<Camera>();
            pm.GetComponent<Rigidbody>().isKinematic = true;
            pim.b_shouldPassInputs = false;

            pm.enabled = false;
            t_camPositionToReturnTo = pim.GetCamera().transform;
            pim.GetCamera().enabled = false;
            pim.GetCamera().GetComponent<ToolTipper>().StopShowing();
            pim.GetCamera().GetComponent<HUDController>().StopShowing();
            Camera.main.GetComponent<CameraRespectWalls>().enabled = false;
            PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
            _pa.SetShootability(false);
            _pa.StopWalking();

            c_infoCanvas.enabled = true;

            i_currentStop = -1;
            NextStop();
            b_isBeingUsed = true;
        }
    }


    public void Interacted() { }

    public void EndInteract()
    {
        StartCoroutine(MoveCamera(t_camPositionToReturnTo, c_cam.transform, false));

        i_currentStop = 0;
        PlayerMover pm = pim?.GetComponent<PlayerMover>();
        pm.GetComponent<Rigidbody>().isKinematic = false;
        pim.b_shouldPassInputs = true;
        pm.enabled = true;
        pim.enabled = true;
        pim.GetCamera().GetComponent<ToolTipper>().StartShowing();
        pim.GetCamera().GetComponent<HUDController>().StartShowing();

        StartCoroutine(MoveCamera(t_camPositionToReturnTo, pim.GetCamera().transform, false));

        c_infoCanvas.enabled = false;
        pim.GetCamera().enabled = true;
        PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
        _pa.SetShootability(true);
        _pa.StartWalking();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        b_isBeingUsed = false;
    }

    #endregion


    public IEnumerator MoveCamera(Transform _t_transformToMoveTo, Transform _t_cameraToMove, bool _b_comingIntoMachine)
    {
        Transform _t = Camera.main.transform;
        float t = 0;
        if (_b_comingIntoMachine)
            _t.parent = tsA_tourStops[i_currentStop].t_cameraPos;
        else
            Camera.main.transform.parent = _t_cameraToMove;

        Vector3 start = _t.localPosition;
        Quaternion iRot = _t.rotation;

        while (t < 1)
        {
            _t.localPosition = Vector3.Lerp(start, Vector3.zero, t);
            _t.rotation = Quaternion.Lerp(iRot, _t_transformToMoveTo.rotation, t);
            t += (Time.deltaTime * (1 / f_cameraSpeed));
            yield return new WaitForEndOfFrame();
        }

        if (_b_comingIntoMachine)
        {
            _t.localPosition = Vector3.zero;
            _t.localEulerAngles = Vector3.zero;
        }
        else
        {
            Camera.main.GetComponent<CameraRespectWalls>().enabled = true;
            Camera.main.transform.localPosition = new Vector3(0, 0, -4);
            Camera.main.transform.localEulerAngles = new Vector3(10, 0, 0);
        }
    }

    private void Update()
    {
        if (b_isBeingUsed)
        {
            f_currentTime -= Time.deltaTime;
            if (f_currentTime < 0)
                NextStop();

            if (Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0))
                NextStop();
        }
    }

    private void NextStop()
    {
        i_currentStop++;

        if (i_currentStop >= tsA_tourStops.Length)
        {
            EndInteract();
            return;
        }

        f_currentTime = tsA_tourStops[i_currentStop].f_timeToWait;
        StartCoroutine(MoveCamera(tsA_tourStops[i_currentStop].t_cameraPos, c_cam.transform, true));
        t_text.text = tsA_tourStops[i_currentStop].s_textToShow;
    }

    [Serializable]
    private struct TourStop
    {
        public string s_name;
        public Transform t_cameraPos;
        [TextArea] public string s_textToShow;
        public float f_timeToWait;
    }
}