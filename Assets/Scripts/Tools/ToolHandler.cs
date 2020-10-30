using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public enum ToolSlot
{
    leftHand,
    rightHand,
    moblility
}

public class ToolHandler : SubjectBase
{
    [SerializeField] private ToolBase[] A_tools = new ToolBase[3];
    private List<ToolBase> L_allTools = new List<ToolBase>();
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
        Ray ray = new Ray(t_camTransform.GetChild(0).position, t_camTransform.forward);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (Physics.Raycast(ray, out hit, 7f))
        {
            if (hit.transform.GetComponent<ToolBase>())
            {
                hit.transform.GetComponent<ToolBase>().Purchase(gameObject, np_network.PlayerID, (int)ts);
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
                        A_tools[(int)_ts_slot] = wt;
                        break;
                }
                break;
            case ToolSlot.rightHand:
                switch (_tb_tool)
                {
                    case WeaponTool wt:
                        A_tools[(int)_ts_slot] = wt;
                        break;
                }
                break;
            case ToolSlot.moblility:
                switch (_tb_tool)
                {
                    case MobilityTool mt:
                        A_tools[(int)_ts_slot] = mt;
                        break;
                }
                break;
            default:
                break;
        }
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
            if(ts == ToolSlot.leftHand || ts == ToolSlot.rightHand)
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
