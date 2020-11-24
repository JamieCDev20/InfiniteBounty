using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum ToolSlot
{
    leftHand,
    rightHand,
    moblility,
    rack
}

public class ToolHandler : SubjectBase
{

    #region Serialized Fields

    [SerializeField] private Transform[] A_toolTransforms;
    [SerializeField] private ToolBase[] A_tools = new ToolBase[3];
    [SerializeField] private ToolLoader[] A_toolLoaders;
    [SerializeField] private LayerMask lm_shoppingMask;

    #endregion

    #region Private
    private List<ToolBase> L_ownedTools = new List<ToolBase>();
    private NetworkedPlayer np_network;
    private Transform t_camTransform;
    private PhotonView view;
    private ToolSlot ts_removeToolSlot;
    private int i_removableRackSlot;
    #endregion

    private void Start()
    {
        np_network = GetComponent<NetworkedPlayer>();
        view = GetComponent<PhotonView>();
        InitialiseTools();
    }

    /// <summary>
    /// Whatcha buyin'?
    /// </summary>
    /// <returns>true if you have purchased something</returns>
    private bool CheckIfBuying(ToolSlot ts)
    {
        // Hit something
        RaycastHit hit;
        Ray ray = new Ray(t_camTransform.position, t_camTransform.forward);
        if (Physics.Raycast(ray, out hit, 10f, lm_shoppingMask))
        {
            if (A_tools[(int)ts])
                A_tools[(int)ts].SetActive(false);
            // Are we getting a tool, an augment or something else?
            ToolBase tb = hit.transform.GetComponent<ToolBase>();
            // Which shop is it?
            Shop sr = hit.transform.root.GetComponent<Shop>();
            // Put a tool back;
            EmptyToolSlot ets = hit.transform.GetComponent<EmptyToolSlot>();
            ts_removeToolSlot = ts;
            if (ets != null)
            {
                if (A_tools[(int)ts] == null)
                    return true;
                if (A_tools[(int)ts].RackID == ets.RackID)
                {
                    CallSwapTool(ToolSlot.rack, ets.RackID, (ToolRack)sr, (ets.Slot == ToolSlot.leftHand || ets.Slot == ToolSlot.rightHand) ? true : false);
                    ToolRack tr = (ToolRack)sr;
                    tr.ReturnToRack(ets.RackID, (ets.Slot == ToolSlot.leftHand || ets.Slot == ToolSlot.rightHand) ? true : false);
                    return true;
                }
            }
            switch (sr)
            {
                case ToolRack tr:
                    if (tb.CheckPurchaseStatus())
                    {
                        switch (tb)
                        {
                            case WeaponTool wt:
                                CallSwapTool(ts, tb.ToolID, tr, true);
                                A_tools[(int)ts].RackID = tr.RemoveFromRack(tb.RackID, true);
                                return true;
                            case MobilityTool mt:
                                CallSwapTool(ToolSlot.moblility, tb.ToolID, tr, false);
                                A_tools[(int)ToolSlot.moblility].RackID = tr.RemoveFromRack(tb.RackID, false);
                                return true;
                        }
                    }
                    switch (tb)
                    {
                        case WeaponTool wt:
                            tb.Purchase(gameObject, t_camTransform, sr, 0, (int)ts);
                            CallSwapTool(ts, tb.ToolID, tr, true);
                            A_tools[(int)ts].RackID = tr.RemoveFromRack(tb.RackID, true);
                            return true;
                        case MobilityTool mt:
                            tb.Purchase(gameObject, t_camTransform, sr, 0, (int)ToolSlot.moblility);
                            CallSwapTool(ToolSlot.moblility, tb.ToolID, tr, false);
                            A_tools[(int)ToolSlot.moblility].RackID = tr.RemoveFromRack(tb.RackID, false);
                            return true;
                    }
                    return false;
            }
        }
        return false;
    }

    private void InitialiseTools()
    {
        foreach (ToolLoader tl in A_toolLoaders)
            switch (tl.Slot)
            {
                case ToolSlot.leftHand:
                    tl.LoadTools(A_toolTransforms[(int)ToolSlot.leftHand]);
                    break;
                case ToolSlot.rightHand:
                    tl.LoadTools(A_toolTransforms[(int)ToolSlot.rightHand]);
                    break;
                case ToolSlot.moblility:
                    tl.LoadTools(A_toolTransforms[(int)ToolSlot.moblility]);
                    break;
            }
    }

