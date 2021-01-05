using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviourPun
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

    private PlayerMover pm_mover;
    private PlayerInputManager pim_inputManager;


    private Transform camTransform;
    private Animator anim;
    private Rigidbody rb;
    private bool b_canShoot = true;
    private Sofa s_currentSofa;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        pm_mover = GetComponent<PlayerMover>();
        pim_inputManager = GetComponent<PlayerInputManager>();
    }

    private void Update()
    {
        GetMovementSpeed();
        CheckJumpAnims();
        SetShootingBools();

        if (Input.anyKeyDown)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("VibinOnSofaleft") || anim.GetCurrentAnimatorStateInfo(0).IsName("VibinOnSofaright"))
            {
                if (anim.GetBool("SofaLeft"))
                {
                    anim.SetBool("SofaLeft", false);
                    rb.isKinematic = false;
                    pm_mover.enabled = true;
                    s_currentSofa.EndSit();
                    s_currentSofa = null;
                    transform.parent = null;
                    GetComponent<Collider>().enabled = true;
                }
                if (anim.GetBool("SofaRight"))
                {
                    anim.SetBool("SofaRight", false);
                    rb.isKinematic = false;
                    pm_mover.enabled = true;
                    s_currentSofa.EndSit();
                    s_currentSofa = null;
                    transform.parent = null;
                    GetComponent<Collider>().enabled = true;
                }
            }
        }
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
        anim.SetBool("JumpUp", pm_mover.b_jumpPress);
        anim.SetBool("FlyingPose", !pm_mover.b_grounded);
    }

    private void GetMovementSpeed()
    {
        Vector3 vec = Vector3.Scale(rb.velocity, Vector3.one - Vector3.up);
        anim.SetFloat("X", Mathf.Lerp(anim.GetFloat("X"), pm_mover.v_movementVector.x * (pm_mover.b_sprintHold ? 2 : 1), Time.deltaTime * 4));
        anim.SetFloat("Y", Mathf.Lerp(anim.GetFloat("Y"), pm_mover.v_movementVector.z * (pm_mover.b_sprintHold ? 2 : 1), Time.deltaTime * 4));
        //anim.SetFloat("Y", pm_inputs.v_movementVector.z * (pm_inputs.b_sprintHold ? 2 : 1));
    }

    private void SetShootingBools()
    {
        if (b_canShoot)
        {
            anim.SetBool("ShootingLeft", pim_inputManager.GetToolBools().b_LToolHold);
            anim.SetBool("ShootingRight", pim_inputManager.GetToolBools().b_RToolHold);
        }
        else
        {
            anim.SetBool("ShootingLeft", false);
            anim.SetBool("ShootingRight", false);
        }
    }

    private void MakeAnArmDoTheRightThing(Transform arm, int fix)
    {
        arm.transform.rotation = camTransform.rotation;
        arm.Rotate(transform.up * fix, 90);
    }

    internal void DoSitDown(bool b_isRightSide, Sofa _s_newSofa)
    {
        s_currentSofa = _s_newSofa;
        photonView.RPC("RemoteSit", RpcTarget.Others, _s_newSofa.GetChairTrasnsform().forward);
        if (b_isRightSide)
            anim.SetBool("SofaRight", true);
        else
            anim.SetBool("SofaLeft", true);
    }

    [PunRPC]
    public void RemoteSit(Vector3 forward)
    {
        GetComponent<Rigidbody>().rotation = Quaternion.LookRotation(forward, Vector3.up);
        GetComponent<Rigidbody>().isKinematic = true;

    }

    #endregion

    #region Public Voids

    public void SetShootability(bool _b_shouldBeAbleToShoot)
    {
        b_canShoot = _b_shouldBeAbleToShoot;
    }

    public void SetCam(Transform cam)
    {
        camTransform = cam;
    }

    public void PlayerDied()
    {
        SetShootability(false);
        anim.SetTrigger("GetKnockedDown");
        anim.SetBool("GetRevived", false);
        anim.SetBool("Knockdown", true);
        pm_mover.SetDown(true);
    }

    public void PlayerRevived()
    {
        SetShootability(true);
        anim.SetBool("Knockdown", false);
        anim.SetBool("GetRevived", true);
        pm_mover.SetDown(false);
    }

    public void DoReviveAnim()
    {
        anim.SetBool("Revive", true);
        StartCoroutine(ReviveOffness());
    }

    public IEnumerator ReviveOffness()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Revive", false);
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
