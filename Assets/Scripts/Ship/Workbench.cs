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
    [SerializeField] private GameObject go_augmentButton;
    [SerializeField] private RectTransform rt_augmentButtonParent;
    [SerializeField] private float f_augmentButtonHeight = 85;
    [SerializeField] private List<GameObject> goL_augmentButtonPool = new List<GameObject>();
    [SerializeField] private List<WeaponTool> wt_toolsInHand = new List<WeaponTool>();
    private int i_currentAugmentIndex;
    private int i_currentWeaponIndex = 0;
    [SerializeField] private Scrollbar s_slider;

    [Header("Augment Display")]
    [SerializeField] AugmentPropertyDisplayer apd;
    [SerializeField] private AugmentDisplay ad_display;
    private List<Augment> aL_allAugmentsOwned = new List<Augment>();
    private ToolHandler th_currentTh;
    [SerializeField] private GameObject go_propertyButton;
    [SerializeField] private GameObject go_propertyParent;
    [SerializeField] private RectTransform rt_bounds;
    private int displayIter = 0;
    #region Interactions

    public void Init(SaveManager _sm)
    {
        saveMan = _sm;
        foreach (ToolLoader too in FindObjectsOfType<ToolLoader>())
            if (too.name.Contains("Weapon"))
                tl = too;
        tl.LoadTools(transform);
        if(!PoolManager.x.CheckIfPoolExists(go_propertyButton))
            PoolManager.x.CreateNewPool(go_propertyButton, 20);
        augMan = FindObjectOfType<AugmentManager>();
        AddObserver(saveMan);
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
                InitAugmentList(castedAugs, false);
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

    private void OnDestroy()
    {
        PoolManager.x.RemovePool(go_propertyButton);
    }

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
            goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[1].text = "Lvl " + aL_allAugmentsOwned[i]?.Level.ToString();
            goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[0].text = aL_allAugmentsOwned[i]?.Name;
            goL_augmentButtonPool[i].GetComponent<AugmentButton>().i_buttonIndex = i;
        }

        rt_augmentButtonParent.sizeDelta = new Vector2(rt_augmentButtonParent.sizeDelta.x, f_augmentButtonHeight * (aL_allAugmentsOwned.Count + 1));
        s_slider.value = 1;
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
        goL_augmentButtonPool[i_currentAugmentIndex].GetComponentInChildren<Outline>().enabled = false;
        i_currentAugmentIndex = _i_augmentIndexClicked;
        goL_augmentButtonPool[i_currentAugmentIndex].GetComponentInChildren<Outline>().enabled = true;

        ad_display.t_augmentName.text = aL_allAugmentsOwned[i_currentAugmentIndex].Name;
        switch (aL_allAugmentsOwned[i_currentAugmentIndex].at_type)
        {
            case AugmentType.standard:
                ad_display.t_augmentFits.text = "Hammer";
                break;
            case AugmentType.projectile:
                ad_display.t_augmentFits.text = "Blaster - Shredder - Cannon";
                break;
            case AugmentType.cone:
                ad_display.t_augmentFits.text = "Nuggsucker";
                break;
        }

        ad_display.t_augmentName.text = aL_allAugmentsOwned[_i_augmentIndexClicked]?.Name;
        ad_display.t_levelNumber.text = aL_allAugmentsOwned[_i_augmentIndexClicked]?.Level.ToString();
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

    private void UpdatePropertyText(int _i_index)
    {
        AugmentProperties ap = aL_allAugmentsOwned[_i_index].GetAugmentProperties();
        AugmentExplosion ae = aL_allAugmentsOwned[_i_index].GetExplosionProperties();
        if (ap.f_weight != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Weight " + ap.f_weight.ToString();

        }
        if (ap.i_damage != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Enemy Damage " + ap.i_damage.ToString();

        }
        if (ap.i_lodeDamage != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Lode Damage " + ap.i_lodeDamage.ToString();
        }
        if (ap.f_speed != 0)
        {
            switch (aL_allAugmentsOwned[_i_index].at_type)
            {
                case AugmentType.standard:
                    PlaceAugmentProperties(go_propertyButton).text = "Attack Speed " + ap.f_speed.ToString();
                    break;
                case AugmentType.projectile:
                    PlaceAugmentProperties(go_propertyButton).text = "Fire Rate " + ap.f_speed.ToString();
                    break;
                case AugmentType.cone:
                    PlaceAugmentProperties(go_propertyButton).text = "Suck Speed " + ap.f_speed.ToString();
                    break;

            }

        }
        if (ap.f_knockback != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Knockback " + ap.f_knockback.ToString();

        }
        if (ap.f_energyGauge != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Energy Capacity " + ap.f_energyGauge.ToString();

        }
        if (ap.f_heatsink != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Heatsink " + ap.f_heatsink.ToString();

        }
        if (ap.f_recoil != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Recoil " + ap.f_recoil.ToString();

        }
        if (ae.i_damage != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Explosion Enemy Damage " + ae.i_damage.ToString();

        }
        if (ae.i_lodeDamage != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Explosion Lode Damage " + ae.i_lodeDamage.ToString();

        }
        if (ae.f_explockBack != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Explosion Knockback " + ae.f_explockBack.ToString();

        }
        if (ae.f_radius != 0)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Explosion Radius " + ae.f_radius.ToString();

        }
        if (ae.b_impact)
        {
            PlaceAugmentProperties(go_propertyButton).text = "Impact Explosion";

        }
        else
        {
            if (ae.f_detonationTime != 0)
            {
                PlaceAugmentProperties(go_propertyButton).text = "Explosion Detonation Time " + ae.f_detonationTime.ToString();
            }
        }
        if (aL_allAugmentsOwned[_i_index] is ProjectileAugment)
        {
            ProjectileAugment projectileCast = (ProjectileAugment)aL_allAugmentsOwned[_i_index];
            AugmentProjectile augmentProperties = projectileCast.GetProjectileData();
            if (augmentProperties.i_shotsPerRound != 0)
                PlaceAugmentProperties(go_propertyButton).text = "Shots per round " + augmentProperties.i_shotsPerRound;
            if (augmentProperties.f_bulletScale != 0)
                PlaceAugmentProperties(go_propertyButton).text = "Bullet Size " + augmentProperties.f_bulletScale;
            if (augmentProperties.f_gravity != 0)
                PlaceAugmentProperties(go_propertyButton).text = "Bullet Weight " + augmentProperties.f_gravity;
        }
        if (aL_allAugmentsOwned[_i_index] is ConeAugment)
        {
            ConeAugment coneCast = (ConeAugment)aL_allAugmentsOwned[_i_index];
            AugmentCone coneProperties = coneCast.GetConeData();
            if (coneProperties.f_angle != 0)
                PlaceAugmentProperties(go_propertyButton).text = "Cone Width " + coneProperties.f_angle;
            if (coneProperties.f_radius != 0)
                PlaceAugmentProperties(go_propertyButton).text = "Cone Length " + coneProperties.f_radius;
        }

    }
    public Text PlaceAugmentProperties(GameObject _go_template)
    {
        const int scailar = 7;
        GameObject btn = PoolManager.x.SpawnObject(go_propertyButton);
        RectTransform rt_button = btn.GetComponent<RectTransform>();
        btn.transform.parent = go_propertyParent.transform;
        btn.transform.localScale = Vector3.one;
        if (displayIter <= scailar)
        {

            rt_button.anchoredPosition = new Vector2(rt_augmentButtonParent.rect.xMin + (rt_button.rect.width / 2), rt_augmentButtonParent.rect.yMax + (rt_button.rect.height) - (34 * displayIter));
        }
        else
        {
            rt_button.anchoredPosition = new Vector2(rt_augmentButtonParent.rect.xMin + (rt_button.rect.width / 2) + 280, rt_augmentButtonParent.rect.yMax + (rt_button.rect.height) - (34 * (displayIter - scailar)));
        }
        displayIter++;
        return btn?.GetComponent<Text>();
    }

    public void RemoveAugmentProperties()
    {
        displayIter = 0;
        PoolManager.x.KillAllObjects(go_propertyButton);
    }

}


public enum AugmentDisplayType
{
    ShowAll, ShowEquipped, ShowSameType
}