    public void ReturnToRack(ToolSlot ts, ToolRack tr, bool _b_rackType)
    {
        if (A_tools[(int)ts] != null)
        {
            tr.ReturnToRack(A_tools[(int)ts].RackID, _b_rackType);
        }
    }

    [PunRPC]
    /// <summary>
    /// Use the tool based on slot
    /// </summary>
    /// <param name="_ts_tool">Slot to use</param>
    public void UseTool(ToolSlot _ts_tool, Vector3 dir)
    {
        A_tools[(int)_ts_tool]?.SetActive(true);
        A_tools[(int)_ts_tool]?.Use(dir);
    }


    public void SyncToolOverNetwork()
    {

        for (int i = 0; i < A_tools.Length; i++)
        {
            if (A_tools[i] != null)
                view.RPC("SwapTool", RpcTarget.Others, (ToolSlot)i, A_tools[i].ToolID);
        }

    }
    public void CallSwapTool(ToolSlot _ts_slot, int _i_toolID, ToolRack tr, bool _b_rackType)
    {
        SwapTool(_ts_slot, _i_toolID, tr, _b_rackType);
        view.RPC("SwapTool", RpcTarget.Others, _ts_slot, _i_toolID);

    }

    [PunRPC]
    public void SwapTool(ToolSlot _ts_slot, int _i_toolID)
    {
        switch (_ts_slot)
        {
            case ToolSlot.leftHand:
                RemoveTool(_ts_slot);
                AddTool(_ts_slot, _i_toolID);
                break;
            case ToolSlot.rightHand:
                RemoveTool(_ts_slot);
                AddTool(_ts_slot, _i_toolID);
                break;
            case ToolSlot.moblility:
                RemoveTool(ToolSlot.moblility);
                AddTool(ToolSlot.moblility, _i_toolID);
                break;
        }
    }
    /// <summary>
    /// Swap weapons based on which type it is and/or what hand it should be in
    /// </summary>
    /// <param name="_b_left">Left or right hand</param>
    /// <param name="_tb_tool">Tool to attach</param>
    public void SwapTool(ToolSlot _ts_slot, int _i_toolID, ToolRack tr, bool _b_rackID)
    {
        // Cast weapons to correct types and assign to correct slot
        switch (_ts_slot)
        {
            case ToolSlot.leftHand:
                RemoveTool(ToolSlot.leftHand, tr, _b_rackID);
                AddTool(ToolSlot.leftHand, _i_toolID);
                break;
            case ToolSlot.rightHand:
                RemoveTool(ToolSlot.rightHand, tr, _b_rackID);
                AddTool(ToolSlot.rightHand, _i_toolID);
                break;
            case ToolSlot.moblility:
                RemoveTool(ToolSlot.moblility, tr, _b_rackID);
                AddTool(ToolSlot.moblility, _i_toolID);
                break;
            case ToolSlot.rack:
                RemoveTool(ts_removeToolSlot);
                break;
            default:
                break;
        }


    }

    public int GetTool(int index)
    {
        return A_tools[index].ToolID;
    }

    private void RemoveTool(ToolSlot _ts_slot)
    {
        if (A_tools[(int)_ts_slot] != null)
        {
            A_tools[(int)_ts_slot].gameObject.SetActive(false);
            A_tools[(int)_ts_slot] = null;
        }
    }

    private void RemoveTool(ToolSlot _ts_, ToolRack tr, bool _b_rackType)
    {
        if (A_tools[(int)_ts_] != null)
        {
            ReturnToRack(_ts_, tr, _b_rackType);
            A_tools[(int)_ts_].gameObject.SetActive(false);
            A_tools[(int)_ts_].SetActive(false);
            A_tools[(int)_ts_] = null;
        }
    }

