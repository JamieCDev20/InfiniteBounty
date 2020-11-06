using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolSlot
{
    leftHand,
    rightHand,
    moblility
}

public class ToolHandler : SubjectBase
{
    [SerializeField] private Transform[] A_toolTransforms;
    [SerializeField] private ToolBase[] A_tools = new ToolBase[3];
    [SerializeField] private LayerMask lm_shoppingMask;
    private List<ToolBase> L_ownedTools = new List<ToolBase>();
    private NetworkedPlayer np_network;
    private Transform t_camTransform;
    private void Start()
    {
        np_network = GetComponent<NetworkedPlayer>();
    }

    /// <summary>
    /// Whatcha buyin'?
    /// </summary>
    /// <returns>true if you have purchased something</returns>
    private bool CheckIfBuying(ToolSlot ts)
    {
        // Check for hitting a weapon
        RaycastHit hit;
        Ray ray = new Ray(t_camTransform.GetChild(0).position, t_camTransform.GetChild(0).forward);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
        if (Physics.Raycast(ray, out hit, 10f, lm_shoppingMask))
        {
            ToolBase tb = hit.transform.GetComponent<ToolBase>();
            if (tb)
            {
                tb.Purchase(gameObject, 0, (int)ts);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Use the tool based on slot
    /// </summary>
    /// <param name="_ts_tool">Slot to use</param>
    public void UseTool(ToolSlot _ts_tool)
    {
        A_tools[(int)_ts_tool].Use();
    }

    /// <summary>
    /// Swap weapons based on which type it is and/or what hand it should be in
    /// </summary>
    /// <param name="_b_left">Left or right hand</param>
    /// <param name="_tb_tool">Tool to attach</param>
    public void SwapWeapon(ToolSlot _ts_slot, ToolBase _tb_tool)
    {
        // Cast weapons to correct types and assign to correct slot
        switch (_ts_slot)
        {
            case ToolSlot.leftHand:
                switch (_tb_tool)
                {
                    case WeaponTool wt:
                        RemoveTool(_ts_slot);
                        AddTool(_ts_slot, wt);
                        break;
                    case MobilityTool mt:
                        RemoveTool(ToolSlot.moblility);
                        AddTool(ToolSlot.moblility, mt);
                        Debug.Log("Boop");
                        break;
                }
                break;
            case ToolSlot.rightHand:
                switch (_tb_tool)
                {
                    case WeaponTool wt:
                        RemoveTool(_ts_slot);
                        AddTool(_ts_slot, wt);
                        break;
                    case MobilityTool mt:
                        RemoveTool(ToolSlot.moblility);
                        AddTool(ToolSlot.moblility, mt);
                        break;
                }
                break;
            case ToolSlot.moblility:
                switch (_tb_tool)
                {
                    case MobilityTool mt:
                        RemoveTool(_ts_slot);
                        AddTool(_ts_slot, mt);
                        break;
                }
                break;
            default:
                break;
        }
    }
    private void RemoveTool(ToolSlot _ts_)
    {
        if(A_tools[(int)_ts_] != null)
        {
            A_tools[(int)_ts_].transform.parent = null;
            A_tools[(int)_ts_].gameObject.SetActive(false);
        }
    }

    private void AddTool(ToolSlot _ts_, ToolBase _tb_)
    {
        A_tools[(int)_ts_] = _tb_;
        _tb_.transform.parent = A_toolTransforms[(int)_ts_];
        _tb_.transform.localPosition = Vector3.zero;
        _tb_.transform.localRotation = Quaternion.identity;
        if (!L_ownedTools.Contains(_tb_))
            L_ownedTools.Add(_tb_);
    }

    /// <summary>
    /// Get inputs via tool booleans
    /// </summary>
    /// <param name="_tbo_inputs">inputs</param>
    public void RecieveInputs(ToolBools _tbo_inputs)
    {
        // Left hand weapon checks
        CheckPressOrHoldUse(ToolSlot.leftHand, _tbo_inputs.b_LToolDown, _tbo_inputs.b_LToolHold);
        CheckReleaseUse(ToolSlot.leftHand, _tbo_inputs.b_LToolUp );
        // Right hand weapon checks
        CheckPressOrHoldUse(ToolSlot.rightHand, _tbo_inputs.b_RToolDown, _tbo_inputs.b_RToolHold);
        CheckReleaseUse(ToolSlot.rightHand, _tbo_inputs.b_RToolUp);
        // Mobility checks
        CheckPressOrHoldUse(ToolSlot.moblility, _tbo_inputs.b_MToolDown, _tbo_inputs.b_MToolHold);
        CheckReleaseUse(ToolSlot.moblility, _tbo_inputs.b_RToolUp);

    }

    /// <summary>
    /// Use a tool or buy an item based on where you're pointing.
    /// </summary>
    /// <param name="ts">Which hand to use</param>
    /// <param name="_b_press">Button Pressed</param>
    /// <param name="_b_hold">Button Held</param>
    private void CheckPressOrHoldUse(ToolSlot ts, bool _b_press, bool _b_hold)
    {
        // Do nothing when you're not pressing or holding the button
        if(_b_press || _b_hold)
        {
            // You want to buy something, not shoot
            if(ts == ToolSlot.leftHand && _b_press || ts == ToolSlot.rightHand && _b_press)
                if (CheckIfBuying(ts)) return;
            // You only want to shoot when the tool isn't release activated
            if(A_tools[(int)ts] != null)
                if (!A_tools[(int)ts].ReleaseActivated)
                    A_tools[(int)ts].Use();
        }
    }
    /// <summary>
    /// Use release based weapons
    /// </summary>
    /// <param name="ts">Tool slot</param>
    /// <param name="_b_released">Release button status</param>
    /// <param name="_b_releaseActivated">Weapon release activation</param>
    private void CheckReleaseUse(ToolSlot ts, bool _b_released)
    {
        // Use release activated tool when the button is released
        if (A_tools[(int)ts] != null)
            if (_b_released && A_tools[(int)ts].ReleaseActivated)
                A_tools[(int)ts].Use();
    }
    /// <summary>
    /// Obtain the camera transforms
    /// </summary>
    /// <param name="_t_cam"></param>
    public void RecieveCameraTransform(Transform _t_cam)
    {
        t_camTransform = _t_cam;
    }
}
