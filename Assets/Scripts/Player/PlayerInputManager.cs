using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{

    //Variables
    #region Serialised

    [Header("Input Axes")]
    [SerializeField] private string s_horizontalMovement = "Horizontal";
    [SerializeField] private string s_verticalMovement = "Vertical";
    [SerializeField] private string s_jumpButton = "Jump";
    [SerializeField] private string s_sprintButton = "Sprint";
    [SerializeField] private string s_cameraLookLR = "Mouse X";
    [SerializeField] private string s_cameraLookUD = "Mouse Y";
    [SerializeField] private string s_mobilityUse = "Mobility";
    [SerializeField] private string s_leftToolUse = "LeftTool";
    [SerializeField] private string s_rightToolUse = "RightTool";

    #endregion

    #region Private

    private bool b_jumpPress;
    private bool b_jumpHold;
    private bool b_jumpRelease;
    private bool b_sprintPress;
    private bool b_sprintHold;
    private bool b_sprintRelease;

    private ToolBools toolBools;

    private Vector2 v2_lookVector;
    private Vector3 v_inputVector;

    private PlayerMover mover;
    private CameraController camControl;
    private ToolHandler toolHandler;
    private PlayerAnimator animator;

    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        GetInputs();
        TellStuffWhatToDo();
    }

    #endregion

    #region Private Voids

    private void Init()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        camControl = GetComponentInChildren<CameraController>();
        camControl.SetFollow(transform);

        mover = GetComponent<PlayerMover>();
        mover.SetCameraTranfsorm(camControl.transform);

        toolHandler = GetComponent<ToolHandler>();
        toolHandler.RecieveCameraTransform(camControl.transform);

        animator = GetComponent<PlayerAnimator>();
        animator.SetCam(camControl.transform);
    }

    private void GetInputs()
    {

        v_inputVector.x = Input.GetAxisRaw(s_horizontalMovement);
        v_inputVector.z = Input.GetAxisRaw(s_verticalMovement);

        b_jumpPress = Input.GetButtonDown(s_jumpButton);
        b_jumpHold = Input.GetButton(s_jumpButton);
        b_jumpRelease = Input.GetButtonUp(s_jumpButton);

        b_sprintPress = Input.GetButtonDown(s_sprintButton);
        b_sprintHold = Input.GetButton(s_sprintButton);
        b_sprintRelease = Input.GetButtonUp(s_sprintButton);

        v2_lookVector.x = Input.GetAxisRaw(s_cameraLookLR);
        v2_lookVector.y = Input.GetAxisRaw(s_cameraLookUD);

        toolBools.b_MToolDown = Input.GetButtonDown(s_mobilityUse);
        toolBools.b_MToolHold = Input.GetButton(s_mobilityUse);
        toolBools.b_MToolUp = Input.GetButtonUp(s_mobilityUse);
        toolBools.b_LToolDown = Input.GetButtonDown(s_leftToolUse);
        toolBools.b_LToolHold = Input.GetButton(s_leftToolUse);
        toolBools.b_LToolUp = Input.GetButtonUp(s_leftToolUse);
        toolBools.b_LToolDown = Input.GetButtonDown(s_rightToolUse);
        toolBools.b_LToolHold = Input.GetButton(s_rightToolUse);
        toolBools.b_LToolUp = Input.GetButtonUp(s_rightToolUse);

    }

    private void TellStuffWhatToDo()
    {

        mover.UpdateInputVector(v_inputVector);
        mover.UpdateJumpBools(b_jumpPress, b_jumpHold, b_jumpRelease);
        mover.UpdateSprintBools(b_sprintPress, b_sprintHold, b_sprintRelease);

        camControl.SetLookInput(v2_lookVector);

        toolHandler.RecieveInputs(toolBools);

    }

    #endregion

    #region Public Voids


    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}

public struct ToolBools
{
    public bool b_MToolDown;
    public bool b_MToolHold;
    public bool b_MToolUp;
    public bool b_LToolDown;
    public bool b_LToolHold;
    public bool b_LToolUp;
    public bool b_RToolDown;
    public bool b_RToolHold;
    public bool b_RToolUp;
}