﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
    private Rigidbody rb;
    private bool b_ableToBuy = true;
    #endregion

    private void Start()
    {
        ac_changer = GetComponent<AppearanceChanger>();
        np_network = GetComponent<NetworkedPlayer>();
        view = GetComponent<PhotonView>();
        InitialiseTools();
        if (transform == NetworkedPlayer.x.GetPlayer())
        {
            SaveManager _sm = FindObjectOfType<SaveManager>();
            AddObserver(_sm);
            LoadSavedTools(_sm);
        }
        rb = GetComponent<Rigidbody>();
    }

    private void LoadSavedTools(SaveManager _sm)
    {
        ToolRack tr = FindObjectOfType<ToolRack>();
        if (_sm?.SaveData.tu_equipped != null)
            foreach ((int toolID, int slotID) tup in _sm.SaveData.tu_equipped)
            {
                if (tup.toolID != -1 && tup.slotID != -1)
                {
                    CallSwapTool((ToolSlot)tup.slotID, tup.toolID, tr, false);
                    ac_changer.SetCurrentArmActive(tup.slotID, false);
                }
            }
        if (_sm?.SaveData.tu_equippedAugments != null)
        {
            foreach ((int toolID, int slotID, AugmentSave[] sav) augs in _sm.SaveData.tu_equippedAugments)
            {
                foreach (AugmentSave s in augs.sav)
                {
                    //Debug.Log($"toolid: {augs.toolID} | slotid: {augs.slotID} | name: {s.SavedAugment.indicies}");
                    ToolBase t = A_toolLoaders[augs.slotID].Tools[augs.toolID];
                    switch (s.SavedAugment.augType)
                    {
                        case AugmentType.standard:
                            (t as WeaponTool).AddStatChanges(AugmentManager.x.GetStandardAugmentAt(s.SavedAugment.augStage, s.SavedAugment.indicies));
                            break;
                        case AugmentType.projectile:
                            (t as WeaponTool).AddStatChanges(AugmentManager.x.GetProjectileAugmentAt(s.SavedAugment.augStage, s.SavedAugment.indicies));
                            break;
                        case AugmentType.cone:
                            (t as WeaponTool).AddStatChanges(AugmentManager.x.GetConeAugmentAt(s.SavedAugment.augStage, s.SavedAugment.indicies));
                            break;
                        default:
                            break;
                    }
                }
                //if (augs.toolID != -1 && augs.slotID != -1)
                //{
                //    Debug.Log(augs.toolID);
                //    LoadAugmentsOnTool(_sm, augs.slotID);
                //}
            }
        }
    }

    private void LoadAugmentsOnTool(SaveManager _sm, int currentSlot)
    {
        // Go through each augment, and check if it's the correct ID and Slot.
        WeaponTool wt = (WeaponTool)A_tools[currentSlot];
        // wt.InitAugmentArrayBlank();<<this isnt called if you have no savedata thats why the first time you load it breaks
        try
        {
            if (_sm.SaveData.tu_equippedAugments != null)
                for (int i = 0; i < _sm.SaveData.tu_equippedAugments.Length; i++)
                {
                    //if (_sm.SaveData.tu_equippedAugments[i].toolID == A_tools[currentSlot].ToolID && _sm.SaveData.tu_equippedAugments[i].slotID == currentSlot)
                    //{
                    for (int j = 0; j < _sm.SaveData.tu_equippedAugments[i].equippedAugs.Length; j++)
                    {
                        switch (_sm.SaveData.tu_equippedAugments[i].equippedAugs[j].SavedAugment.augStage)
                        {
                            case AugmentStage.full:

                                switch (_sm.SaveData.tu_equippedAugments[i].equippedAugs[j].SavedAugment.augType)
                                {
                                    case AugmentType.projectile:
                                        wt.AddStatChanges(AugmentManager.x.GetProjectileAugmentAt(AugmentStage.full, _sm.SaveData.tu_equippedAugments[i].equippedAugs[j].SavedAugment.indicies));
                                        break;
                                    case AugmentType.cone:
                                        wt.AddStatChanges(AugmentManager.x.GetConeAugmentAt(AugmentStage.full, _sm.SaveData.tu_equippedAugments[i].equippedAugs[j].SavedAugment.indicies));
                                        break;
                                    case AugmentType.standard:
                                        wt.AddStatChanges(AugmentManager.x.GetStandardAugmentAt(AugmentStage.full, _sm.SaveData.tu_equippedAugments[i].equippedAugs[j].SavedAugment.indicies));
                                        break;
                                }
                                break;
                            case AugmentStage.fused:
                                switch (_sm.SaveData.tu_equippedAugments[i].equippedAugs[j].SavedAugment.augType)
                                {
                                    case AugmentType.projectile:
                                        wt.AddStatChanges(AugmentManager.x.GetProjectileAugmentAt(AugmentStage.fused, _sm.SaveData.tu_equippedAugments[i].equippedAugs[j].SavedAugment.indicies));
                                        break;
                                    case AugmentType.cone:
                                        wt.AddStatChanges(AugmentManager.x.GetConeAugmentAt(AugmentStage.fused, _sm.SaveData.tu_equippedAugments[i].equippedAugs[j].SavedAugment.indicies));
                                        break;
                                    case AugmentType.standard:
                                        wt.AddStatChanges(AugmentManager.x.GetStandardAugmentAt(AugmentStage.fused, _sm.SaveData.tu_equippedAugments[i].equippedAugs[j].SavedAugment.indicies));
                                        break;
                                }
                                break;
                        }
                    }
                    //}
                }
        }
        catch (System.InvalidCastException e) { Debug.Log("FUCKED IT"); return; }

        Debug.Log("SHOW ME WHAT YOU GOT");
        string s = "";
        for (int j = 0; j < wt.Augs.Length; j++)
        {
            s += $"{j}: ";
            s += wt.Augs[j];
            s += "\n";

        }
        Debug.Log(s);
    }

    public void RemoveAllAugmentsOnWeapon()
    {
        foreach (AugmentGo ago in FindObjectsOfType<AugmentGo>(true))
        {
            ago.GetComponent<PoolableObject>().Die();
        }
    }

    internal float ReturnWeaponWeight(int _i_toolIndex)
    {
        if (A_tools[_i_toolIndex] != null)
            return (A_tools[_i_toolIndex] as WeaponTool).f_weight;
        else
            return 0;
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
            if (b_ableToBuy)
            {
                ToolBase tb = null;
                AugmentGo ab = null;
                // Did we hit a tool?
                try { tb = hit.transform.GetComponent<ToolBase>(); }
                catch (System.InvalidCastException e) { /*if we error remove this return*/return false; }
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
                        tr.ReturnToRack(ets.RackID, (ets.Slot == ToolSlot.leftHand || ets.Slot == ToolSlot.rightHand) ? true : false, false);
                        ac_changer.SetArmActive((int)ts, true);
                        SendSave(-1, ts);
                        return true;
                    }
                    return false;
                }
                // Purchase a tool
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

                                    if (A_tools[0] != null && A_tools[1] != null)
                                        TutorialManager.x.PickedUpBothTools();

                                    SendSave(-1, ts);
                                    return true;
                                case MobilityTool mt:
                                    CallSwapTool(ToolSlot.moblility, tb.ToolID, tr, false);
                                    A_tools[(int)ToolSlot.moblility].RackID = tr.RemoveFromRack(tb.RackID, false);

                                    //TutorialManager Section
                                    TutorialManager.x.PickedUpBackPack();

                                    SendSave(-1, ToolSlot.moblility);
                                    return true;
                            }
                        }
                        if (GetComponent<NugManager>().Nugs >= tb.Cost)
                        {
                            int currentNugs = 0;
                            switch (tb)
                            {
                                case WeaponTool wt:
                                    tb.Purchase(gameObject, t_camTransform, sr, 0, (int)ts);
                                    GetComponent<NugManager>().CollectNugs(-tb.Cost, false);
                                    CallSwapTool(ts, tb.ToolID, tr, true);
                                    currentNugs = GetComponent<NugManager>().Nugs;
                                    A_tools[(int)ts].RackID = tr.RemoveFromRack(tb.RackID, true);

                                    //Tutorial Section
                                    if (A_tools[0] != null && A_tools[1] != null)
                                        TutorialManager.x.PickedUpBothTools();

                                    SendSave(currentNugs, ts, tb.RackID);
                                    return true;
                                case MobilityTool mt:
                                    tb.Purchase(gameObject, t_camTransform, sr, 0, (int)ToolSlot.moblility);
                                    GetComponent<NugManager>().CollectNugs(-mt.Cost, false);
                                    CallSwapTool(ToolSlot.moblility, tb.ToolID, tr, false);
                                    currentNugs = GetComponent<NugManager>().Nugs;
                                    A_tools[(int)ToolSlot.moblility].RackID = tr.RemoveFromRack(tb.RackID, false);

                                    //TutorialManager Section
                                    TutorialManager.x.PickedUpBackPack();

                                    SendSave(currentNugs, ToolSlot.moblility, tb.RackID);

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
        }
        return false;
    }

    private void SendSave(int _nuggets, ToolSlot _toolSlot)
    {
        (int, int) tup = (A_tools[(int)_toolSlot] != null ? A_tools[(int)_toolSlot].ToolID : -1, (int)_toolSlot);
        Notify(new SaveEvent(new PlayerSaveData(_nuggets, -1, -1, null,
            new (int, int)[3] { tup, (-1, -1), (-1, -1) }, null, null, null, null, -1)));
    }
    private void SendSave(int _nuggets, ToolSlot _toolSlot, int _rackID)
    {
        (int, int) tup = (A_tools[(int)_toolSlot].ToolID, (int)_toolSlot);
        (int, int) purchasedTup = (A_tools[(int)_toolSlot].ToolID, A_tools[(int)_toolSlot].RackID);
        Notify(new SaveEvent(new PlayerSaveData(_nuggets, -1, -1, null,
            new (int, int)[] { tup },
            new (int, int)[] { purchasedTup }, null, null, null, -1)));
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
            tr.ReturnToRack(A_tools[(int)ts].RackID, _b_rackType, true);
        }
    }

    [PunRPC]
    /// <summary>
    /// Use the tool based on slot
    /// </summary>
    /// <param name="_ts_tool">Slot to use</param>
    public void UseTool(ToolSlot _ts_tool, Vector3 dir)
    {
        A_tools[(int)_ts_tool]?.SetActiveState(true);
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

        //for (int i = 0; i < A_tools.Length; i++)
        //{
        //    if (A_tools[i] != null)
        //        view.RPC("SwapTool", RpcTarget.Others, (ToolSlot)i, A_tools[i].ToolID);
        //}

    }
    public void CallSwapTool(ToolSlot _ts_slot, int _i_toolID, ToolRack tr, bool _b_rackType)
    {
        SwapTool(_ts_slot, _i_toolID, tr, _b_rackType);
        b_ableToBuy = false;
        StartCoroutine(RackWait());
        PlaySwapNoise();
        view.RPC(nameof(SwapTool), RpcTarget.Others, _ts_slot, _i_toolID);
        //SYNC AUGMENTS HERE
        SyncAllAugments();
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
        if (A_tools[index] != null)
            return A_tools[index].ToolID;
        return -1;
    }

    [PunRPC]
    public void ApplyAugment((int[] inds, int level, int type) aug, int slot)
    {
        AugmentStage st = aug.inds.Length > 1 ? AugmentStage.fused : AugmentStage.full;
        switch (aug.type)
        {
            case 1:
                ProjectileAugment p = AugmentManager.x.GetProjectileAugmentAt(st, aug.inds);
                p.Level = aug.level;
                (A_tools[slot] as WeaponTool).AddStatChanges(p);
                break;
            case 2:
                ConeAugment c = AugmentManager.x.GetConeAugmentAt(st, aug.inds);
                c.Level = aug.level;
                (A_tools[slot] as WeaponTool).AddStatChanges(c);
                break;
            default:
                Augment a = AugmentManager.x.GetStandardAugmentAt(st, aug.inds);
                a.Level = aug.level;
                (A_tools[slot] as WeaponTool).AddStatChanges(a);
                break;
        }
    }

    public void ClearTools()
    {
        RemoveTool(ToolSlot.leftHand);
        RemoveTool(ToolSlot.rightHand);
        RemoveTool(ToolSlot.moblility);
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
            A_tools[(int)_ts_].SetActiveState(false);
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
                    //float spread = A_tools[(int)ts].GetSpread() * (rb.velocity.magnitude * 0.05f);
                    //dir = Quaternion.AngleAxis(Random.Range(-spread, spread), transform.up) * dir;
                    //dir = Quaternion.AngleAxis(Random.Range(-spread, spread), transform.right) * dir;
                    view.RPC(nameof(UseTool), RpcTarget.Others, ts, dir);
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
                A_tools[(int)ts].SetActiveState(true);
                view.RPC(nameof(StopUsingTool), RpcTarget.All, ts);

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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Invoke(nameof(FUCKINGREAPPLYTOOLS), 1);
    }

    private void FUCKINGREAPPLYTOOLS()
    {
        for (int i = 0; i < 3; i++)
        {
            if (A_tools[i] != null)
            {
                CallSwapTool((ToolSlot)i, A_tools[i].ToolID, FindObjectOfType<ToolRack>(), false);
                if (A_tools[i] is ProjectileTool)
                {
                    (A_tools[i] as ProjectileTool).SetActiveState(true);
                    A_tools[i].SetUseable(true);
                }
            }
        }
        SyncAllAugments();
    }

    public void SyncAllAugments()
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (Augment a in (A_tools[i] as WeaponTool).GetAugments())
            {
                int[] inds = a.Stage == AugmentStage.fused ? AugmentManager.x.GetIndicesByName(a.Name) : new int[] { AugmentManager.x.GetAugmentIndex(a.at_type, a.Name) };
                view.RPC(nameof(ApplyAugment), RpcTarget.Others, (inds, a.Level, a.at_type), i);
            }

        }
    }

    private IEnumerator RackWait()
    {
        yield return new WaitForSeconds(0.5f);
        b_ableToBuy = true;
    }

}
