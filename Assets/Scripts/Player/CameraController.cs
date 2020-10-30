using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    //Variables
    #region Serialised


    #endregion

    #region Private

    private Vector2 v2_lookInputs;
    private Vector3 v_offset = Vector3.up * 3;
    private Transform t_follow;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        Detach();
    }

    private void LateUpdate()
    {
        //DoStuff
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
