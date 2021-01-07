using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private string s_pauseButton = "Pause";
    [SerializeField] private string s_captureButton = "ScreenShot";
    [SerializeField] private TextMeshPro nameText;
    [Space]
    [SerializeField] private bool offline = false;

    #endregion

    #region Private

    private int playerID;
    private string playerNickname;

    private bool b_jumpPress;
    private bool b_jumpHold;
    private bool b_jumpRelease;
    private bool b_sprintPress;
    private bool b_sprintHold;
    private bool b_sprintRelease;
    private bool b_capturePress;

    private bool b_canPickUpNugs;
    private bool b_pausePressed;
    private bool b_inCannon = false;

    private ToolBools toolBools;

    private Vector2 v2_lookVector;
    private Vector3 v_inputVector;

    private PlayerMover mover;
    private CameraController camControl;
    private ToolHandler toolHandler;
    private PlayerAnimator animator;
    private PhotonView view;
    private NugManager nugMan;
    private PlayerHealth ph_health;
    private PhotoCapture pc_capture;
    private Rigidbody rb;
    internal bool b_shouldPassInputs = true;

    #endregion

    //Methods
    #region Unity Standards

    private void Awake()
    {
        Init();
        ph_health.Cam = camControl;
        //camControl.SetFollow(transform);
    }

    private void Update()
    {
        if (!view.IsMine && b_networked)
            return;
        GetInputs();

        if (b_shouldPassInputs)
            TellStuffWhatToDo();

        PhotonNetwork.OfflineMode = offline;

    }

    #endregion

    #region Private Voids

    private void Init()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DontDestroyOnLoad(gameObject);

        mover = GetComponent<PlayerMover>();

        toolHandler = GetComponent<ToolHandler>();

        animator = GetComponent<PlayerAnimator>();

        view = GetComponent<PhotonView>();

        nugMan = GetComponent<NugManager>();

        ph_health = GetComponent<PlayerHealth>();

        pc_capture = GetComponent<PhotoCapture>();

        ph_health.SetAnimator(animator);

        toolHandler.SetPlayerHealth(ph_health);
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

        b_capturePress = Input.GetButtonDown(s_captureButton);

        b_pausePressed = Input.GetButtonDown(s_pauseButton);

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

        pc_capture.RecieveInputs(b_capturePress);

    }

    private void Interact()
    {
        RaycastHit hitInfo;
        LayerMask mask = ~LayerMask.GetMask("Player");
        if (Physics.Raycast(camControl.transform.GetChild(0).position, camControl.transform.GetChild(0).forward, out hitInfo, 10, mask, QueryTriggerInteraction.Ignore))
        {
            IInteractible inter = hitInfo.collider.GetComponent<IInteractible>();
            inter?.Interacted();
            inter?.Interacted(transform);
        }
    }

    #endregion

    #region Public Voids

    public void GoToSpawnPoint()
    {
        foreach (GameObject spawn in TagManager.x.GetTagSet("Spawn"))
        {
            transform.position = spawn.transform.GetChild(playerID).position;
            try
            {
                spawn.transform.GetChild(playerID).GetComponent<PlayerLevelSpawnController>().SetupPlayer(gameObject);
            }
            catch
            {
            }

        }
    }

    public void SetCamera(CameraController _cam)
    {
        camControl = _cam;
        camControl.SetFollow(transform);
        camControl.SetPIM(this);

        mover.SetHUDController(camControl.GetComponent<HUDController>());

        mover.SetCameraTranfsorm(camControl.transform.GetChild(0));
        toolHandler.RecieveCameraTransform(camControl.transform.GetChild(0));
        animator.SetCam(camControl.transform.GetChild(0));
        nugMan.SetNugTextRef(camControl.GetNugCountText());
        ph_health.Cam = camControl;

        //pc_capture.RecieveCameraController(camControl);

        camControl.GetComponentInChildren<PauseMenuController>().SetPIM(this);
    }

    public void ResetCamFollow()
    {
        camControl?.SetFollow(transform);
    }

    public void SetPlayerNumber(int _i_index)
    {
        playerID = _i_index;
    }

    [PunRPC]
    public void JoinedRoom()
    {
        NetworkManager.x.TellClientToSync();
    }

    [PunRPC]
    public void SetPlayerID(int id, string nickName)
    {
        playerID = id;

        ph_health.SetID(id);

        if (id == 0)
            playerNickname = nickName + " (Host)";
        else
            playerNickname = nickName;

        //Debug.Log($"Nickname: {playerNickname} / {id}");
        nameText.text = playerNickname;
    }

    public void SyncNameOverNetwork()
    {
        view.RPC("SetName", RpcTarget.Others, playerID, PhotonNetwork.NickName);
    }

    [PunRPC]
    public void SetName(int id, string _Name)
    {
        if (id == 0)
        {
            _Name += " (Host)";
        }
        nameText.text = _Name;
    }

    public void SetCanPickUpNugs(bool val)
    {
        b_canPickUpNugs = val;
    }

    public void ChangedScene(bool onShip)
    {
        animator.PlayerRevived();
        ResetCamFollow();
        rb.velocity = Vector3.zero;
        view.RPC("SetMaxHealth", RpcTarget.All);
        view.RPC("RemoteRevive", RpcTarget.All);
        b_inCannon = false;
        mover.SetMoveSpeeds(onShip);
    }

    #endregion

    #region Private Returns


    #endregion

    #region Public Returns

    public ToolBools GetToolBools()
    {
        return toolBools;
    }

    public int GetID()
    {
        return playerID;
    }

    public bool CanPickUpNugs()
    {
        return b_canPickUpNugs;
    }

    public bool GetIsPaused()
    {
        return b_pausePressed;
    }

    public CameraController GetCamera()
    {
        return camControl;
    }

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