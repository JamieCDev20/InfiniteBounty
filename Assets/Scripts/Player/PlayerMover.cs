﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMover : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private bool b_networked = false;

    [Header("Motion Variables")]
    [SerializeField] private float f_shipMoveSpeed = 10; // How fast the player moves on the ship
    [SerializeField] private float f_planetMoveSpeed = 20; // How fast the player moves on the planet
    [SerializeField] private float f_shipSprintMult = 2; // The multiplier applied to the base speed whilst sprinting on the ship;
    [SerializeField] private float f_planetSprintMult = 3; // The multiplier applied to the base speed whilst sprinting on the planet;
    [SerializeField] private float f_downMult = 0.3f; // The multiplier applied to the base speed whilst down;
    [SerializeField] private float f_jumpForce = 10; // How high the player jumps
    [SerializeField] private float f_jumpDelay = 0.15f; //How long after the initial input the jump occurs;
    [SerializeField] private float f_coyoteTime = 0.2f;

    [Space]
    [Header("Physics")]
    [SerializeField] private float f_gravityScale = 10; // The force of gravity on the player
    [SerializeField] private float f_plummetPoint = 5; //the velocity at which below, gravity will increase
    [SerializeField] private float f_plummetMultiplier = 3; //the velocity at which below, gravity will increase
    [SerializeField] private Vector3 v_dragVector = (Vector3.one - Vector3.up) * 0.1f; // The rate at which the player slows down in each axis direction

    [Header("Getting Teleported")]
    [SerializeField] private GameObject go_characterMesh;
    [SerializeField] private ParticleSystem p_goopyParticle;

    #endregion

    #region Private

    internal bool b_grounded;
    private bool b_applyDrag = true;
    private bool b_down;
    private bool b_applyGravity;
    internal bool b_jumpPress;
    private float f_lastOnGround;
    private float f_currentMoveSpeed;
    private float f_currentMultiplier;
    private bool b_jumpHold;
    private bool b_jumpUp;
    private bool b_sprintPress;
    internal bool b_sprintHold;
    private bool b_sprintUp;
    private Surface s_currentSurface;
    internal Vector3 v_movementVector; // The direction the player is inputting
    private Vector3 v_startPos;
    private Vector3 v_groundNormal;
    private Rigidbody rb;
    private Transform t_camTransform;
    private PhotonView view;
    private FootstepAudioPlayer fap_audio;
    internal bool b_isSitting;
    private bool b_knockedback;
    private Animator anim;    

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        Init();
        SceneManager.sceneLoaded += SceneChange;
    }

    private void Update()
    {
        b_grounded = CheckGrounded();
        anim.SetBool("Grounded", b_grounded);
        if (!view.IsMine && b_networked)
            return;
        Jump();
        ResetIfOffMap();
        //Debug.Log($"players Velocity is: {rb.velocity.magnitude}");
    }

    private void FixedUpdate()
    {

        ApplyGravity();

        ApplyDrag();

        if (!view.IsMine && b_networked)
            return;

        HandleAllInputs();

        ApplyMovement();

    }

    #endregion

    #region Private Voids

    /// <summary>
    /// Initialising player variables
    /// </summary>
    private void Init()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        v_startPos = new Vector3(-8, 1, -4);
        SetMoveSpeeds(true);
        view = GetComponent<PhotonView>();
        fap_audio = GetComponentInChildren<FootstepAudioPlayer>();

    }

    private void HandleAllInputs()
    {

    }

    /// <summary>
    /// Applying the players movement to them
    /// </summary>
    private void ApplyMovement()
    {
        Vector3 dir = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(t_camTransform.TransformDirection(v_movementVector), Vector3.up), v_groundNormal);
        if (v_movementVector.sqrMagnitude > 0.25f)
        {
            rb.AddForce(dir.normalized * f_currentMoveSpeed * Time.deltaTime * (b_down ? f_downMult : (b_sprintHold ? f_currentMultiplier : 1)), ForceMode.Impulse);
            transform.forward = Vector3.Lerp(transform.forward, Vector3.Scale(t_camTransform.forward, Vector3.one - Vector3.up), 0.1f); //Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.2f);
        }
    }

    /// <summary>
    /// Applying the players jump
    /// </summary>
    private void Jump()
    {
        if (!b_jumpPress || b_down)
            return;
        if ((Time.realtimeSinceStartup - f_lastOnGround) < f_coyoteTime)
        {
            f_lastOnGround -= 10;
            Vector3 t = rb.velocity;
            t.y = f_jumpForce;
            rb.velocity = t;
        }
    }

    private IEnumerator DelayedJump()
    {
        yield return new WaitForSeconds(f_jumpDelay);
        rb.AddForce(Vector3.up * f_jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Dragging the players velocity in each axis
    /// </summary>
    private void ApplyDrag()
    {
        if (b_applyDrag)
            rb.velocity = Vector3.Scale(rb.velocity, Vector3.one - v_dragVector);
    }

    /// <summary>
    /// Applying gravity to the player
    /// </summary>
    private void ApplyGravity()
    {
        if (b_applyGravity)
            rb.velocity -= Vector3.up * f_gravityScale * (rb.velocity.y < f_plummetPoint ? f_plummetMultiplier : 1) * Time.deltaTime;
        else
            rb.velocity -= Vector3.up * 1f * Time.deltaTime;
    }

    private void ResetIfOffMap()
    {
        if (transform.position.y < -25 && view.IsMine)
        {
            transform.position = v_startPos + (Vector3.up);// * 5);                      
        }
    }

    internal void SetHUDController(HUDController _HUDController)
    {
        GetComponent<PlayerHealth>().hudControl = _HUDController;
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void Landed()
    {
        fap_audio.PlayLandingSound();
    }

    private void SceneChange(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Lobby"))
        {
            fap_audio.ChangeSurfaceEnum(Surface.ship);
        }
        else
        {
            fap_audio.ChangeSurfaceEnum(Surface.planet);
        }
    }

    #endregion

    #region Public Voids

    /// <summary>
    /// Update the direction that the player is trying to move
    /// </summary>
    /// <param name="_v_newMovementVector"></param>
    public void UpdateInputVector(Vector3 _v_newMovementVector)
    {
        v_movementVector = _v_newMovementVector;
    }

    /// <summary>
    /// Updates jump bools based on player pressing and holding the buttons
    /// </summary>
    /// <param name="_b_press"></param>
    /// <param name="_b_hold"></param>
    public void UpdateJumpBools(bool _b_press, bool _b_hold, bool _b_up)
    {
        b_jumpPress = _b_press;
        b_jumpHold = _b_hold;
        b_jumpUp = _b_up;
    }

    /// <summary>
    /// Updates sprint bools based on player pressing and holding the buttons
    /// </summary>
    /// <param name="_b_press"></param>
    /// <param name="_b_hold"></param>
    /// <param name="_b_up"></param>
    public void UpdateSprintBools(bool _b_press, bool _b_hold, bool _b_up)
    {
        b_sprintPress = _b_press;
        b_sprintHold = _b_hold;
        b_sprintUp = _b_up;
    }

    public void SetCameraTranfsorm(Transform _t_camTransform)
    {
        t_camTransform = _t_camTransform;
    }

    public void SetDown(bool _b_val)
    {
        b_down = _b_val;
    }

    public void SetMoveSpeeds(bool onShip)
    {
        if (onShip)
        {
            f_currentMoveSpeed = f_shipMoveSpeed;
            f_currentMultiplier = f_shipSprintMult;
        }
        else
        {
            f_currentMoveSpeed = f_planetMoveSpeed;
            f_currentMultiplier = f_planetSprintMult;
        }
    }

    public void HitKnockback(Vector3 _dir, float _force)
    {
        view.RPC(nameof(HitKnockbackRPC), RpcTarget.All, _dir, _force);
    }

    [PunRPC]
    public void HitKnockbackRPC(Vector3 _dir, float _force)
    {
        if (view.IsMine)
        {
            transform.forward = -_dir;
            rb.AddForce(_dir.normalized * _force, ForceMode.Impulse);
            StartCoroutine(ApplyDragDelay());
        }
    }

    IEnumerator ApplyDragDelay()
    {
        b_knockedback = true;
        b_applyDrag = false;
        yield return new WaitForSeconds(0.5f);
        b_applyDrag = true;
        b_knockedback = false;
    }


    internal void GetTeleported()
    {
        view.RPC(nameof(TeleportRPC), RpcTarget.All);
    }

    [PunRPC]
    public IEnumerator TeleportRPC()
    {
        enabled = false;
        go_characterMesh.SetActive(false);
        p_goopyParticle.Play();

        yield return new WaitForSeconds(0.3f);

        p_goopyParticle.Stop();
        go_characterMesh.SetActive(true);
        enabled = true;
    }


    #endregion

    #region Private Returns

    private bool CheckGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hit, 0.5f);
        if (hit.collider != null)
        {
            b_applyGravity = hit.distance > 0.15f;
            v_groundNormal = hit.normal;
            f_lastOnGround = Time.realtimeSinceStartup;

            ISurfacable iS = hit.collider.GetComponent<ISurfacable>();
            if (iS != null)
                s_currentSurface = iS.GetSurface();

            if (!b_grounded)
                Landed();

            return true;

        }
        v_groundNormal = Vector3.up;
        b_applyGravity = true;
        return false;
    }
    #endregion

    #region Public Returns

    public bool IsGrounded()
    {
        return b_grounded;
    }

    public bool IsKnockedBack()
    {
        return b_knockedback;
    }

    #endregion

}
