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

    public void RecieveInputs(params bool[] _b_inputs)
    {
        
    }
}
