using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRespectWalls : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private LayerMask lm_playerMask;

    #endregion

    #region Private

    private float f_targetDistance;
    private Vector3 v_targetPos;
    private Transform t_root;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        v_targetPos = transform.localPosition;
        t_root = transform.root;
        f_targetDistance = v_targetPos.magnitude;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(t_root.position, t_root.TransformPoint(v_targetPos) - t_root.position, out hit, f_targetDistance, lm_playerMask))
            transform.localPosition = Vector3.Lerp(transform.localPosition, v_targetPos.normalized * (hit.distance - 0.35f), 0.9f);
        else
            transform.localPosition = v_targetPos;
    }

    #endregion

    #region Private Voids


    #endregion

    #region Public Voids


    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
