using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HandymanAI : AIBase
{
    private HandymanMover mover;

    [Header("Punch Attack")]
    [SerializeField] private float f_punchRange;
    private bool b_shouldBeThrowing;

    [Header("Throwing Stats")]
    [SerializeField] private float f_pickupRange;
    private GameObject go_currentThrowable;
    private GameObject go_nearestThrowable;
    private float f_throwTimer = 0;
    private bool b_hasThrowable;
    [SerializeField] private float f_throwCooldown;
    [SerializeField] private float f_timeBetweenThrows = 2;
    [SerializeField] private float f_minThrowDistance;
    [SerializeField] private GameObject go_centreofPickup;
    [SerializeField] private float f_throwWindup;

    private HandymanAnimator anim;



    #region Queries

    private bool IsInPunchRangeQuery()
    {
        return Vector3.Distance(transform.position, t_target.position) <= f_punchRange;
    }

    private bool IsInPickUpRangeQuery()
    {
        return Vector3.Distance(transform.position, go_nearestThrowable.transform.position) <= f_pickupRange;
    }

    private bool HasThrowableObjectQuery()
    {
        return b_hasThrowable;
    }

    private bool ShouldBeThrowingQuery()
    {
        return b_shouldBeThrowing;
    }

    #endregion

    #region Check Actions

    private GameObject GetClosestTaggedObjectAction(string _s_tag, bool _b_ignoreHieght)
    {
        float _f_distance = 1000000000;
        GameObject go_object = null;

        foreach (GameObject item in TagManager.x.GetTagSet(_s_tag))
        {
            if (Mathf.Abs(item.transform.position.y - transform.position.y) > 10 && !_b_ignoreHieght)
                continue;
            float _f_distanceCheck = Vector3.SqrMagnitude(item.transform.position - transform.position);
            if (_f_distanceCheck < _f_distance)
            {
                go_object = item;
                _f_distance = _f_distanceCheck;
            }
        }
        return go_object;
    }

    private void GetClosestPlayerAction()
    {
        t_target = GetClosestTaggedObjectAction("Player", true).transform;
    }

    private void GetClosestThrowableObjectAction()
    {
        go_nearestThrowable = GetClosestTaggedObjectAction("Throwable", false);
    }

    private void CheckShouldBeThrowing()
    {
        float throwableDistance = Vector3.SqrMagnitude(transform.position - go_nearestThrowable.transform.position);
        float targetDistance = Vector3.SqrMagnitude(transform.position - t_target.position);
        bool playerFurther = throwableDistance < targetDistance;
        b_shouldBeThrowing = playerFurther && throwableDistance < (f_minThrowDistance * f_minThrowDistance);
    }


    #endregion

    #region Actions

    private void MoveTowardsAction()
    {
        if (b_shouldBeThrowing)
            mover.Move((go_nearestThrowable.transform.position - transform.position).normalized);
        else
            mover.Move((t_target.position - transform.position).normalized);
    }

    private void PickUpAction()
    {
        toggleHurtboxes(false);
        go_nearestThrowable.GetComponent<Rigidbody>().isKinematic = true;

        //go_nearestThrowable.GetComponent<Collider>().enabled = false;
        MoverBase m = go_nearestThrowable.GetComponent<MoverBase>();
        if (m != null)
            m.SendMessage("SetCanMove", false);

        go_nearestThrowable.transform.parent = go_centreofPickup.transform;
        go_nearestThrowable.transform.localPosition = Vector3.zero;
        go_currentThrowable = go_nearestThrowable;
        go_currentThrowable.GetComponent<Collider>().enabled = false;
        b_hasThrowable = true;
    }

    private void ThrowAction()
    {
        if (f_throwTimer <= 0)
        {
            mover.SetCanMove(false);
            StartCoroutine(IThrow());
        }
        f_throwTimer -= Time.deltaTime;
    }

    private IEnumerator IThrow()
    {
        Rigidbody _rb = go_nearestThrowable.GetComponent<Rigidbody>();
        Collider _c = go_nearestThrowable.GetComponent<Collider>();
        _c.enabled = false;
        anim.Throw();
        f_throwTimer = f_throwCooldown;

        yield return new WaitForSeconds(f_throwWindup);
        go_currentThrowable.GetComponent<Throwable>().EnterAboutToBeThrownState();
        go_currentThrowable.transform.parent = null;
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.None;
        _rb.AddForce(GetThrowVector(t_target.transform.position + (Vector3.up * 5)), ForceMode.Impulse);
        _rb.AddTorque(new Vector3(Random.value, Random.value, Random.value) * Random.Range(5, 10));

        f_throwTimer = f_throwCooldown;
        yield return new WaitForSeconds(0.2f);
        mover.SetCanMove(true);
        _c.enabled = true;

        yield return new WaitForSeconds(1);
        go_nearestThrowable = null;
        go_currentThrowable = null;
        b_hasThrowable = false;
        b_hasThrowable = false;
    }

    private void PunchAction()
    {
        //toggleHurtboxes(true);
        anim.Slap();
        StartCoroutine(PunchRoutine());

    }

    IEnumerator PunchRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        toggleHurtboxes(true);
        yield return new WaitForSeconds(0.3f);
        toggleHurtboxes(false);
    }

    #endregion

}