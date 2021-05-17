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
    private Vector3 v_currentOffset;

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
            transform.localPosition = Vector3.Lerp(transform.localPosition, (v_targetPos.normalized * (hit.distance - 0.35f)) + v_currentOffset, 0.9f);
        else
            transform.localPosition = v_targetPos + v_currentOffset;
    }

    #endregion

    #region Private Voids


    #endregion

    #region Public Voids

    internal void CameraShake(float _f_severity, float _f_time, bool _b_onlyUp)
    {
        v_currentOffset += new Vector3(0/*Random.Range(-_f_severity, _f_severity)*/, Random.Range(_b_onlyUp ? 0 : -_f_severity, _f_severity), Random.Range(-_f_severity, _f_severity));        
        StartCoroutine(ReturnToNeutral(_f_time));
    }

    private IEnumerator ReturnToNeutral(float _f_time)
    {
        float _f = 0;
        while (_f < _f_time)
        {
            yield return new WaitForEndOfFrame();
            _f += Time.deltaTime;
            v_currentOffset = Vector3.Lerp(v_currentOffset, Vector3.zero, _f / _f_time);
        }
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
