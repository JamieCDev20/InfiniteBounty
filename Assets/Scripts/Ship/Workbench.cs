using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : SubjectBase, IInteractible
{

    private PlayerInputManager pim;
    private bool b_isBeingUsed;
    private Transform t_camPositionToReturnTo;
    private SaveManager saveMan;
    [SerializeField] private ToolLoader tl;
    [SerializeField] private GameObject[] goA_tools;
    [SerializeField] private Canvas c_workbenchCanvas;
    [SerializeField] private Transform t_playerPos;
    [Header("Camera & Movement")]
    [SerializeField] private Transform t_camParent;
    [SerializeField] private int i_timesToLerpCam = 50;
    [SerializeField] private float f_lerpTime = 0.5f;
    [SerializeField] private float f_cameraMovementT = 0.4f;

    [Header("Button Info")]
    [SerializeField] private List<WeaponTool> wt_toolsInHand = new List<WeaponTool>();
    private int i_currentAugmentIndex;
    private int i_currentWeaponIndex = 0;

    [Header("Augment Info Display")]
    [SerializeField] AugmentPropertyDisplayer apd;
    public AugmentPropertyDisplayer AugPropertyDisplay { get { return apd; } }
    private List<Augment> aL_allAugmentsOwned = new List<Augment>();
    private ToolHandler th_currentTh;
    private int displayIter = 0;
    #region Interactions
    [Header("Tab info")]
    [SerializeField] private Image img_all;
    [SerializeField] private Image img_equip;
    [SerializeField] private Image img_sameType;
    [SerializeField] private Color col_selected;
    [SerializeField] private Color col_unselected;

    public void Init(SaveManager _sm)
    {
        saveMan = _sm;
        tl.LoadTools(transform);
        if (wt_toolsInHand.Count > 0)
        {
            if (wt_toolsInHand[0] != null)
                tl.GetPrefabTool(wt_toolsInHand[0].ToolID).SetActive(true);
            else if (wt_toolsInHand[1] != null)
                tl.GetPrefabTool(wt_toolsInHand[1].ToolID).SetActive(true);
        }
        AddObserver(saveMan);
        AddObserver(FindObjectOfType<FuseSaver>());
        apd.Init();
    }

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            wt_toolsInHand.Clear();
            // Move player to cinimatic point
            interactor.position = t_playerPos.position;
            interactor.transform.forward = t_playerPos.forward;
            // Don't let the menu be used twice
            b_isBeingUsed = true;
            pim = interactor.GetComponent<PlayerInputManager>();
            th_currentTh = interactor.GetComponent<ToolHandler>();
            PlayerMover pm = pim.GetComponent<PlayerMover>();

            // Check if there's tools in the players hands
            if (th_currentTh.GetTool((int)ToolSlot.leftHand) != -1)
                wt_toolsInHand.Add((WeaponTool)tl?.GetPrefabTool(th_currentTh.GetTool((int)ToolSlot.leftHand)));
            if (th_currentTh.GetTool((int)ToolSlot.rightHand) != -1)
                wt_toolsInHand.Add((WeaponTool)tl?.GetPrefabTool(th_currentTh.GetTool((int)ToolSlot.rightHand)));

            // Stop the player and camera from moving 
            pm.GetComponent<Rigidbody>().isKinematic = true;
            pim.b_shouldPassInputs = false;
            pm.enabled = false;
            t_camPositionToReturnTo = pim.GetCamera().transform;
            pim.GetCamera().enabled = false;
            Camera.main.GetComponent<CameraRespectWalls>().enabled = false;
            pm.GetComponent<PlayerAnimator>().enabled = false;
            // Stop player from shooting
            PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
            _pa.SetShootability(false);
            _pa.StopWalking();
            // Move camera
            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
            c_workbenchCanvas.enabled = true;
            c_workbenchCanvas.transform.localScale = Vector3.one * 0.739f;
            // Find any saved augments and load them
            if (wt_toolsInHand.Count > 0)
            {
                apd.AugType = wt_toolsInHand[i_currentWeaponIndex].AugType;
                aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowAll, false);
                DisplayWeapon();
            }
            // Enable cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;



        }
    }

    public void Interacted() { }

    public void EndInteract()
    {
        // Make the player able to move
        HideWeapon();
        PlayerMover pm = pim?.GetComponent<PlayerMover>();
        pm.GetComponent<Rigidbody>().isKinematic = false;
        pim.b_shouldPassInputs = true;
        pm.enabled = true;
        // Remove weapon refs
        wt_toolsInHand.Clear();

        StartCoroutine(MoveCamera(t_camPositionToReturnTo, pim.GetCamera().transform, false));
        // Player is able to move camera
        c_workbenchCanvas.transform.localScale = Vector3.zero;
        c_workbenchCanvas.enabled = false;
        pim.GetCamera().enabled = true;
        // Player is able to animate again!
        pm.GetComponent<PlayerAnimator>().enabled = true;
        PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
        _pa.SetShootability(true);
        _pa.StartWalking();
        // Remove the cursor and allow the bench to be used again
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        b_isBeingUsed = false;
    }

    #endregion

    public IEnumerator MoveCamera(Transform _t_transformToMoveTo, Transform _t_cameraToMove, bool _b_comingIntoMachine)
    {
        Transform _t = Camera.main.transform;
        float t = 0;
        if (_b_comingIntoMachine)
            _t.parent = t_camParent;
        else
            Camera.main.transform.parent = _t_cameraToMove;

        Vector3 start = _t.localPosition;
        Quaternion iRot = _t.rotation;

        while (t < 1)
        {
            _t.localPosition = Vector3.Lerp(start, Vector3.zero, t);
            _t.rotation = Quaternion.Lerp(iRot, _t_transformToMoveTo.rotation, t);
            t += (Time.deltaTime * (1 / f_lerpTime));
            yield return new WaitForEndOfFrame();
        }

        if (_b_comingIntoMachine)
        {
            _t.localPosition = Vector3.zero;
            _t.localEulerAngles = Vector3.zero;
        }
        else
        {
            Camera.main.GetComponent<CameraRespectWalls>().enabled = true;
            Camera.main.transform.localPosition = new Vector3(0, 0, -4);
            Camera.main.transform.localEulerAngles = new Vector3(10, 0, 0);
        }
    }

    private void DisplayWeapon()
    {
        if (wt_toolsInHand.Count <= 0)
            return;
        goA_tools[tl.GetIndex(wt_toolsInHand[i_currentWeaponIndex])].SetActive(true);

        apd.AugType = wt_toolsInHand[i_currentWeaponIndex].AugType;
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowSameType, false);
    }

    private void HideWeapon()
    {
        if (wt_toolsInHand.Count <= 0)
            return;
        goA_tools[tl.GetIndex(wt_toolsInHand[i_currentWeaponIndex])].SetActive(false);
    }

    #region Button Functions

    public void ApplyAugment()
    {
        // Check if you have tools
        if (wt_toolsInHand.Count == 0)
            return;
        // Get the tool from tool handler as its type
        if (th_currentTh.GetToolBase(i_currentWeaponIndex) is ProjectileTool)
        {
            // Check if the augment is compatable
            if (aL_allAugmentsOwned[apd.CurrentAugIndex].at_type == AugmentType.projectile)
            {
                ProjectileTool pt = th_currentTh.GetToolBase(i_currentWeaponIndex).GetComponent<ProjectileTool>();
                // Attempt to add stat changes
                if (pt.AddStatChanges(aL_allAugmentsOwned[apd.CurrentAugIndex]))
                {
                    // Save the augment
                    Augment _aug = aL_allAugmentsOwned[apd.CurrentAugIndex];
                    AugmentSave _savedAug = new AugmentSave(_aug);
                    Debug.Log(_savedAug.SavedAugment.indicies[0]);
                    SendAttachSave(_aug, _savedAug);
                }
                else
                    Debug.LogError("Augments Full");
            }
            else
                Debug.LogError("Incompatable Augment Type");
        }
        else if (th_currentTh.GetToolBase(i_currentWeaponIndex) is ConeTool)
        {
            if (aL_allAugmentsOwned[apd.CurrentAugIndex].at_type == AugmentType.cone)
            {
                ConeTool ct = th_currentTh.GetToolBase(i_currentWeaponIndex).GetComponent<ConeTool>();
                if (ct.AddStatChanges(aL_allAugmentsOwned[apd.CurrentAugIndex]))
                {
                    Augment _aug = aL_allAugmentsOwned[apd.CurrentAugIndex];
                    AugmentSave _savedAug = new AugmentSave(_aug);
                    Debug.Log(_savedAug.SavedAugment.indicies[0]);
                    SendAttachSave(_aug, _savedAug);
                }
                else
                    Debug.LogError("Augments Full");
            }
            else
                Debug.LogError("Incompatable Augment Type");
        }
        else if (th_currentTh.GetToolBase(i_currentWeaponIndex) is WeaponTool)
        {
            if (aL_allAugmentsOwned[apd.CurrentAugIndex].at_type == AugmentType.standard)
            {
                WeaponTool wt = th_currentTh.GetToolBase(i_currentWeaponIndex).GetComponent<WeaponTool>();
                if (wt.AddStatChanges(aL_allAugmentsOwned[apd.CurrentAugIndex]))
                {
                    Augment _aug = aL_allAugmentsOwned[apd.CurrentAugIndex];
                    AugmentSave _savedAug = new AugmentSave(_aug);
                    Debug.Log(_savedAug.SavedAugment.indicies[0]);
                    SendAttachSave(_aug, _savedAug);
                }
                else
                    Debug.LogError("Augments Full");
            }
            else
                Debug.LogError("Incompatable Augment Type");
        }
    }

    public void RemoveAugment()
    {
        // Augment index out of range.
        Notify(new UnequipAugmentEvent(wt_toolsInHand[i_currentWeaponIndex].ToolID, i_currentWeaponIndex,
            new AugmentSave(wt_toolsInHand[i_currentWeaponIndex].Augs[i_currentAugmentIndex])));
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, apd.CurrentDisplayType, false);

    }

    private void SendAttachSave(Augment _aug, AugmentSave _save)
    {
        // apd.CurrentAugIndex might not be the correct thing to send but we'll see.
        if(_save.SavedAugment.indicies.Length > 1)
            Debug.Log(_save.SavedAugment.indicies[0] + " " + _save.SavedAugment.indicies[1]);
        RemoveAugmentEvent rae = new RemoveAugmentEvent(new AugmentSave(_aug));
        Notify(rae);
        EquipAugEvent eae = new EquipAugEvent((wt_toolsInHand[i_currentWeaponIndex].ToolID, i_currentWeaponIndex, new AugmentSave[] { _save }));
        switch (_save.SavedAugment.augType)
        {
            case AugmentType.projectile:
                wt_toolsInHand[i_currentWeaponIndex].Augs = Utils.AddToArray(wt_toolsInHand[i_currentWeaponIndex].Augs, AugmentManager.x.GetProjectileAugmentAt(_save.SavedAugment.augStage, _save.SavedAugment.indicies));
                break;
            case AugmentType.cone:
                wt_toolsInHand[i_currentWeaponIndex].Augs = Utils.AddToArray(wt_toolsInHand[i_currentWeaponIndex].Augs, AugmentManager.x.GetConeAugmentAt(_save.SavedAugment.augStage, _save.SavedAugment.indicies));
                break;
            case AugmentType.standard:
                wt_toolsInHand[i_currentWeaponIndex].Augs = Utils.AddToArray(wt_toolsInHand[i_currentWeaponIndex].Augs, AugmentManager.x.GetStandardAugmentAt(_save.SavedAugment.augStage, _save.SavedAugment.indicies));
                break;
        }
        Notify(eae);
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, apd.CurrentDisplayType, false);
    }

    public void AllTab()
    {
        Color sel = new Color(col_selected.r, col_selected.g, col_selected.b, 1f);
        Color unSel = new Color(col_unselected.r, col_unselected.g, col_unselected.b, 1f);
        img_all.color = sel;
        img_equip.color = unSel;
        img_sameType.color = unSel;
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowAll, false);
        Debug.Log(apd.CurrentDisplayType);
    }
    public void SameTab()
    {
        Color sel = new Color(col_selected.r, col_selected.g, col_selected.b, 1f);
        Color unSel = new Color(col_unselected.r, col_unselected.g, col_unselected.b, 1f);
        img_sameType.color = sel;
        img_all.color = unSel;
        img_equip.color = unSel;
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowSameType, false);
        Debug.Log(apd.CurrentDisplayType);
    }
    public void EquippedTab()
    {
        Color sel = new Color(col_selected.r, col_selected.g, col_selected.b, 1f);
        Color unSel = new Color(col_unselected.r, col_unselected.g, col_unselected.b, 1f);
        img_equip.color = sel;
        img_all.color = unSel;
        img_sameType.color = unSel;
        List<Augment> augList = new List<Augment>();
        apd.ToolToCheck = (WeaponTool)th_currentTh.GetToolBase(i_currentWeaponIndex);
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowEquipped, false);
        Debug.Log(apd.CurrentDisplayType);
    }

    #endregion

    #region Swap Weapons

    public void ChangeWeaponPos(int lr)
    {

        HideWeapon();

        i_currentWeaponIndex += lr;
        if (i_currentWeaponIndex >= wt_toolsInHand.Count)
            i_currentWeaponIndex = 0;
        else if (i_currentWeaponIndex < 0)
            i_currentWeaponIndex = wt_toolsInHand.Count - 1;

        DisplayWeapon();

    }

    #endregion

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (b_isBeingUsed)
            EndInteract();
    }

}