    private void AddTool(ToolSlot _ts_, int _i_toolID)
    {
        // Check here if you have enough nugs.
        A_tools[(int)_ts_] = A_toolLoaders[(int)_ts_].GetToolAt(_i_toolID);
        if (A_tools[(int)_ts_] != null)
        {
            A_tools[(int)_ts_].gameObject.SetActive(true);
            if (!A_tools[(int)_ts_].Purchased)
            {
                L_ownedTools.Add(A_tools[(int)_ts_]);
            }
        }
    }

    private ToolBase GetToolInHand(ToolSlot _ts_)
    {
        return A_tools[(int)_ts_];
    }

    /// <summary>
    /// Get inputs via tool booleans
    /// </summary>
    /// <param name="_tbo_inputs">inputs</param>
    public void RecieveInputs(ToolBools _tbo_inputs)
    {
        // Left hand weapon checks
        CheckPressOrHoldUse(ToolSlot.leftHand, _tbo_inputs.b_LToolDown, _tbo_inputs.b_LToolHold);
        CheckReleaseUse(ToolSlot.leftHand, _tbo_inputs.b_LToolUp);
        // Right hand weapon checks
        CheckPressOrHoldUse(ToolSlot.rightHand, _tbo_inputs.b_RToolDown, _tbo_inputs.b_RToolHold);
        CheckReleaseUse(ToolSlot.rightHand, _tbo_inputs.b_RToolUp);
        // Mobility checks
        CheckPressOrHoldUse(ToolSlot.moblility, _tbo_inputs.b_MToolDown, _tbo_inputs.b_MToolHold);
        CheckReleaseUse(ToolSlot.moblility, _tbo_inputs.b_MToolUp);

        //view.RPC("RecieveInputs", RpcTarget.Others, _tbo_inputs.b_LToolDown, _tbo_inputs.b_LToolHold, _tbo_inputs.b_LToolUp, _tbo_inputs.b_RToolDown, _tbo_inputs.b_RToolHold, _tbo_inputs.b_RToolUp, _tbo_inputs.b_MToolDown, _tbo_inputs.b_MToolHold, _tbo_inputs.b_MToolUp);

    }

    //[PunRPC]
    public void RecieveInputs(bool b_LToolDown, bool b_LToolHold, bool b_LTooUp, bool b_RToolDown, bool b_RToolHold, bool b_RTooUp, bool b_MToolDown, bool b_MToolHold, bool b_MTooUp)
    {
        CheckPressOrHoldUse(ToolSlot.leftHand, b_LToolDown, b_LToolHold);
        CheckReleaseUse(ToolSlot.leftHand, b_LTooUp);

        CheckPressOrHoldUse(ToolSlot.rightHand, b_RToolDown, b_RToolHold);
        CheckReleaseUse(ToolSlot.rightHand, b_RTooUp);

        CheckPressOrHoldUse(ToolSlot.moblility, b_MToolDown, b_MToolHold);
        CheckReleaseUse(ToolSlot.moblility, b_MTooUp);
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
        if (_b_press || _b_hold)
        {
            // You want to buy something, not shoot
            if (ts == ToolSlot.leftHand && _b_press || ts == ToolSlot.rightHand && _b_press)
                if (CheckIfBuying(ts)) return;
            // You only want to shoot when the tool isn't release activated
            if (A_tools[(int)ts] != null)
                if (!A_tools[(int)ts].ReleaseActivated)
                {
                    view.RPC("UseTool", RpcTarget.Others, ts, t_camTransform.forward);
                    A_tools[(int)ts].Use(t_camTransform.forward);
                }
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
        {
            if (_b_released)
                A_tools[(int)ts].SetActive(true);
            if (_b_released && A_tools[(int)ts].ReleaseActivated)
                A_tools[(int)ts].Use(t_camTransform.forward);

        }
    }

    /// <summary>
    /// Obtain the camera transforms
    /// </summary>
    /// <param name="_t_cam"></param>
    public void RecieveCameraTransform(Transform _t_cam)
    {
        t_camTransform = _t_cam;
    }

    public bool CheckInTools(ToolBase _tb_checker)
    {
        return L_ownedTools.Contains(_tb_checker);
    }

    public bool CheckIfRackUpgraded(ToolBase _tb_checker)
    {
        foreach (WeaponTool wt in L_ownedTools)
        {
            if (wt == _tb_checker)
            {
                return wt.RackUpgrade;
            }
        }
        return false;
    }
}
