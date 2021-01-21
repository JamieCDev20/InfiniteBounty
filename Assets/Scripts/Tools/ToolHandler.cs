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
    [SerializeField] private AudioClip[] acA_ToolSwapEffects;
    [SerializeField] private Vector2 v2_toolSwapPitchRange;
    [SerializeField] private LayerMask lm_shoppingMask;
    [SerializeField] private LayerMask lm_shootingMask;

    #endregion

    #region Private
    private List<ToolBase> L_ownedTools = new List<ToolBase>();
    private AppearanceChanger ac_changer;
    private NetworkedPlayer np_network;
    private Transform t_camTransform;
    private PlayerHealth ph_health;
    private PhotonView view;
    private ToolSlot ts_removeToolSlot;
    private int i_removableRackSlot;
    #endregion

    private void Start()
    {
        ac_changer = GetComponent<AppearanceChanger>();
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
            ToolBase tb = null;
            AugmentGo ab = null;
            // Are we getting a tool, an augment or something else?
            try { tb = hit.transform.GetComponent<ToolBase>(); }
            catch (System.InvalidCastException e) { }
            try { ab = hit.transform.GetComponent<AugmentGo>(); }
            catch (System.InvalidCastException e) { }
            // Which shop is it?
            Shop sr = hit.transform.root.GetComponent<Shop>();
            // Put a tool back;
            EmptyToolSlot ets = hit.transform.GetComponent<EmptyToolSlot>();
            ts_removeToolSlot = ts;
            if (ets != null)
            {
                if (A_tools[(int)ts] == null)
                    return true;
                Debug.Log(string.Format("Tool Slot: {0} Empty Rack Slot: {1}", A_tools[(int)ts].RackID, ets.RackID));
                if (A_tools[(int)ts].RackID == ets.RackID)
                {
                    CallSwapTool(ToolSlot.rack, ets.RackID, (ToolRack)sr, (ets.Slot == ToolSlot.leftHand || ets.Slot == ToolSlot.rightHand) ? true : false);
                    ToolRack tr = (ToolRack)sr;
                    tr.ReturnToRack(ets.RackID, (ets.Slot == ToolSlot.leftHand || ets.Slot == ToolSlot.rightHand) ? true : false);
                    ac_changer.SetArmActive((int)ts, true);
                    return true;
                }
                return false;
            }
            switch (sr)
            {
                case ToolRack tr:
                    if ((bool)tb?.CheckPurchaseStatus())
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
                    if (GetComponent<NugManager>().Nugs >= tb.Cost)
                    {

                        switch (tb)
                        {
                            case WeaponTool wt:
                                tb.Purchase(gameObject, t_camTransform, sr, 0, (int)ts);
                                GetComponent<NugManager>().CollectNugs(-tb.Cost, false);
                                CallSwapTool(ts, tb.ToolID, tr, true);
                                A_tools[(int)ts].RackID = tr.RemoveFromRack(tb.RackID, true);
                                return true;
                            case MobilityTool mt:
                                tb.Purchase(gameObject, t_camTransform, sr, 0, (int)ToolSlot.moblility);
                                GetComponent<NugManager>().CollectNugs(-mt.Cost, false);
                                CallSwapTool(ToolSlot.moblility, tb.ToolID, tr, false);
                                A_tools[(int)ToolSlot.moblility].RackID = tr.RemoveFromRack(tb.RackID, false);
                                return true;
                        }
                    }
                    else
                    {
                        switch (sr)
                        {
                            case ToolRack toolRackRef:
                                switch (tb)
                                {
                                    case WeaponTool wt:
                                        toolRackRef.UnableToBuy(wt.RackID, true);
                                        break;
                                    case MobilityTool mt:
                                        toolRackRef.UnableToBuy(mt.RackID, false);
                                        break;
                                }
                                break;

                        }
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

    [PunRPC]
    public void StopUsingTool(ToolSlot ts)
    {
        A_tools[(int)ts]?.PlayParticles(false);
        A_tools[(int)ts]?.StopAudio();
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
        PlaySwapNoise();
        view.RPC("SwapTool", RpcTarget.Others, _ts_slot, _i_toolID);

    }

    private void PlaySwapNoise()
    {
        if (acA_ToolSwapEffects.Length > 0)
            AudioSource.PlayClipAtPoint(acA_ToolSwapEffects[Random.Range(0, acA_ToolSwapEffects.Length)], transform.position);
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
        if(A_tools[index] != null)
            return A_tools[index].ToolID;
        return -1;
    }

    public ToolBase GetToolBase(int index)
    {
        if (A_tools[index] != null)
            return A_tools[index];
        return null;
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
            ac_changer.SetArmActive((int)_ts_, false);
            try
            {
                if (_ts_ != ToolSlot.moblility)
                    A_tools[(int)_ts_].GetComponent<Collider>().enabled = false;
            }
            catch (System.NullReferenceException e) { }

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
        if (ph_health.GetIsDead())
            return;
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
                    RaycastHit hit;
                    Vector3 dir = t_camTransform.forward;
                    if (Physics.Raycast(t_camTransform.position, t_camTransform.forward, out hit, 10000, lm_shootingMask, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.distance > 7)
                            dir = hit.point - A_toolTransforms[(int)ts].position;
                    }
                    view.RPC("UseTool", RpcTarget.Others, ts, dir);
                    A_tools[(int)ts].Use(dir);
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
        if (ph_health.GetIsDead())
            return;
        // Use release activated tool when the button is released
        if (A_tools[(int)ts] != null)
        {
            if (_b_released)
            {
                A_tools[(int)ts].SetActive(true);
                view.RPC("StopUsingTool", RpcTarget.All, ts);

            }
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

    public void SetPlayerHealth(PlayerHealth health)
    {
        ph_health = health;
    }
}
