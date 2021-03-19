using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimator : MonoBehaviourPun
{

    //Variables
    #region Serialised

    [SerializeField] private Transform armR;
    [SerializeField] private Transform armL;
    [SerializeField] private Transform chest;
    [SerializeField] private Transform chestRef;
    [SerializeField] private Transform spine;
    [SerializeField] private Transform spineRef;
    [SerializeField] private Transform stomach;
    [SerializeField] private Transform stomachRef;
    [SerializeField] private Transform hips;
    [SerializeField] private float f_spineWeight = 0.55f;
    [SerializeField] private float f_stomachWeight = 0.2f;
    [SerializeField] private float f_maxBodyAngle = 80;
    [SerializeField] private float f_maxArmAngle = 80;
    [SerializeField] private float f_armOffset = -20;
    [Space]
    [Header("Tester")]
    [SerializeField] private bool doDemoIK = true;

    #endregion

    #region Private

    private bool b_doIK = true; //Pronounced like palpatine says "Do it" but with a 'K' not a 'T'

    private PlayerMover pm_mover;
    private PlayerInputManager pim_inputManager;
    private ToolHandler tHandler;
    private Transform camTransform;
    private Animator anim;
    private Rigidbody rb;
    private bool b_canShoot = true;
    private Sofa s_currentSofa;
    private bool b_canWalk = true;
    private Vector3 v_posLastFrame;
    internal bool b_isSprinting;
    private PlayerNetworkSync pns;
    private Vector3 v_vel;
    private bool b_rShootLeft;
    private bool b_rShootRight;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        pm_mover = GetComponent<PlayerMover>();
        pim_inputManager = GetComponent<PlayerInputManager>();
        tHandler = GetComponent<ToolHandler>();
        try
        {
            doDemoIK = NetworkedPlayer.x.GetPlayer() == transform;

        }
        catch
        {
            doDemoIK = false;
        }
        pns = GetComponent<PlayerNetworkSync>();
    }

    private void Update()
    {
        if (photonView.IsMine)
            b_isSprinting = pm_mover.b_sprintHold;
        else
            b_isSprinting = pns.GetIsSprinting();

        anim.SetBool("Knockback", pm_mover.IsKnockedBack());

        GetMovementSpeed();
        CheckJumpAnims();
        SetShootingBools();

        if (s_currentSofa != null)
        {
            if (Input.anyKeyDown)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("PoopCycle") || anim.GetCurrentAnimatorStateInfo(0).IsName("VibinOnSofaright"))
                {
                    if (anim.GetBool("Poop"))
                    {
                        EndSitAnim("Poop");
                    }
                    if (anim.GetBool("SofaRight"))
                    {
                        EndSitAnim("SofaRight");
                    }
                }
            }
        }
        else
        {
            EndSitAnim("Poop");
            EndSitAnim("SofaRight");
        }
        v_posLastFrame = transform.position;
    }

    private void LateUpdate()
    {
        if (doDemoIK)
        {
            //MakeAnArmDoTheRightThing(armR, -1);
            //MakeAnArmDoTheRightThing(armL, 1);
            if (anim.GetBool("ShootingLeft") || anim.GetBool("ShootingRight"))
            {
                ApplyBodyRotation();
                ArmUpDownRespect(armL, anim.GetBool("ShootingLeft"));
                ArmUpDownRespect(armR, anim.GetBool("ShootingRight"));
            }
        }
    }

    private void EndSitAnim(string n)
    {
        anim.SetBool(n, false);
        rb.isKinematic = false;
        pm_mover.enabled = true;
        s_currentSofa?.EndSit();
        s_currentSofa = null;
        transform.parent = null;
        GetComponent<Collider>().enabled = true;
    }

    private void SceneLoaded(Scene s, LoadSceneMode m)
    {
        s_currentSofa = null;
    }

    public void SetRemoteShooting(bool l, bool r)
    {
        b_rShootLeft = l;
        b_rShootRight = r;
    }

    public void SetRemoteVelocity(Vector3 _vel)
    {
        v_vel = _vel;
    }

    #endregion

    #region Private Voids

    private void CheckJumpAnims()
    {
        anim.SetBool("JumpUp", pm_mover.b_jumpPress);
        anim.SetBool("FlyingPose", !pm_mover.b_grounded);
        if (pm_mover.b_grounded)
            anim.SetBool("LavaHit", !pm_mover.b_grounded);
    }

    private void GetMovementSpeed()
    {
        if (b_canWalk)
        {
            if (photonView.IsMine)
                v_vel = rb.velocity;

            anim.SetFloat("X", Mathf.Lerp(anim.GetFloat("X"), transform.InverseTransformDirection(v_vel).x / 7, 0.3f));
            anim.SetFloat("Y", Mathf.Lerp(anim.GetFloat("Y"), transform.InverseTransformDirection(v_vel).z / 7, 0.3f));


            if (rb.velocity.sqrMagnitude > 0.1f)
                return;
            if (Mathf.Abs(anim.GetFloat("Y")) < 0.05f)
                anim.SetFloat("Y", 0);
            if (Mathf.Abs(anim.GetFloat("X")) < 0.05f)
                anim.SetFloat("X", 0);
            //anim.SetFloat("Y", pm_inputs.v_movementVector.z * (pm_inputs.b_sprintHold ? 2 : 1));
        }
    }

    internal void StopWalking()
    {
        b_canWalk = false;
        anim.SetFloat("X", 0);
        anim.SetFloat("Y", 0);
    }
    internal void StartWalking()
    {
        b_canWalk = true;
    }

    private void SetShootingBools()
    {
        if (b_canShoot)
        {
            if (photonView.IsMine)
            {
                anim.SetBool(tHandler.GetTool((int)ToolSlot.leftHand) != 4 ? "ShootingLeft" : "HammerLeft", pim_inputManager.GetToolBools().b_LToolHold);
                anim.SetBool(tHandler.GetTool((int)ToolSlot.rightHand) != 4 ? "ShootingRight" : "HammerRight", pim_inputManager.GetToolBools().b_RToolHold);
            }
            else
            {
                anim.SetBool("ShootingLeft", b_rShootLeft);
                anim.SetBool("ShootingRight", b_rShootRight);
            }
        }
        else
        {
            anim.SetBool("ShootingLeft", false);
            anim.SetBool("ShootingRight", false);
        }
    }

    private void ApplyBodyRotation()
    {

        float ang = Mathf.Clamp(Vector3.Angle(Vector3.ProjectOnPlane(camTransform.forward, hips.up), hips.forward), -f_maxBodyAngle, f_maxBodyAngle);

        float side = Mathf.Sign(Vector3.Dot(camTransform.transform.forward, hips.right));

        stomach.rotation = Quaternion.AngleAxis(ang * f_stomachWeight * side, stomachRef.up) * stomach.rotation;
        spine.rotation = Quaternion.AngleAxis(ang * (f_spineWeight - f_stomachWeight) * side, spineRef.up) * spine.rotation;
        chest.rotation = Quaternion.AngleAxis(ang * (1 - (f_spineWeight + f_stomachWeight)) * side, chestRef.up) * chest.rotation;

    }

    private void ArmUpDownRespect(Transform _arm, bool _active)
    {
        if (!_active)
            return;

        float ang = Mathf.Clamp(Vector3.Angle(Vector3.ProjectOnPlane(camTransform.forward, hips.right), hips.forward), -f_maxArmAngle, f_maxArmAngle);
        float side = Mathf.Sign(Vector3.Dot(camTransform.transform.forward, -hips.up));

        Vector3 temp = _arm.localEulerAngles;
        temp.z += (ang * side) + f_armOffset;
        _arm.localEulerAngles = temp;

    }

    internal void DoSitDown(bool b_isRightSide, Sofa _s_newSofa)
    {
        s_currentSofa = _s_newSofa;
        photonView.RPC("RemoteSit", RpcTarget.Others, _s_newSofa.GetChairTrasnsform().forward);
        if (b_isRightSide)
            anim.SetBool("Poop", true);
        else
            anim.SetBool("Poop", true);
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

    [PunRPC]
    public void RemoteDied()
    {
        anim.SetTrigger("GetKnockedDown");
        anim.SetBool("GetRevived", false);
        anim.SetBool("Knockdown", true);
    }

    public void PlayerDied()
    {
        SetShootability(false);
        anim.SetTrigger("GetKnockedDown");
        anim.SetBool("GetRevived", false);
        anim.SetBool("Knockdown", true);
        pm_mover.SetDown(true);
        photonView.RPC("RemoteDied", RpcTarget.Others);
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
