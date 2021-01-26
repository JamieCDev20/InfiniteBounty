using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Diversifier
{
    None, JackedRabbits, GigaGeysers, SolarStorm, 

}


public class SlotMachine : MonoBehaviour, IInteractible
{
    private Animator anim;


    [Header("Interactable Things That Moves the Camera")]
    [SerializeField] private Transform t_playerPos;
    private bool b_isBeingUsed;
    private PlayerInputManager pim;
    private Transform t_camPositionToReturnTo;
    [SerializeField] private Transform t_camParent;
    [SerializeField] private float f_lerpTime = 0.5f;

    [Header("Wheels")]
    [SerializeField] private WheelData[] wdA_wheels = new WheelData[3];

    private Diversifier[] dA_activeDiversifiers = new Diversifier[3];

    [Header("Diversifier Sprites")]
    [SerializeField] private DiversifierInfo[] diA_diversifiers = new DiversifierInfo[0];

    [Header("UI References")]
    [SerializeField] private Text t_nameText;
    [SerializeField] private Text t_descriptionText;
    [SerializeField] private Canvas c_infoCanvas;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }


    #region Interactions

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            interactor.position = t_playerPos.position;
            interactor.transform.forward = t_playerPos.forward;

            b_isBeingUsed = true;
            pim = interactor.GetComponent<PlayerInputManager>();
            PlayerMover pm = pim.GetComponent<PlayerMover>();
            pm.GetComponent<Rigidbody>().isKinematic = true;
            pim.b_shouldPassInputs = false;
            pm.enabled = false;
            t_camPositionToReturnTo = pim.GetCamera().transform;
            pim.GetCamera().enabled = false;
            Camera.main.GetComponent<CameraRespectWalls>().enabled = false;
            PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
            _pa.SetShootability(false);
            _pa.StopWalking();

            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
            c_infoCanvas.enabled = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    public void Interacted() { }

    public void EndInteract()
    {
        PlayerMover pm = pim?.GetComponent<PlayerMover>();
        pm.GetComponent<Rigidbody>().isKinematic = false;
        pim.b_shouldPassInputs = true;
        pm.enabled = true;

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
            _t.parent = t_camParent;
        else
            Camera.main.transform.parent = _t_cameraToMove;

        Vector3 start = _t.localPosition;
        Quaternion iRot = _t.rotation;

        while (t < 1)
        {
            _t.localPosition = Vector3.Lerp(start, Vector3.zero, t);
            _t.rotation = Quaternion.Lerp(iRot, _t_transformToMoveTo.rotation, t);
            t += (Time.deltaTime * (1 / f_lerpTime));
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


    private void GenerateDiversifiers()
    {

    }

    private IEnumerator RollWheel(WheelData _wd_wheel, float _f_startDelay)
    {
        yield return new WaitForSeconds(_f_startDelay);

        int _i_diversifierToRoll = UnityEngine.Random.Range(0, _wd_wheel.dL_wheelDiversifiers.Count);
        int _i_currentIndex = 0;

        print(_i_diversifierToRoll);
        for (int i = 0; i < 80 - _i_diversifierToRoll; i++)
        {
            yield return new WaitForSeconds(0.009f);
            _wd_wheel.go_wheelSpinner.transform.Rotate(Vector3.right * 22.5f, Space.Self);
            yield return new WaitForSeconds(0.009f);
            _wd_wheel.go_wheelSpinner.transform.Rotate(Vector3.right * 22.5f, Space.Self);

            SpriteRenderer _sr = _wd_wheel.srL_wheelSprites[0];
            _wd_wheel.srL_wheelSprites.RemoveAt(0);
            _wd_wheel.srL_wheelSprites.Add(_sr);
            _sr.sprite = diA_diversifiers[(int)_wd_wheel.dL_wheelDiversifiers[_i_currentIndex]].s_image;
            _i_currentIndex++;
            if (_i_currentIndex >= _wd_wheel.dL_wheelDiversifiers.Count)
                _i_currentIndex = 0;
        }

        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.009f);
            _wd_wheel.go_wheelSpinner.transform.Rotate(Vector3.right * 11.25f, Space.Self);
        }

        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.02f);
            _wd_wheel.go_wheelSpinner.transform.Rotate(-Vector3.right * 22.5f, Space.Self);
        }

        dA_activeDiversifiers[_wd_wheel.i_wheelIndex] = _wd_wheel.dL_wheelDiversifiers[_i_diversifierToRoll];
        anim.SetBool("PullLever", false);
    }


    internal void PullLever()
    {
        anim.SetBool("PullLever", true);
        StartCoroutine(RollWheel(wdA_wheels[0], 0.19f));
        StartCoroutine(RollWheel(wdA_wheels[1], 0.28f));
        StartCoroutine(RollWheel(wdA_wheels[2], 0.37f));
    }

    internal void DisplayDiversifierInfo(int _i_index)
    {
        //if (diA_diversifiers.Length > _i_index)
        {
            t_descriptionText.text = diA_diversifiers[(int)dA_activeDiversifiers[_i_index]].s_desc;
            t_nameText.text = diA_diversifiers[(int)dA_activeDiversifiers[_i_index]].s_name;
        }
    }






    [System.Serializable]
    private struct WheelData
    {
        public GameObject go_wheelSpinner;
        public List<SpriteRenderer> srL_wheelSprites;
        public int i_wheelIndex;
        public List<Diversifier> dL_wheelDiversifiers;

    }
    [System.Serializable]
    private struct DiversifierInfo
    {
        public string s_name;
        [TextArea] public string s_desc;
        public Sprite s_image;
    }

}
