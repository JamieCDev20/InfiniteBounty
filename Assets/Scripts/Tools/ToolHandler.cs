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
        // Check for hitting a weapon
        RaycastHit hit;
        Ray ray = new Ray(t_camTransform.GetChild(0).position, t_camTransform.GetChild(0).forward);
        if (Physics.Raycast(ray, out hit, 10f, lm_shoppingMask))
        {
            IPurchasable tb = hit.transform.GetComponent<IPurchasable>();
            Shop sr = hit.transform.root.GetComponent<Shop>();
            if (tb != null)
            {
                if (tb.CheckPurchaseStatus())
                {
                    SwapTool(ts, tb.GetGameObject().GetComponent<ToolBase>().ToolID);
                    return true;
                }
                tb.Purchase(gameObject, t_camTransform, sr, 0, (int)ts);
                return true;
            }
        }
        return false;
    }

    private void InitialiseTools()
    {
        foreach(ToolLoader tl in A_toolLoaders)
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

    [PunRPC]
    /// <summary>
    /// Use the tool based on slot
    /// </summary>
    /// <param name="_ts_tool">Slot to use</param>
    public void UseTool(ToolSlot _ts_tool, Vector3 dir)
    {
        A_tools[(int)_ts_tool].Use(dir);
    }

    /// <summary>
    /// Swap weapons based on which type it is and/or what hand it should be in
    /// </summary>
    /// <param name="_b_left">Left or right hand</param>
    /// <param name="_tb_tool">Tool to attach</param>
    public void SwapTool(ToolSlot _ts_slot, int _i_toolID)
    {
        // Cast weapons to correct types and assign to correct slot
        switch (_ts_slot)
        {
            case ToolSlot.leftHand:
                RemoveTool(ToolSlot.leftHand);
                AddTool(ToolSlot.leftHand, _i_toolID);
                break;
            case ToolSlot.rightHand:
                RemoveTool(ToolSlot.rightHand);
                AddTool(ToolSlot.rightHand, _i_toolID);
                break;
            case ToolSlot.moblility:
                RemoveTool(ToolSlot.moblility);
                AddTool(ToolSlot.moblility, _i_toolID);
                break;
            default:
                break;
        }

    }
    private void RemoveTool(ToolSlot _ts_)
    {
        Debug.Log("Removing Tool");
        if(A_tools[(int)_ts_] != null)
        {
            A_tools[(int)_ts_].gameObject.SetActive(false);
            A_tools[(int)_ts_] = null;
        }
    }

    private void AddTool(ToolSlot _ts_, int _i_toolID)
    {
        Debug.Log(A_tools[(int)_ts_]);
        A_tools[(int)_ts_] = A_toolLoaders[(int)_ts_].GetToolAt(_i_toolID);
        A_tools[(int)_ts_].gameObject.SetActive(true);
        if (!A_tools[(int)_ts_].Purchased)
            L_ownedTools.Add(A_tools[(int)_ts_]);
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
        if(_b_press || _b_hold)
        {
            // You want to buy something, not shoot
            if(ts == ToolSlot.leftHand && _b_press || ts == ToolSlot.rightHand && _b_press)
                if (CheckIfBuying(ts)) return;
            // You only want to shoot when the tool isn't release activated
            if(A_tools[(int)ts] != null)
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
            if (_b_released && A_tools[(int)ts].ReleaseActivated)
                A_tools[(int)ts].Use(t_camTransform.forward);
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
        foreach(WeaponTool wt in L_ownedTools)
        {
            if(wt == _tb_checker)
            {
                return wt.RackUpgrade;
            }
        }
        return false;
    }
}
