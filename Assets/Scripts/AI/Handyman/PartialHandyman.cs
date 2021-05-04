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
    private GameObject go_nearestThrowable;
    private bool b_hasThrowable;
    private float f_throwCooldown;
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

    private GameObject GetClosestTaggedObjectAction(string _s_tag)
    {
        float _f_distance = 1000000000;
        GameObject go_object = null;

        foreach (GameObject item in TagManager.x.GetTagSet(_s_tag))
        {
            float _f_distanceCheck = Vector3.SqrMagnitude(item.transform.position - transform.position);
            if (_f_distanceCheck < _f_distance)
            {
                go_object = item;
                _f_distance = _f_distanceCheck;
            }
        }
        Debug.Log(go_object);
        return go_object;
    }

    private void GetClosestPlayerAction()
    {
        t_target = GetClosestTaggedObjectAction("Player").transform;
    }

    private void GetClosestThrowableObjectAction()
    {
        go_nearestThrowable = GetClosestTaggedObjectAction("Throwable");
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
        b_hasThrowable = true;
    }

    private void ThrowAction()
    {
        if (f_throwWindup <= 0)
        {
            mover.SetCanMove(false);
            StartCoroutine(IThrow());
        }
        f_throwWindup -= Time.deltaTime;
    }

    private IEnumerator IThrow()
    {
        Rigidbody _rb = go_nearestThrowable.GetComponent<Rigidbody>();
        Collider _c = go_nearestThrowable.GetComponent<Collider>();
        _c.enabled = false;
        anim.Throw();
        f_throwCooldown = f_throwWindup;

        yield return new WaitForSeconds(f_throwWindup);
        go_nearestThrowable.GetComponent<Throwable>().EnterAboutToBeThrownState();
        go_nearestThrowable.transform.parent = null;
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.None;
        _rb.AddForce(GetThrowVector(t_target.transform.position), ForceMode.Impulse);
        _rb.AddTorque(new Vector3(Random.value, Random.value, Random.value) * Random.Range(5, 10));


        f_throwCooldown = f_throwWindup;
        b_hasThrowable = false;
        yield return new WaitForSeconds(0.1f);
        mover.SetCanMove(true);
        _c.enabled = true;
        
        yield return new WaitForSeconds(1);
        go_nearestThrowable = null;
    }

    private void PunchAction()
    {
        //toggleHurtboxes(true);
        anim.Slap();


    }

    #endregion

}