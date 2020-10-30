﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{

    //Variables
    #region Serialised

    [Header("Motion Variables")]
    [SerializeField] private float f_moveSpeed = 10; // How fast the player moves
    [SerializeField] private float f_jumpForce = 10; // How high the player jumps

    [Space]
    [Header("Physics")]
    [SerializeField] private float f_gravityScale = 10; // The force of gravity on the player
    [SerializeField] private float f_plummetPoint = 5; //the velocity at which below, gravity will increase
    [SerializeField] private Vector3 v_dragVector = (Vector3.one - Vector3.up) * 0.1f; // The rate at which the player slows down in each axis direction

    #endregion

    #region Private

    private bool b_jumpPress;
    private bool b_jumpHold;
    private bool b_jumpUp;
    private bool b_sprintPress;
    private bool b_sprintHold;
    private bool b_sprintUp;
    private Vector3 v_movementVector; // The direction the player is inputting
    private Rigidbody rb;
    private Transform t_camTransform;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {

        HandleAllInputs();

        ApplyMovement();

        ApplyGravity();

        ApplyDrag();

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
    }


    private void HandleAllInputs()
    {
        v_movementVector = v_movementVector.normalized;
    }

    /// <summary>
    /// Applying the players movement to them
    /// </summary>
    private void ApplyMovement()
    {
        rb.AddForce(v_movementVector * f_moveSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    /// <summary>
    /// Applying the players jump
    /// </summary>
    private void Jump()
    {
        if (!b_jumpPress)
            return;
        if (CheckGrounded())
            rb.AddForce(Vector3.up * f_jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Dragging the players velocity in each axis
    /// </summary>
    private void ApplyDrag()
    {
        rb.velocity = Vector3.Scale(rb.velocity, Vector3.one - v_dragVector);
    }

    /// <summary>
    /// Applying gravity to the player
    /// </summary>
    private void ApplyGravity()
    {
        rb.velocity -= Vector3.up * f_gravityScale * (rb.velocity.y < f_plummetPoint ? 2 : 1) * Time.deltaTime;
    }

    private bool CheckGrounded()
    {
        return true;
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

    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}
