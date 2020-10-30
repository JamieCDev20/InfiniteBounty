using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private Transform armR;
    [SerializeField] private Transform armL;

    #endregion

    #region Private

    private bool b_doIK = true; //Pronounced like palpatine says "Do it" but with a 'K' not a 'T'

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
    }

    private void Update()
    {
        GetMovementSpeed();
    }

    private void LateUpdate()
    {
        //MakeAnArmDoTheRightThing(armR, -1);
        //MakeAnArmDoTheRightThing(armL, 1);
    }

    #endregion

    #region Private Voids

    private void GetMovementSpeed()
    {
        Vector3 vec = Vector3.Scale(rb.velocity, Vector3.one - Vector3.up);
        anim.SetFloat("X", transform.InverseTransformDirection(vec).x * 10);
        anim.SetFloat("Y", transform.InverseTransformDirection(vec).z * 10);
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
