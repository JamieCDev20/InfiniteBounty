using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandymanMover : MoverBase
{

    [SerializeField] private LayerMask lm_wallCheckMask;

    public override void Move(Vector3 _dir)
    {

        string check = "";

        Color c = Color.red;

        if (Physics.Raycast(transform.position + (Vector3.up * 1.5f), transform.forward, 6f, jumpMask, QueryTriggerInteraction.Ignore))
        {
            c = Color.green;
            if (Time.realtimeSinceStartup - lastJumped > 1.5f)
            {
                rb.AddForce(Vector3.up * f_jumpForce, ForceMode.Impulse);
                lastJumped = Time.realtimeSinceStartup;

            }
        }
        Debug.DrawRay(transform.position + (Vector3.up * 1.5f), transform.forward * 4f, c);

        if (!Physics.Raycast(transform.position + Vector3.up, _dir, 5, lm_wallCheckMask))
        {
            base.Move(_dir);
            return;
        }

        check += Physics.Raycast(transform.position + Vector3.up, Quaternion.AngleAxis(-45, -Vector3.up) * _dir, 5, lm_wallCheckMask) ? "1" : "0";
        check += Physics.Raycast(transform.position + Vector3.up, Quaternion.AngleAxis(45, -Vector3.up) * _dir, 5, lm_wallCheckMask) ? "1" : "0";

        int state = Convert.ToInt32(check, 2);

        switch (state)
        {
            case 1:
                _dir = Quaternion.AngleAxis(45, -Vector3.up) * _dir;
                break;
            case 2:
                _dir = Quaternion.AngleAxis(-45, -Vector3.up) * _dir;
                break;
            default:
                _dir = Quaternion.AngleAxis(45 * (((int)UnityEngine.Random.Range(0, 2) * 2) - 1), -Vector3.up) * _dir;
                break;
        }

        base.Move(_dir);

    }

}
