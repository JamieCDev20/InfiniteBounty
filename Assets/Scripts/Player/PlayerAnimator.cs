using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private Transform armR;
    [SerializeField] private Transform armL;
    [Space]
    [Header("Tester")]
    [SerializeField] private bool doDemoIK = true;

    #endregion

    #region Private

    private bool b_doIK = true; //Pronounced like palpatine says "Do it" but with a 'K' not a 'T'

    private PlayerMover pm_inputs;
    private Transform camTransform;
    private Animator anim;
    private Rigidbody rb;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        pm_inputs = GetComponent<PlayerMover>();
    }

    private void Update()
    {
        GetMovementSpeed();
        CheckJumpAnims();
    }

    private void LateUpdate()
    {
        if (doDemoIK)
        {
            MakeAnArmDoTheRightThing(armR, -1);
            MakeAnArmDoTheRightThing(armL, 1);

        }
    }

    #endregion

    #region Private Voids

    private void CheckJumpAnims()
    {
        anim.SetBool("JumpUp", pm_inputs.b_jumpPress);
        anim.SetBool("FlyingPose", !pm_inputs.b_grounded);
    }

    private void GetMovementSpeed()
    {
        Vector3 vec = Vector3.Scale(rb.velocity, Vector3.one - Vector3.up);
        anim.SetFloat("X", Mathf.Lerp(anim.GetFloat("X"), pm_inputs.v_movementVector.x * (pm_inputs.b_sprintHold ? 2 : 1), Time.deltaTime * 4));
        anim.SetFloat("Y", Mathf.Lerp(anim.GetFloat("Y"), pm_inputs.v_movementVector.z * (pm_inputs.b_sprintHold ? 2 : 1), Time.deltaTime * 4));
        //anim.SetFloat("Y", pm_inputs.v_movementVector.z * (pm_inputs.b_sprintHold ? 2 : 1));
    }

    private void MakeAnArmDoTheRightThing(Transform arm, int fix)
    {
        arm.transform.rotation = camTransform.rotation;
        arm.Rotate(transform.up * fix, 90);
    }

    #endregion

    #region Public Voids

    public void SetCam(Transform cam)
    {
        camTransform = cam;
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
