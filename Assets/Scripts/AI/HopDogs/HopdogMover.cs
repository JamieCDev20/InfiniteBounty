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

    public void Launch(Vector3 _targetPos)
    {
        if (!b_canMove)
            return;
        Debug.Log("Launched");
        b_canMove = false;
        Invoke(nameof(SetCanMove), 2);
        rb.AddForce((_targetPos - transform.position).normalized * f_launchForce, ForceMode.VelocityChange);

    }

}
