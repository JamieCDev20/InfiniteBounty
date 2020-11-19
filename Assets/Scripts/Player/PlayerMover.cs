using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private bool b_networked = false;

    [Header("Motion Variables")]
    [SerializeField] private float f_moveSpeed = 10; // How fast the player moves
    [SerializeField] private float f_sprintMult = 2; // The multiplier applied to the base speed whilst sprinting;
    [SerializeField] private float f_jumpForce = 10; // How high the player jumps
    [SerializeField] private float f_jumpDelay = 0.15f; //How long after the initial input the jump occurs;

    [Space]
    [Header("Physics")]
    [SerializeField] private float f_gravityScale = 10; // The force of gravity on the player
    [SerializeField] private float f_plummetPoint = 5; //the velocity at which below, gravity will increase
    [SerializeField] private float f_plummetMultiplier = 3; //the velocity at which below, gravity will increase
    [SerializeField] private Vector3 v_dragVector = (Vector3.one - Vector3.up) * 0.1f; // The rate at which the player slows down in each axis direction

    #endregion

    #region Private


    internal bool b_grounded;
    private bool b_applyGravity;
    internal bool b_jumpPress;
    private bool b_jumpHold;
    private bool b_jumpUp;
    private bool b_sprintPress;
    internal bool b_sprintHold;
    private bool b_sprintUp;
    internal Vector3 v_movementVector; // The direction the player is inputting
    private Vector3 v_startPos;
    private Vector3 v_groundNormal;
    private Rigidbody rb;
    private Transform t_camTransform;
    private PhotonView view;
    private int i_timesJumpedOff;

    #endregion

    //Methods
    #region Unity Standards
        
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (!view.IsMine && b_networked)
            return;
        b_grounded = CheckGrounded();
        Jump();
        ResetIfOffMap();
        //Debug.Log(rb.velocity.y);
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
        v_startPos = transform.position;

        view = GetComponent<PhotonView>();

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
            rb.AddForce(dir.normalized * f_moveSpeed * Time.deltaTime * (b_sprintHold ? f_sprintMult : 1), ForceMode.Impulse);
            transform.forward = Vector3.Lerp(transform.forward, Vector3.Scale(t_camTransform.forward, Vector3.one - Vector3.up), 0.3f); //Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.2f);

        }
    }

    /// <summary>
    /// Applying the players jump
    /// </summary>
    private void Jump()
    {
        if (!b_jumpPress)
            return;
        if (b_grounded)
            StartCoroutine(DelayedJump());
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

        if (transform.position.y < -25)
        {
            if (i_timesJumpedOff < 4)
            {
                transform.position = v_startPos + (Vector3.up);// * 5);
                i_timesJumpedOff++;
            }
            else
            {
                rb.isKinematic = true;
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
             Invoke("Quit", 3);
#endif
            }
        }

    }

    private void Quit()
    {
        Application.Quit();
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

    #endregion

    #region Private Returns

    private bool CheckGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hit, 0.5f);
        if(hit.collider != null)
        {
            b_applyGravity = hit.distance > 0.15f;
            v_groundNormal = hit.normal;
            return true;

        }
        v_groundNormal = Vector3.up;
        b_applyGravity = true;
        return false;
    }

    #endregion

    #region Public Returns


    #endregion

}
