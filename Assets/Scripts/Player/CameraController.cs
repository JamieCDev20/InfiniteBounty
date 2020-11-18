using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private float f_cameraSensitivity = 180;
    [SerializeField] private bool networkedCamera = false;

    [Header("Firing Cam Positions")]
    [SerializeField] private Vector3 v_firingLeftOffset;
    [SerializeField] private Vector3 v_firingRightOffset;
    [SerializeField] private Vector3 v_firingBothOffset;

    #endregion

    #region Private

    private Vector2 v2_lookInputs;
    private Vector3 v_offset = Vector3.up * 3;
    private Transform t_follow;
    private PlayerInputManager pim_inputs;

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
        Follow();
        Look();
    }

    #endregion

    #region Private Voids

    private void Detach()
    {
        transform.parent = null;
    }

    private void Follow()
    {
        if (pim_inputs.GetToolBools().b_LToolHold)
            transform.position = t_follow.position + v_firingLeftOffset;

        else if (pim_inputs.GetToolBools().b_RToolHold)
            transform.position = t_follow.position + v_firingRightOffset;

        else if (pim_inputs.GetToolBools().b_LToolHold && pim_inputs.GetToolBools().b_RToolHold)
            transform.position = t_follow.position + v_firingBothOffset;

        else
            transform.position = t_follow.position + v_offset;
    }

    private void Look()
    {
        transform.Rotate(transform.right.normalized * v2_lookInputs.y * Time.deltaTime * f_cameraSensitivity, Space.World);
        transform.Rotate(transform.up.normalized * v2_lookInputs.x * Time.deltaTime * f_cameraSensitivity, Space.World);
        transform.eulerAngles = Vector3.Scale(transform.eulerAngles, Vector3.one - Vector3.forward);
    }

    #endregion

    #region Public Voids

    public void SetLookInput(Vector2 _v2_newLookInput)
    {
        v2_lookInputs = _v2_newLookInput;
    }

    public void SetFollow(Transform _t_newFollow)
    {
        t_follow = _t_newFollow;
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

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
