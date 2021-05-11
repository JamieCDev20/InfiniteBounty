using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] internal Vector2 v2_cameraSensitivity = Vector2.one;
    [SerializeField] private bool b_invertY;
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
    private float frametime;
    private float mydeltatime;
    private Transform t_follow;
    internal PlayerInputManager pim_inputs;
    private float f_firingTime;
    private float f_yLook;
    private bool b_isSpectating;
    private Camera c_cam;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneLoad;
        c_cam = GetComponentInChildren<Camera>();
        if (!networkedCamera)
        {
            pim_inputs = transform.root.GetComponentInChildren<PlayerInputManager>();
            pim_inputs?.SetCamera(this);
            Detach();
        }
    }

    private void LateUpdate()
    {
        //DoStuff
        mydeltatime = Time.realtimeSinceStartup - frametime;
        Look();
        Follow();
        frametime = Time.realtimeSinceStartup;
    }

    #endregion

    #region Private Voids

    private void SceneLoad(Scene s, LoadSceneMode m)
    {
        if (this == null)
            return;
        transform.eulerAngles = Vector3.zero;
        f_yLook = 0;
    }

    private void Detach()
    {
        transform.parent = null;
    }

    private void Follow()
    {
        if (t_follow != null)
            transform.position = t_follow.position + v_offset;
    }

    private void Look()
    {

        float x = v2_cameraSensitivity.x * mydeltatime;
        float y = v2_cameraSensitivity.y * mydeltatime;

        f_yLook = Mathf.Clamp(f_yLook + (v2_lookInputs.y * y), -80, 40);
        transform.Rotate(transform.up.normalized * (v2_lookInputs.x * x), Space.World);
        transform.eulerAngles = new Vector3(f_yLook, transform.eulerAngles.y, 0);

        transform.eulerAngles = Vector3.Scale(transform.eulerAngles, Vector3.one - Vector3.forward);

    }

    #endregion

    #region Public Voids

    internal void CancelInputs()
    {
        v2_lookInputs = Vector2.zero;
    }

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

    public void SetInvertedY(bool val)
    {
        v2_cameraSensitivity.y = Mathf.Abs(v2_cameraSensitivity.y) * (val ? -1 : 1);
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

    public void SetFollow(Transform _t_newFollow, bool _b_isSpectating)
    {
        t_follow = _t_newFollow;
        t_follow.GetComponentInChildren<Billboard>()?.gameObject.SetActive(false);
        FindObjectOfType<PauseMenuController>().SetSpectating();
        b_isSpectating = _b_isSpectating;
    }

    internal void StopSpectating()
    {
        if (b_isSpectating)
        {
            FindObjectOfType<PauseMenuController>().StopSpectating();
            b_isSpectating = false;
        }
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

    internal Camera ReturnCamera()
    {
        return c_cam;
    }

    #endregion

}
