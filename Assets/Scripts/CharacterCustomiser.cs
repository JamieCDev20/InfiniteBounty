using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomiser : MonoBehaviourPunCallbacks, IInteractible
{

    public static CharacterCustomiser x;

    private AppearanceChanger ac_user;
    private CameraController cc_cam;
    private PlayerInputManager pim;
    [SerializeField] private Camera c_mirrorCam;
    [SerializeField] private Camera c_trueMirrorCam;
    [SerializeField] private GameObject go_uiStuff;
    private bool b_isBeingUsed;
    [SerializeField] private GameObject[] goA_playerSpots;
    [SerializeField] private GameObject[] goA_camSpots;
    private int f_lerpTime = 2;
    private Transform t_camPositionToReturnTo;

    private void Start()
    {
        x = this;
    }

    public void Interacted() { }

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            ac_user = interactor.GetComponent<AppearanceChanger>();
            pim = interactor.GetComponent<PlayerInputManager>();
            pim.GetComponent<PlayerAnimator>().SetShootability(false);
            pim.b_shouldPassInputs = false;

            cc_cam = pim.GetCamera();
            //cc_cam.gameObject.SetActive(false);
            go_uiStuff.SetActive(true);

            ac_user.GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<ToolTip>().enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            interactor.position = goA_playerSpots[pim.GetID()].transform.position;
            interactor.forward = goA_playerSpots[pim.GetID()].transform.forward;

            //c_mirrorCam.gameObject.SetActive(true);
            //c_mirrorCam.transform.position = goA_camSpots[pim.GetID()].transform.position;
            b_isBeingUsed = true;
            Camera.main.GetComponent<CameraRespectWalls>().enabled = false;
            StartCoroutine(MoveCamera(goA_camSpots[pim.GetID()].transform, pim.GetCamera().transform, true));

            c_mirrorCam.enabled = true;
            c_trueMirrorCam.enabled = true;
        }
    }

    public void EndInteract()
    {
        if (!b_isBeingUsed)
            return;
        pim.b_shouldPassInputs = true;
        ac_user.GetComponent<Rigidbody>().isKinematic = false;
        pim.GetComponent<PlayerAnimator>().SetShootability(true);
        go_uiStuff.SetActive(false);
        cc_cam.gameObject.SetActive(true);
        c_mirrorCam.gameObject.SetActive(false);
        b_isBeingUsed = false;
        GetComponent<ToolTip>().enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Saving");
        FindObjectOfType<SaveManager>().OnNotify(new SaveEvent(new PlayerSaveData(-1, -1, -1,
            new int[] { ac_user.Body, ac_user.Head, ac_user.Arms, ac_user.Feet },
            null, null, null, null, null, -1)));

        TutorialManager.x.InteractedWithReflectron();

        StartCoroutine(MoveCamera(pim.GetCamera().transform, pim.GetCamera().transform, false));

        SetCameraBasedOnQualityLevel();

    }

    public IEnumerator MoveCamera(Transform _t_transformToMoveTo, Transform _t_cameraToMove, bool _b_comingIntoMachine)
    {
        Transform _t = Camera.main.transform;
        float t = 0;
        if (_b_comingIntoMachine)
            _t.parent = goA_camSpots[pim.GetID()].transform;
        else
            Camera.main.transform.parent = _t_cameraToMove;

        Vector3 start = _t.localPosition;
        Quaternion iRot = _t.rotation;

        while (t < 1)
        {
            _t.localPosition = Vector3.Lerp(start, _b_comingIntoMachine ? Vector3.zero : Vector3.forward * -4, t);
            _t.rotation = Quaternion.Lerp(iRot, _t_transformToMoveTo.rotation, t);
            t += (Time.deltaTime * 2);
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
            Camera.main.transform.localEulerAngles = new Vector3(5, 0, 0);
            Camera.main.transform.localPosition = new Vector3(0, 0, -4);
        }
    }

    public void NextHead()
    {
        ac_user.NextHead();
    }
    public void LastHead()
    {
        ac_user.LastHead();
    }

    public void NextBody()
    {
        ac_user.NextBody();
    }
    public void LastBody()
    {
        ac_user.LastBody();
    }

    public void NextArms()
    {
        ac_user.NextArms();
    }
    public void LastArms()
    {
        ac_user.LastArms();
    }

    public void NextLegs()
    {
        ac_user.NextFeet();
    }

    public void LastLegs()
    {
        ac_user.LastFeet();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        EndInteract();
    }

    public override void OnJoinedRoom()
    {
        SetCameraBasedOnQualityLevel();
    }

    public void SetCameraBasedOnQualityLevel()
    {
        if (QualitySettings.GetQualityLevel() == 0)
        {
            c_mirrorCam.enabled = false;
            c_trueMirrorCam.enabled = false;
        }
    }

}
