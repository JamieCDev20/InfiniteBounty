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
    ToolBase[] A_tools = new ToolBase[2];
    List<ToolBase> L_allTools = new List<ToolBase>();
    NetworkedPlayer np_network;
    Transform t_camTransform;
    private void Start()
    {
        np_network = GetComponent<NetworkedPlayer>();
    }

    private bool CheckIfBuying()
    {
        RaycastHit hit;
        Ray ray = new Ray(t_camTransform.position, t_camTransform.forward);
        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            if (hit.transform.GetComponent<WeaponTool>())
            {
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
            case ToolSlot.moblility:
                switch (_tb_tool)
                {
                    case MobilityTool mt:
                        A_tools[(int)_ts_slot] = mt;
                        break;
                }
                break;
            default:
                switch (_tb_tool)
                {
                    case WeaponTool wt:
                        A_tools[(int)_ts_slot] = wt;
                        break;
                }
                break;
        }
    }
    /// <summary>
    /// Get inputs via tool booleans
    /// </summary>
    /// <param name="_tbo_inputs">inputs</param>
    public void RecieveInputs(ToolBools _tbo_inputs)
    {
        CheckPressOrHoldUse(ToolSlot.leftHand, _tbo_inputs.b_LToolDown, _tbo_inputs.b_LToolHold);
        CheckPressOrHoldUse(ToolSlot.rightHand, _tbo_inputs.b_RToolDown, _tbo_inputs.b_RToolHold);
        CheckPressOrHoldUse(ToolSlot.moblility, _tbo_inputs.b_MToolDown, _tbo_inputs.b_MToolHold);
        if(A_tools.Length < 0)
        {
            CheckReleaseUse(ToolSlot.leftHand, _tbo_inputs.b_LToolUp, A_tools[(int)ToolSlot.leftHand].ReleaseActivated);
            CheckReleaseUse(ToolSlot.rightHand, _tbo_inputs.b_RToolUp, A_tools[(int)ToolSlot.rightHand].ReleaseActivated);
            CheckReleaseUse(ToolSlot.moblility, _tbo_inputs.b_RToolUp, A_tools[(int)ToolSlot.moblility].ReleaseActivated);
        }

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
                if (CheckIfBuying()) return;
            // You only want to shoot when the tool isn't release activated
            if(A_tools.Length < 0)
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
    private void CheckReleaseUse(ToolSlot ts, bool _b_released, bool _b_releaseActivated)
    {
        // Use release activated tool when the button is released
        if (_b_released && _b_releaseActivated)
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
