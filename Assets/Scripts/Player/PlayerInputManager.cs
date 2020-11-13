using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private bool b_networked = false;

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
    [SerializeField] private string s_interactButton = "Use";

    #endregion

    #region Private

    private int playerID;

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
    private PhotonView view;

    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (!view.IsMine && b_networked)
            return;
        GetInputs();
        TellStuffWhatToDo();
    }

    #endregion

    #region Private Voids

    private void Init()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DontDestroyOnLoad(gameObject);

        mover = GetComponent<PlayerMover>();

        toolHandler = GetComponent<ToolHandler>();

        animator = GetComponent<PlayerAnimator>();

        view = GetComponent<PhotonView>();
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
        toolBools.b_RToolDown = Input.GetButtonDown(s_rightToolUse);
        toolBools.b_RToolHold = Input.GetButton(s_rightToolUse);
        toolBools.b_RToolUp = Input.GetButtonUp(s_rightToolUse);

        if (Input.GetButtonDown(s_interactButton))
            Interact();

    }

    private void TellStuffWhatToDo()
    {

        mover.UpdateInputVector(v_inputVector);
        mover.UpdateJumpBools(b_jumpPress, b_jumpHold, b_jumpRelease);
        mover.UpdateSprintBools(b_sprintPress, b_sprintHold, b_sprintRelease);

        camControl.SetLookInput(v2_lookVector);

        toolHandler.RecieveInputs(toolBools);

    }

    private void Interact()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(camControl.transform.GetChild(0).position, camControl.transform.GetChild(0).forward, out hitInfo, 10))
        {
            IInteractible inter = hitInfo.collider.GetComponent<IInteractible>();
            inter?.Interacted();
        }
    }

    #endregion

    #region Public Voids

    public void GoToSpawnPoint()
    {
        foreach (GameObject spawn in TagManager.x.GetTagSet("Spawn"))
        {
            transform.position = spawn.transform.GetChild(playerID).position;
        }
    }

    public void SetCamera(CameraController _cam)
    {
        camControl = _cam;
        camControl.SetFollow(transform);

        mover.SetCameraTranfsorm(camControl.transform);
        toolHandler.RecieveCameraTransform(camControl.transform);
        animator.SetCam(camControl.transform);
    }

    public void SetPlayerNumber(int _i_index)
    {
        playerID = _i_index;
    }

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