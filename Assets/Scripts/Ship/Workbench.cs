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

    [Header("Augment Display")]
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
        if(wt_toolsInHand.Count > 0)
        {
            if (wt_toolsInHand[0] != null)
                tl.GetPrefabTool(wt_toolsInHand[0].ToolID).SetActive(true);
            else if (wt_toolsInHand[1] != null)
                tl.GetPrefabTool(wt_toolsInHand[1].ToolID).SetActive(true);
        }
        AddObserver(saveMan);
        apd.Init();
    }

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
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
            // Find any saved augments and load them
            apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowAll, false);
            // Enable cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Interacted() { }

    public void EndInteract()
    {
        // Make the player able to move
        PlayerMover pm = pim?.GetComponent<PlayerMover>();
        pm.GetComponent<Rigidbody>().isKinematic = false;
        pim.b_shouldPassInputs = true;
        pm.enabled = true;
        // Remove weapon refs
        wt_toolsInHand.Clear();

        StartCoroutine(MoveCamera(t_camPositionToReturnTo, pim.GetCamera().transform, false));
        // Player is able to move camera
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
        if (tl.GetPrefabTool(wt_toolsInHand[i_currentWeaponIndex]) != null)
        {
            tl.GetPrefabTool(wt_toolsInHand[i_currentWeaponIndex]).SetActive(true);
            Debug.Log("GlarBooble");
        }
    }

    private void UndisplayWeapon()
    {
        if (tl.GetPrefabTool(wt_toolsInHand[i_currentWeaponIndex]) != null)
            tl.GetPrefabTool(wt_toolsInHand[i_currentWeaponIndex]).SetActive(false);
    }

    #region Button Functions

    public void ApplyAugment()
    {
        if (wt_toolsInHand.Count != 0)
        {
            if (th_currentTh.GetToolBase(i_currentWeaponIndex) is ProjectileTool)
            {

                if (aL_allAugmentsOwned[i_currentAugmentIndex].at_type == AugmentType.projectile)
                {
                    ProjectileTool pt = th_currentTh.GetToolBase(i_currentWeaponIndex).GetComponent<ProjectileTool>();
                    if (pt.AddStatChanges(aL_allAugmentsOwned[i_currentAugmentIndex]))
                    {
                        SendSave(aL_allAugmentsOwned[i_currentAugmentIndex]);
                    }
                    else
                    {
                        Debug.LogError("Augments Full");
                    }
                }
                else
                {
                    Debug.LogError("Incompatable Augment Type");
                }
            }
            else if (th_currentTh.GetToolBase(i_currentWeaponIndex) is ConeTool)
            {
                if (aL_allAugmentsOwned[i_currentAugmentIndex].at_type == AugmentType.cone)
                {
                    ConeTool ct = th_currentTh.GetToolBase(i_currentWeaponIndex).GetComponent<ConeTool>();
                    if (ct.AddStatChanges(aL_allAugmentsOwned[i_currentAugmentIndex]))
                    {
                        SendSave(aL_allAugmentsOwned[i_currentAugmentIndex]);
                    }
                    else
                    {
                        Debug.LogError("Augments Full");
                    }
                }
                else
                {
                    Debug.LogError("Incompatable Augment Type");
                }
            }
            else if (th_currentTh.GetToolBase(i_currentWeaponIndex) is WeaponTool)
            {

                if (aL_allAugmentsOwned[i_currentAugmentIndex].at_type == AugmentType.standard)
                {
                    WeaponTool wt = th_currentTh.GetToolBase(i_currentWeaponIndex).GetComponent<WeaponTool>();
                    if (wt.AddStatChanges(aL_allAugmentsOwned[i_currentAugmentIndex]))
                    {
                        SendSave(aL_allAugmentsOwned[i_currentAugmentIndex]);
                    }
                    else
                    {
                        Debug.LogError("Augments Full");
                    }
                }
                else
                {
                    Debug.LogError("Incompatable Augment Type");
                }
            }
        }
        //aL_allAugmentsOwned[i_currentAugmentIndex];
    }

    private void SendSave(Augment _aug)
    {
        SaveEvent saveEvent = new SaveEvent(new PlayerSaveData(-1, -1, -1, null, null, null,
            new (int, int, Augment[])[] { (th_currentTh.GetTool(i_currentWeaponIndex), i_currentWeaponIndex, new Augment[] { _aug }) },
            null, null, 0));
        Notify(saveEvent);
    }

    public void AllTab()
    {
        Color sel = new Color(col_selected.r, col_selected.g, col_selected.b, 1f);
        Color unSel = new Color(col_unselected.r, col_unselected.g, col_unselected.b, 1f);
        img_all.color = sel;
        img_equip.color = unSel;
        img_sameType.color = unSel;
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowAll, false);
    }
    public void SameTab()
    {
        Color sel = new Color(col_selected.r, col_selected.g, col_selected.b, 1f);
        Color unSel = new Color(col_unselected.r, col_unselected.g, col_unselected.b, 1f);
        img_sameType.color = sel;
        img_all.color = unSel;
        img_equip.color = unSel;
        //apd.AugType = the type you want to show?
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowSameType, false);
    }
    public void EquippedTab()
    {
        Color sel = new Color(col_selected.r, col_selected.g, col_selected.b, 1f);
        Color unSel = new Color(col_unselected.r, col_unselected.g, col_unselected.b, 1f);
        img_equip.color = sel;
        img_all.color = unSel;
        img_sameType.color = unSel;
        List<Augment> augList = new List<Augment>();
        augList.AddRange(wt_toolsInHand[i_currentWeaponIndex].Augs);
        aL_allAugmentsOwned = apd.InitAugmentList(augList, AugmentDisplayType.ShowEquipped, false);
    }

    #endregion

    #region Swap Weapons

    public void ChangeWeaponPos()
    {
        if(wt_toolsInHand != null && wt_toolsInHand.Count -1 > 0)
            if (wt_toolsInHand[0] != null && wt_toolsInHand[1] != null)
            {
                Debug.Log("Blumnbo");
                UndisplayWeapon();
                if (i_currentWeaponIndex == wt_toolsInHand?.Count - 1)
                {
                    i_currentWeaponIndex = 0;
                }
                else
                {
                    i_currentWeaponIndex++;
                }
                DisplayWeapon();
            }
    }
    public void ChangeWeaponNeg()
    {
        if (wt_toolsInHand != null && wt_toolsInHand.Count - 1 > 0)
        {
            if(wt_toolsInHand[0] != null && wt_toolsInHand[1] != null)
            {
                UndisplayWeapon();
                if (i_currentWeaponIndex == 0)
                    i_currentWeaponIndex = wt_toolsInHand.Count - 1;
                else
                    i_currentWeaponIndex--;
                DisplayWeapon();
            }
        }
    }

    #endregion

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (b_isBeingUsed)
            EndInteract();
    }

}

