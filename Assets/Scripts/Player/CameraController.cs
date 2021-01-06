using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private float f_cameraSensitivity = 180;
    [SerializeField] private bool networkedCamera = false;
    [SerializeField] private Text nugCountText;
    [SerializeField] private Vector3 v_offset = Vector3.up * 3;

    [Header("Firing Cam Positions")]
    [SerializeField] private float f_rightWardOffset;
    [SerializeField] private float f_leftWardOffset;
    [SerializeField] private Vector3 v_firingBothOffset;
    [SerializeField] private float f_cameraLerpFiring;
    [SerializeField] private GameObject go_logo;

    #endregion

    #region Private

    private Vector2 v2_lookInputs;
    private Transform t_follow;
    internal PlayerInputManager pim_inputs;
    private float f_firingTime;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (!networkedCamera)
        {
            pim_inputs = transform.root.GetComponentInChildren<PlayerInputManager>();
            pim_inputs.SetCamera(this);
            Detach();
        }
    }

    private void LateUpdate()
    {
        //DoStuff
        Look();
        Follow();
    }

    #endregion

    #region Private Voids

    private void Detach()
    {
        transform.parent = null;
    }

    private void Follow()
    {
        transform.position = t_follow.position + v_offset;
    }

    private void Look()
    {
        transform.Rotate(transform.right.normalized * v2_lookInputs.y * f_cameraSensitivity * Time.deltaTime, Space.World);
        transform.Rotate(transform.up.normalized * v2_lookInputs.x * f_cameraSensitivity * Time.deltaTime, Space.World);
        transform.eulerAngles = Vector3.Scale(transform.eulerAngles, Vector3.one - Vector3.forward);
    }

    #endregion

    #region Public Voids

    internal void Recoil(float _f_recoilSeverity)
    {
        /*
        transform.Rotate(-_f_recoilSeverity, UnityEngine.Random.Range(-_f_recoilSeverity * 0.7f, _f_recoilSeverity * 0.7f), 0);
        StartCoroutine(BackToNormal(_f_recoilSeverity));
        */
    }

    private IEnumerator BackToNormal(float _f_recoilSeverity)
    {
        yield return new WaitForSeconds(0.3f);
        transform.Rotate(_f_recoilSeverity, 0, 0);
    }

    public void SetLookInput(Vector2 _v2_newLookInput)
    {
        v2_lookInputs = _v2_newLookInput;
    }

    public void SetFollow(Transform _t_newFollow)
    {
        t_follow = _t_newFollow;
        t_follow.GetComponentInChildren<Billboard>()?.gameObject.SetActive(false);
    }
    public void SetFollow(Transform _t_newFollow, Vector3 _v_newOffset)
    {
        t_follow = _t_newFollow;
        SetOffset(_v_newOffset);
    }

    public void SetOffset(Vector3 _v_newOffset)
    {
        v_offset = _v_newOffset;
    }

    public void SetPIM(PlayerInputManager _pim_newPIM)
    {
        pim_inputs = _pim_newPIM;

    }

    public void ToggleLogo(bool on)
    {
        go_logo?.SetActive(on);
    }
    

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns

    public Text GetNugCountText()
    {
        return nugCountText;
    }

    #endregion

}
