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
    private AugmentManager augMan;
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
    private List<Augment> aL_allAugmentsOwned = new List<Augment>();
    private ToolHandler th_currentTh;
    private int displayIter = 0;
    #region Interactions

    public void Init(SaveManager _sm)
    {
        saveMan = _sm;
        foreach (ToolLoader too in FindObjectsOfType<ToolLoader>())
            if (too.name.Contains("Weapon"))
                tl = too;
        tl.LoadTools(transform);
        augMan = FindObjectOfType<AugmentManager>();
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

            StartCoroutine(MoveCamera(t_camParent, pim.GetCamera().transform, true));
            c_workbenchCanvas.enabled = true;
            // Find any saved augments and load them
            saveMan = FindObjectOfType<SaveManager>();
            if (saveMan.SaveData.purchasedAugments != null)
            {
                Augment[] augs = saveMan.SaveData.purchasedAugments;
                Augment[] castedAugs = new Augment[augs.Length];
                if(augs != null && augs.Length != 0)
                {
                    for (int i = 0; i < castedAugs.Length; i++)
                        if(augMan.GetAugment(augs[i].Name) != null)
                            castedAugs[i] = augMan.GetAugment(augs[i].Name).Aug;
                }
                apd.InitAugmentList(aL_allAugmentsOwned, castedAugs, false);
                ClickAugment(0);
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
            tl.GetPrefabTool(wt_toolsInHand[i_currentWeaponIndex]).SetActive(true);
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

    public void ClickAugment(int _i_augmentIndexClicked)
    {
        apd.AugmentButtons[i_currentAugmentIndex].GetComponentInChildren<Outline>().enabled = false;
        i_currentAugmentIndex = _i_augmentIndexClicked;
        apd.AugmentButtons[i_currentAugmentIndex].GetComponentInChildren<Outline>().enabled = true;

        apd.AugDisplay.t_augmentName.text = aL_allAugmentsOwned[i_currentAugmentIndex].Name;
        switch (aL_allAugmentsOwned[i_currentAugmentIndex].at_type)
        {
            case AugmentType.standard:
                apd.AugDisplay.t_augmentFits.text = "Hammer";
                break;
            case AugmentType.projectile:
                apd.AugDisplay.t_augmentFits.text = "Blaster - Shredder - Cannon";
                break;
            case AugmentType.cone:
                apd.AugDisplay.t_augmentFits.text = "Nuggsucker";
                break;
        }

        apd.AugDisplay.t_augmentName.text = aL_allAugmentsOwned[_i_augmentIndexClicked]?.Name;
        apd.AugDisplay.t_levelNumber.text = aL_allAugmentsOwned[_i_augmentIndexClicked]?.Level.ToString();
        apd.RemoveAugmentProperties();
        apd.UpdatePropertyText(aL_allAugmentsOwned[_i_augmentIndexClicked]);
//        UpdatePropertyText(_i_augmentIndexClicked);
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


public enum AugmentDisplayType
{
    ShowAll, ShowEquipped, ShowSameType
}
