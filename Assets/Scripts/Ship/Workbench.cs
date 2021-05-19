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
    private bool b_initted = false;
    private Transform t_camPositionToReturnTo;
    private SaveManager saveMan;
    [SerializeField] private ToolLoader tl;
    [SerializeField] private GameObject[] goA_tools;
    [SerializeField] private Canvas go_workbenchCanvas;
    [SerializeField] private Transform t_playerPos;
    [SerializeField] private Text toolname;
    [Header("Camera & Movement")]
    [SerializeField] private Transform t_camParent;
    [SerializeField] private int i_timesToLerpCam = 50;
    [SerializeField] private float f_lerpTime = 0.5f;
    [SerializeField] private float f_cameraMovementT = 0.4f;

    [Header("Button Info")]
    [SerializeField] private WeaponTool[] wt_toolsInHand = new WeaponTool[2];
    private int i_currentAugmentIndex;
    private int i_currentWeaponIndex = 0;

    [Header("Augment Info Display")]
    [SerializeField] AugmentPropertyDisplayer apd;
    public AugmentPropertyDisplayer AugPropertyDisplay { get { return apd; } }
    private List<Augment> aL_allAugmentsOwned = new List<Augment>();
    private ToolHandler th_currentTh;
    private int displayIter = 0;

    [Header("Tab info")]
    [SerializeField] private Image img_all;
    [SerializeField] private Image img_equip;
    [SerializeField] private Image img_sameType;
    [SerializeField] private Color col_selected;
    [SerializeField] private Color col_unselected;
    [SerializeField] AudioSource as_source;
    [SerializeField] AudioClip ac_attach;
    [SerializeField] AudioClip ac_notAttached;
    [SerializeField] ParticleSystem ps_stars;
    public void Init(SaveManager _sm)
    {
        saveMan = _sm;
        tl.LoadTools(transform);
        if (!b_initted)
        {
            AddObserver(saveMan);
            AddObserver(FindObjectOfType<FuseSaver>());
            b_initted = true;
        }
        apd.Init();
    }

    #region Interactions

    public void Interacted(Transform interactor)
    {
        if (!b_isBeingUsed)
        {
            //forget what they had last time
            wt_toolsInHand = new WeaponTool[2];
            HideWeapon();
            // Move player to cinimatic point
            interactor.position = t_playerPos.position;
            interactor.transform.forward = t_playerPos.forward;
            
            // Don't let the menu be used twice
            b_isBeingUsed = true;
            pim = interactor.GetComponent<PlayerInputManager>();
            th_currentTh = interactor.GetComponent<ToolHandler>();
            PlayerMover pm = pim.GetComponent<PlayerMover>();

            // Check if there's tools in the players hands
            bool hasATool = false;
            if (th_currentTh.GetTool((int)ToolSlot.leftHand) != -1)
            {
                wt_toolsInHand[0] = (WeaponTool)tl?.GetPrefabTool(th_currentTh.GetTool((int)ToolSlot.leftHand));
                hasATool = true;
            }
            if (th_currentTh.GetTool((int)ToolSlot.rightHand) != -1)
            {
                wt_toolsInHand[1] = (WeaponTool)tl?.GetPrefabTool(th_currentTh.GetTool((int)ToolSlot.rightHand));
                hasATool = true;
            }

            // Stop the player and camera from moving 
            pm.GetComponent<Rigidbody>().isKinematic = true;
            pim.b_shouldPassInputs = false;
            pm.enabled = false;
            t_camPositionToReturnTo = pim.GetCamera().transform;
            pim.GetCamera().enabled = false;
            Camera.main.GetComponent<CameraRespectWalls>().enabled = false;
            pm.GetComponent<PlayerAnimator>().enabled = false;

            // Stop player from animating
            PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
            _pa.SetShootability(false);
            _pa.StopWalking();

            // Move camera
            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
            go_workbenchCanvas.enabled = true;
            go_workbenchCanvas.transform.localScale = Vector3.one * 0.739f;

            if (hasATool)
            {
                i_currentWeaponIndex = wt_toolsInHand[0] == null ? 1 : 0;
                apd.AugType = wt_toolsInHand[i_currentWeaponIndex].AugType;
                SameTab();
                DisplayWeapon();
            }
            else
                AllTab();

            // Enable cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }
    public void Interacted() { }

    public void EndInteract()
    {
        HideWeapon();

        // Make the player able to move
        PlayerMover pm = pim?.GetComponent<PlayerMover>();
        pm.GetComponent<Rigidbody>().isKinematic = false;
        pim.b_shouldPassInputs = true;
        pm.enabled = true;

        // Remove weapon refs
        wt_toolsInHand = new WeaponTool[2];

        StartCoroutine(MoveCamera(t_camPositionToReturnTo, pim.GetCamera().transform, false));
        // Player is able to move camera
        go_workbenchCanvas.transform.localScale = Vector3.zero;
        go_workbenchCanvas.enabled = false;
        pim.GetCamera().enabled = true;

        // Player is able to animate again!
        pm.GetComponent<PlayerAnimator>().enabled = true;
        PlayerAnimator _pa = pm.GetComponent<PlayerAnimator>();
        _pa.SetShootability(true);
        _pa.StartWalking();

        // Remove the cursor and allow the bench to be used again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        b_isBeingUsed = false;

        th_currentTh.SyncAllAugments();

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
            _t.localPosition = Vector3.Lerp(start, _b_comingIntoMachine ? Vector3.zero : Vector3.forward * -4, t);
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

        // Check if you have tools
        bool hasATool = false;
        for (int i = 0; i < wt_toolsInHand.Length; i++)
        {
            if (wt_toolsInHand[i] != null)
                hasATool = true;
        }
        if (!hasATool)
            return;

        goA_tools[tl.GetIndex(wt_toolsInHand[i_currentWeaponIndex])].SetActive(true);
        toolname.text = $"{goA_tools[tl.GetIndex(wt_toolsInHand[i_currentWeaponIndex])].name} ({(i_currentWeaponIndex == 0 ? "L" : "R")})";
        apd.AugType = wt_toolsInHand[i_currentWeaponIndex].AugType;

        //aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, apd.CurrentDisplayType, false);

        //update the text to show the name of the tool
    }

    private void HideWeapon()
    {
        // Check if you have tools
        bool hasATool = false;
        for (int i = 0; i < wt_toolsInHand.Length; i++)
        {
            if (wt_toolsInHand[i] != null)
                hasATool = true;
        }
        if (!hasATool)
            return;
        goA_tools[tl.GetIndex(wt_toolsInHand[i_currentWeaponIndex])].SetActive(false);
        toolname.text = "";
        //hide the text too
    }

    #region Button Functions

    public void ApplyAugment()
    {
        // Check if you have tools
        bool hasATool = false;
        for (int i = 0; i < wt_toolsInHand.Length; i++)
        {
            if (wt_toolsInHand[i] != null)
                hasATool = true;

        }

        if (!hasATool)
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
                    SendAttachSave(_aug, _savedAug);
                    AbleToAugment(_aug.Name + " attached");
                }
                else
                    UnableToAugment("Augment Slots Full");
            }
            else
                UnableToAugment("Incompatable Augment Type");
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
                    AbleToAugment(_aug.Name + " attached");
                }
                else
                    UnableToAugment("Augment Slots Full");
            }
            else
                UnableToAugment("Incompatable Augment Type");
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
                    SendAttachSave(_aug, _savedAug);
                    AbleToAugment(_aug.Name + " attached");
                }
                else
                    UnableToAugment("Augment Slots Full");
            }
            else
                UnableToAugment("Incompatable Augment Type");
        }
        
    }

    private void AbleToAugment(string _message)
    {
        InfoText.x.OnNotify(new InfoTextEvent(_message));
        as_source.pitch = Random.Range(0.9f, 1.1f);
        as_source.PlayOneShot(ac_attach);
        ps_stars.Play();
    }

    private void UnableToAugment(string _message)
    {
        InfoText.x.OnNotify(new InfoTextEvent(_message));
        as_source.pitch = Random.Range(0.9f, 1.1f);
        as_source.PlayOneShot(ac_notAttached);
    }

    public void RemoveAugment()
    {
        // Augment index out of range.
        WeaponTool toolToRemoveFrom = (WeaponTool)th_currentTh.GetToolBase(i_currentWeaponIndex);
        (string nam, int lev) match = apd.AugmentButtons[apd.CurrentAugIndex].GetComponent<AugmentButton>().Tup;
        int i = -1;
        for (int j = 0; j < toolToRemoveFrom.Augs.Length; j++)
        {
            if (toolToRemoveFrom.Augs[j]?.Tup == match)
            {
                i = j;
                break;
            }
        }
        Notify(new UnequipAugmentEvent(toolToRemoveFrom.ToolID, i_currentWeaponIndex, new AugmentSave(toolToRemoveFrom.Augs[i])));
        toolToRemoveFrom.RemoveStatChanges(toolToRemoveFrom.Augs[i]);
        //aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, apd.CurrentDisplayType, false);
        EquippedTab();

    }

    private void SendAttachSave(Augment _aug, AugmentSave _save)
    {
        // apd.CurrentAugIndex might not be the correct thing to send but we'll see.
        RemoveAugmentEvent rae = new RemoveAugmentEvent(new AugmentSave(_aug));
        Notify(rae);
        EquipAugEvent eae = new EquipAugEvent((th_currentTh.GetToolBase(i_currentWeaponIndex).ToolID, i_currentWeaponIndex, new AugmentSave[] { _save }));
        //WeaponTool weaponToEq = (WeaponTool)th_currentTh.GetToolBase(i_currentWeaponIndex);
        
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
        apd.CurrentDisplayType = AugmentDisplayType.ShowAll;
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowAll, false);
    }
    public void SameTab()
    {
        Color sel = new Color(col_selected.r, col_selected.g, col_selected.b, 1f);
        Color unSel = new Color(col_unselected.r, col_unselected.g, col_unselected.b, 1f);
        img_sameType.color = sel;
        img_all.color = unSel;
        img_equip.color = unSel;
        // Check if you have tools
        bool hasATool = false;
        for (int i = 0; i < wt_toolsInHand.Length; i++)
        {
            if (wt_toolsInHand[i] != null)
                hasATool = true;
        }
        if (hasATool)
        {
            apd.CurrentDisplayType = AugmentDisplayType.ShowSameType;
            aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowSameType, false);
        }
        else
        {
            apd.CurrentDisplayType = AugmentDisplayType.None;
            aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.None, false);
        }
    }
    public void EquippedTab()
    {
        Color sel = new Color(col_selected.r, col_selected.g, col_selected.b, 1f);
        Color unSel = new Color(col_unselected.r, col_unselected.g, col_unselected.b, 1f);
        img_equip.color = sel;
        img_all.color = unSel;
        img_sameType.color = unSel;

        apd.ToolToCheck = (WeaponTool)th_currentTh.GetToolBase(i_currentWeaponIndex);
        apd.CurrentDisplayType = AugmentDisplayType.ShowEquipped;
        aL_allAugmentsOwned = apd.InitAugmentList(aL_allAugmentsOwned, AugmentDisplayType.ShowEquipped, false);
        
    }

    #endregion

    #region Swap Weapons

    public void ChangeWeaponPos(int lr)
    {

        HideWeapon();
        // Check if you have tools
        bool hasATool = false;
        for (int i = 0; i < wt_toolsInHand.Length; i++)
        {
            if (wt_toolsInHand[i] != null)
                hasATool = true;

        }

        if (!hasATool)
            return;

        i_currentWeaponIndex += lr;

        if (i_currentWeaponIndex >= wt_toolsInHand.Length)
            i_currentWeaponIndex = 0;
        if (i_currentWeaponIndex < 0)
            i_currentWeaponIndex = 1;
        if (wt_toolsInHand[i_currentWeaponIndex] == null)
            i_currentWeaponIndex = 1 - i_currentWeaponIndex;


        DisplayWeapon();

        apd.ToolToCheck = (WeaponTool)th_currentTh.GetToolBase(i_currentWeaponIndex);

        aL_allAugmentsOwned = apd.DisplayCurrentType();
    }

    #endregion

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (b_isBeingUsed)
            EndInteract();
    }

}

