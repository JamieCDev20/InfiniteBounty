using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : MonoBehaviourPun
{

    [SerializeField] protected float f_summoningSickness = 1;
    [SerializeField] protected float f_spottingDistance = 50;
    [SerializeField] protected LayerMask lm_spottingMask;

    protected float f_timeStarted;
    protected float f_targetFindLimiter = 0;
    protected BehaviourTree tree;
    protected Transform t_target;

    #region Queries

    public bool IsOverSummoningSickness()
    {
        return (Time.realtimeSinceStartup - f_timeStarted) > f_summoningSickness;
    }

    protected bool CanSeeTransform(Transform _targ)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, _targ.position - transform.position, out hit, f_spottingDistance))
        {
            return hit.collider.transform == _targ;
        }
        return false;
    }

    public bool StillHasTarget()
    {
        if (t_target != null)
        {
            bool canSee = CanSeeTransform(t_target);
            if (canSee)
                return canSee;
            else
                t_target = null;
        }
        return false;
    }

    public bool CanSeeAPlayer()
    {
        foreach (PlayerHealth ph in FindObjectsOfType<PlayerHealth>())
        {
            if (!ph.IsDead())
            {
                if (CanSeeTransform(ph.transform))
                {
                    t_target = ph.transform;
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region Actions

    public void DebugTarget()
    {
        Debug.Log($"Target: {t_target}");
    }

    public void GetTargetAction()
    {
        f_targetFindLimiter += Time.deltaTime;
        t_target = null;
        if (f_targetFindLimiter <= 1)
            return;

        f_targetFindLimiter = 0;

        t_target = FindObjectOfType<PlayerInputManager>().transform;

    }

    #endregion

}
