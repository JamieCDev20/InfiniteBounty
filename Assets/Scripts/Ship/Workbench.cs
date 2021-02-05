using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : MonoBehaviourPunCallbacks, IInteractible
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
    [SerializeField] private GameObject go_augmentButton;
    [SerializeField] private RectTransform rt_augmentButtonParent;
    [SerializeField] private float f_augmentButtonHeight = 85;
    [SerializeField] private List<GameObject> goL_augmentButtonPool = new List<GameObject>();
    [SerializeField] private List<WeaponTool> wt_toolsInHand = new List<WeaponTool>();
    private int i_currentAugmentIndex;
    private int i_currentWeaponIndex = 0;
    [SerializeField] private Scrollbar s_slider;

    [Header("Augment Display")]
    [SerializeField] private AugmentDisplay ad_display;
    private List<Augment> aL_allAugmentsOwned = new List<Augment>();
    private ToolHandler th_currentTh;


    #region Interactions

    public void Init(SaveManager _sm)
    {
        saveMan = _sm;
        foreach (ToolLoader too in FindObjectsOfType<ToolLoader>())
            if (too.name.Contains("Weapon"))
                tl = too;
        tl.LoadTools(transform);        
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
            if (th_currentTh.GetTool(0) != -1)
                wt_toolsInHand.Add((WeaponTool)tl?.GetPrefabTool(th_currentTh.GetTool(0)));
            if (th_currentTh.GetTool(1) != -1)
                wt_toolsInHand.Add((WeaponTool)tl?.GetPrefabTool(th_currentTh.GetTool(1)));

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
                InitAugmentList(augs, false);
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

    internal void InitAugmentList(Augment[] _aA_augmentsInList, bool _b_shouldAddToExistingList)
    {
        // Clear the display
        if (!_b_shouldAddToExistingList)
            aL_allAugmentsOwned.Clear();
        // Update display from save file
        aL_allAugmentsOwned.AddRange(_aA_augmentsInList);
        UpdateAugmentListDisplay(AugmentDisplayType.ShowAll);
    }

    private void UpdateAugmentListDisplay(AugmentDisplayType _adt_whichToShow)
    {
        List<Augment> _aL_augmentsToShow = new List<Augment>();
        // Currently only show all augments
        switch (_adt_whichToShow)
        {
            case AugmentDisplayType.ShowAll:
                _aL_augmentsToShow.AddRange(aL_allAugmentsOwned);
                break;

            case AugmentDisplayType.ShowSameType:
                for (int i = 0; i < aL_allAugmentsOwned.Count; i++)
                {
                    /*
                        if(aL_allAugmentsOwned[i].type == currentWeapon.type)
                            _aL_augmentsToShow.Add(aL_allAugmentsOwned[i]);
                    */
                }
                break;
        }

        for (int i = 0; i < aL_allAugmentsOwned.Count; i++)
        {
            if (goL_augmentButtonPool.Count <= i)
                goL_augmentButtonPool.Add(Instantiate(go_augmentButton, rt_augmentButtonParent));
            goL_augmentButtonPool[i].SetActive(true);
            goL_augmentButtonPool[i].transform.localPosition = new Vector3(0, (-i * f_augmentButtonHeight) - 70, 0);
            //goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[0].text = _aA_augmentsInList[i].level;
            goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[0].text = aL_allAugmentsOwned[i].Name;
            goL_augmentButtonPool[i].GetComponent<AugmentButton>().i_buttonIndex = i;
        }

        rt_augmentButtonParent.sizeDelta = new Vector2(rt_augmentButtonParent.sizeDelta.x, f_augmentButtonHeight * (aL_allAugmentsOwned.Count + 1));
        s_slider.value = 1;
    }

    private void DisplayWeapon()
    {
        tl.GetPrefabTool(wt_toolsInHand[i_currentWeaponIndex]).SetActive(true);
    }

    private void UndisplayWeapon()
    {
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
                    if (!pt.AddStatChanges(aL_allAugmentsOwned[i_currentAugmentIndex]))
                        Debug.LogError("Augments Full");
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
                    if (!ct.AddStatChanges(aL_allAugmentsOwned[i_currentAugmentIndex]))
                        Debug.LogError("Augments Full");
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
                    if (!wt.AddStatChanges(aL_allAugmentsOwned[i_currentAugmentIndex]))
                        Debug.LogError("Augments Full");
                }
                else
                {
                    Debug.LogError("Incompatable Augment Type");
                }
            }


        }
        //aL_allAugmentsOwned[i_currentAugmentIndex];
    }

    public void ClickAugment(int _i_augmentIndexClicked)
    {
        goL_augmentButtonPool[i_currentAugmentIndex].GetComponentInChildren<Outline>().enabled = false;
        i_currentAugmentIndex = _i_augmentIndexClicked;
        goL_augmentButtonPool[i_currentAugmentIndex].GetComponentInChildren<Outline>().enabled = true;

        ad_display.t_augmentName.text = aL_allAugmentsOwned[i_currentAugmentIndex].Name;
        switch (aL_allAugmentsOwned[i_currentAugmentIndex].at_type)
        {
            case AugmentType.standard:
                ad_display.t_augmentType.text = "Standard";
                ad_display.t_augmentFits.text = "Hammer - Blaster - Shredder - Cannon";
                break;
            case AugmentType.projectile:
                ad_display.t_augmentType.text = "Projectile";
                ad_display.t_augmentFits.text = "Blaster - Shredder - Cannon";
                break;
            case AugmentType.cone:
                ad_display.t_augmentType.text = "Cone";
                ad_display.t_augmentFits.text = "Nuggsucker";
                break;
        }



        /*
        aL_augmentsInPool[i_currentAugment].t_levelNumber.text = aA_avaliableAugments[i_currentAugmentIndex].Level;
        aL_augmentsInPool[i_currentAugment].t_augmentType.text = aA_avaliableAugments[i_currentAugmentIndex].type;
        aL_augmentsInPool[i_currentAugment].t_augmentFits.text = aA_avaliableAugments[i_currentAugmentIndex].fits;
        aL_augmentsInPool[i_currentAugment].t_augmentEffects.text = aA_avaliableAugments[i_currentAugmentIndex].effects;
        */
    }

    #endregion

    #region Swap Weapons

    public void ChangeWeaponPos()
    {
        if (wt_toolsInHand != null)
        {
            UndisplayWeapon();
            if (i_currentWeaponIndex == wt_toolsInHand.Count - 1)
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
        if (wt_toolsInHand != null)
        {
            UndisplayWeapon();
            if (i_currentWeaponIndex == 0)
                i_currentWeaponIndex = wt_toolsInHand.Count - 1;
            else
                i_currentWeaponIndex--;
            DisplayWeapon();
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
