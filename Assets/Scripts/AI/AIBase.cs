using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

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

    public virtual bool IsOverSummoningSickness()
    {
        return (Time.realtimeSinceStartup - f_timeStarted) > f_summoningSickness;
    }

    protected bool CanSeeTarget()
    {
        return CanSeeTransform(t_target);
    }

    protected bool CanSeeTransform(Transform _targ)
    {
        Profiler.BeginSample("Can See Specific");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, _targ.position - transform.position, out hit, f_spottingDistance))
        {
            Profiler.EndSample();
            return hit.collider.transform == _targ;
        }
        Profiler.EndSample();
        return false;
    }

    public bool StillHasTarget()
    {

        Profiler.BeginSample("Still has");
        if (t_target != null)
        {
            bool canSee = CanSeeTransform(t_target);
            if (canSee)
            {
                Profiler.EndSample();
                return canSee;
            }
            else
                t_target = null;
        }
        Profiler.EndSample();
        return false;
    }

    public bool CanSeeAPlayer()
    {
        Profiler.BeginSample("Can See Any");
        foreach (PlayerHealth ph in FindObjectsOfType<PlayerHealth>())
        {
            if (!ph.IsDead())
            {
                if (CanSeeTransform(ph.transform))
                {
                    t_target = ph.transform;
                    Profiler.EndSample();
                    return true;
                }
            }
        }
        Profiler.EndSample();
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

        Profiler.BeginSample("Get Target");
        f_targetFindLimiter += Time.deltaTime;
        t_target = null;
        if (f_targetFindLimiter <= 1)
        {

            Profiler.EndSample();
            return;
        }

        f_targetFindLimiter = 0;

        t_target = FindObjectOfType<PlayerInputManager>().transform;

    }

    #endregion

}
