using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopdogMover : MoverBase
{

    /*
     *  f_moveSpeed
     *  v_drag
     *  b_canMove
     *  hit
     *  v_groundNormal
     *  rb
     */
    [SerializeField] private float f_launchForce = 15f;
    private HopdogAnimator anim;

    private void Start()
    {
        anim = GetComponent<HopdogAnimator>();
    }

    public void Launch(Vector3 _targetPos)
    {
        if (!b_canMove)
            return;
        anim.SetLaunchAnims(true);
        b_canMove = false;
        Invoke(nameof(UnLaunch), 2);
        rb.AddForce((_targetPos - transform.position).normalized * f_launchForce, ForceMode.VelocityChange);

    }

    private void UnLaunch()
    {
        SetCanMove(true);
        anim.SetLaunchAnims(false);
    }

}
