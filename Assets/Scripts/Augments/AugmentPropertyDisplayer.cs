using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AugmentPropertyDisplayer : MonoBehaviour
{
    [SerializeField] private int i_xDisplacement;
    [SerializeField] private int i_yDisplacement;
    [SerializeField] private int i_columnMax;
    [SerializeField] private GameObject go_propertyText; // What is this?
    [SerializeField] private GameObject go_propertyParent; // the parent that all properties get set to
    [SerializeField] private GameObject go_clickableAugmentButton; // the clickable augment button. never used by the vending machine
    [SerializeField] private GameObject go_listMover; //this is the parent of the clickable buttons. not used in the vending machine
    [SerializeField] private List<GameObject> goL_augmentButtonPool = new List<GameObject>(); //what is this?
    [SerializeField] private RectTransform rt_augmentButtonParent; //what is this?
    [SerializeField] private Scrollbar s_slider;
    [SerializeField] private float f_augmentButtonHeight = 85;
    [SerializeField] private AugmentDisplay ad_display;
    [SerializeField] private Sprite[] fitIcons;
    List<Augment> aL_allAugmentsOwned = new List<Augment>(); // clarify
    List<GameObject> goL_propertyList = new List<GameObject>(); // clarify
    private int i_currentAugmentIndex = 0;
    private AugmentDisplayType adt_currentDisplayType;
    private WeaponTool wt_toolToCheck;
    private string s_augName;
    private AugmentType at_type;
    public WeaponTool ToolToCheck { get { return wt_toolToCheck; } set { wt_toolToCheck = value; } }
    public AugmentType AugType { set { at_type = value; } }
    public AugmentDisplayType CurrentDisplayType { get { return adt_currentDisplayType; } set { adt_currentDisplayType = value; } }
    public string AugmentName { set { s_augName = value; } }
    public List<GameObject> AugmentButtons { get { return goL_augmentButtonPool; } }
    public AugmentDisplay AugDisplay { get { return ad_display; } }
    public int CurrentAugIndex { get { return i_currentAugmentIndex; } }
    public Sprite[] FitIcons { get { return fitIcons; } }

    private int i_displayIter = 0;

    public void Init()
    {

    }

    private void OnDestroy()
    {
        //if(!PoolManager.x.CheckIfPoolExists(go_propertyButton))
        //    PoolManager.x.RemovePool(go_propertyButton);
    }

    public List<Augment> InitAugmentList(List<Augment> aL_augs, AugmentDisplayType adt, bool _b_shouldAddToExistingList, params (string nam, int lv)[] toExclude)
    {
        // Init some lists
        List<Augment> _augmentsInList = new List<Augment>();

        RemoveAugmentProperties();

        // Clear the display <<<< THIS NEEDS STUFF DONE TO IT
        if (!_b_shouldAddToExistingList)
        {
            aL_augs.Clear();
            foreach (GameObject btn in goL_augmentButtonPool)
                Destroy(btn);
            goL_augmentButtonPool.Clear();
            aL_allAugmentsOwned.Clear();
        }

        // Find all purchased Augments
        SaveManager saveMan = FindObjectOfType<SaveManager>();

        if (saveMan.SaveData.purchasedAugments != null)
        {
            AugmentSave[] as_data = saveMan.SaveData.purchasedAugments;
            Augment[] augs = new Augment[as_data.Length];

            //this loop goes through the AugmentSave[] and converts each of them into augments
            for (int i = 0; i < augs.Length; i++)
            {
                switch (saveMan.SaveData.purchasedAugments[i].SavedAugment.augType)
                {
                    case AugmentType.projectile:
                        augs[i] = AugmentManager.x.GetProjectileAugmentAt(as_data[i].SavedAugment.augStage, as_data[i].SavedAugment.indicies);
                        break;
                    case AugmentType.cone:
                        augs[i] = AugmentManager.x.GetConeAugmentAt(as_data[i].SavedAugment.augStage, as_data[i].SavedAugment.indicies);
                        break;
                    case AugmentType.standard:
                        augs[i] = AugmentManager.x.GetStandardAugmentAt(as_data[i].SavedAugment.augStage, as_data[i].SavedAugment.indicies);
                        break;
                }
                augs[i].at_type = saveMan.SaveData.purchasedAugments[i].SavedAugment.augType;
                augs[i].Level = saveMan.SaveData.purchasedAugments[i].SavedAugment.level;
            }


            FuseSaver fs = FuseSaver.x;

            if (!Utils.ArrayIsNullOrZero(augs))
                _augmentsInList.AddRange(augs);
            if (!Utils.ArrayIsNullOrZero(fs.FusedAugments))
                _augmentsInList.AddRange(fs.FusedAugments);
            if (!Utils.ArrayIsNullOrZero(fs.FusedProjectiles))
                _augmentsInList.AddRange(fs.FusedProjectiles);
            if (!Utils.ArrayIsNullOrZero(fs.FusedCones))
                _augmentsInList.AddRange(fs.FusedCones);

            // ^^ this is everything in the "Old" region but just compressed for space and niceness

            #region Old
            /*
                //if you have fused augments and you have regular augments
                if (!Utils.ArrayIsNullOrZero(fs.FusedAugments) && !Utils.ArrayIsNullOrZero(augs))
                {
                    augs = Utils.CombineArrays(augs, fs.FusedAugments);
                    //fused augments is added to the list
                }
                //if you have fused augments and no regular augments
                else if(!Utils.ArrayIsNullOrZero(fs.FusedAugments) && Utils.ArrayIsNullOrZero(augs))
                {
                    augs = fs.FusedAugments;
                    //fused augments the list
                }

                //if you have fused projectile augments and regular augments
                if (!Utils.ArrayIsNullOrZero(fs.FusedProjectiles) && !Utils.ArrayIsNullOrZero(augs))
                {
                    augs = Utils.CombineArrays(augs, fs.FusedProjectiles);
                    //fused projectiles gets added
                }
                //if you have no augments and you have fused projectiles
                else if(!Utils.ArrayIsNullOrZero(fs.FusedProjectiles) && Utils.ArrayIsNullOrZero(augs))
                {
                    augs = fs.FusedProjectiles;
                    //fused projectiles is the list
                }

                //if you have fused cones and augments
                if (!Utils.ArrayIsNullOrZero(fs.FusedCones) && !Utils.ArrayIsNullOrZero(augs))
                {
                    augs = Utils.CombineArrays(augs, fs.FusedCones);
                    //fused cones gets added
                }
                //if you have fused cones and no augments
                else if(!Utils.ArrayIsNullOrZero(fs.FusedCones) && Utils.ArrayIsNullOrZero(augs))
                {
                    augs = fs.FusedCones;
                    //the list is fused cones
                }

                //the augments list then gets added to another list
                _augmentsInList.AddRange(augs);
                */
            #endregion

        }
        // Update display from save file
        aL_allAugmentsOwned.AddRange(_augmentsInList);
        aL_allAugmentsOwned = UpdateAugmentListDisplay(aL_allAugmentsOwned, adt, toExclude);
        return aL_allAugmentsOwned;
    }

    public List<Augment> InitAugmentList(List<Augment> aL_augs, AugmentDisplayType adt, bool _b_shouldAddToExistingList)
    {
        return InitAugmentList(aL_augs, adt, _b_shouldAddToExistingList, ("", 0));
    }

    #region DisplayTypes

    private List<Augment> DisplayAugmentsOfTypeExcluding(List<Augment> aL_augs, bool excludeFused, params (string nam, int lv)[] _toExclude)
    {
        List<Augment> _augList = new List<Augment>();
        List<(string nam, int lv)> _exclusionZone = new List<(string nam, int lv)>(_toExclude);

        foreach (Augment aug in aL_augs)
            if (aug.at_type == at_type && !_exclusionZone.Contains((aug.Name, aug.Level)))
            {
                if (excludeFused && aug.Stage != AugmentStage.fused)
                    _augList.Add(aug);
                else if (!excludeFused)
                    _augList.Add(aug);
            }
            else
            {
                _exclusionZone.Remove((aug.Name, aug.Level));
            }
        return _augList;
    }

    private List<Augment> DisplayAugmentsOfType(List<Augment> aL_augs, bool _excludeFused)
    {
        List<Augment> _augList = new List<Augment>();
        foreach (Augment aug in aL_augs)
            if (aug.at_type == at_type)
                if (_excludeFused && aug.Stage != AugmentStage.fused)
                    _augList.Add(aug);
                else if (!_excludeFused)
                {
                    _augList.Add(aug);
                }
        return _augList;
    }

    private List<Augment> DisplayAugmentsWithNameExclulding(List<Augment> aL_augs, params (string nam, int lv)[] _toExclude)
    {
        List<Augment> _augList = new List<Augment>();
        List<(string nam, int lv)> _exclusionZone = new List<(string nam, int lv)>(_toExclude);

        foreach (Augment aug in aL_augs)
            if (aug.Name == s_augName && !_exclusionZone.Contains((aug.Name, aug.Level)))
                _augList.Add(aug);
            else
                _exclusionZone.Remove((aug.Name, aug.Level));
        return _augList;
    }

    private List<Augment> DisplayAugmentsWithName(List<Augment> aL_augs)
    {
        List<Augment> _augList = new List<Augment>();
        foreach (Augment aug in aL_augs)
            if (aug.Name == s_augName)
                _augList.Add(aug);
        return _augList;
    }

    private List<Augment> DisplayAttachedAugments()
    {
        List<Augment> _augList = new List<Augment>();
        // We don't actually add any augments here???
        return _augList;
    }

    #endregion

    private List<Augment> UpdateAugmentListDisplay(List<Augment> aL_augs, AugmentDisplayType _adt_whichToShow, params (string nam, int lv)[] _toExclude)
    {
        List<Augment> _aL_augmentsToShow = new List<Augment>();

        //based on display type _aL_augmentsToShow is added to with different size stuff
        switch (_adt_whichToShow)
        {
            case AugmentDisplayType.ShowAll:
                _aL_augmentsToShow.AddRange(aL_augs);
                break;

            case AugmentDisplayType.ShowSameType:
                _aL_augmentsToShow.AddRange(DisplayAugmentsOfType(aL_augs, false));
                break;

            case AugmentDisplayType.ShowSameTypeNotFused:
                _aL_augmentsToShow.AddRange(DisplayAugmentsOfType(aL_augs, true));
                break;

            case AugmentDisplayType.ShowSameTypeNotFusedExcluding:
                _aL_augmentsToShow.AddRange(DisplayAugmentsOfTypeExcluding(aL_augs, true, _toExclude));
                break;

            case AugmentDisplayType.ShowSameName:
                _aL_augmentsToShow.AddRange(DisplayAugmentsWithName(aL_augs));
                break;

            case AugmentDisplayType.ShowSameNameExcluding:
                _aL_augmentsToShow.AddRange(DisplayAugmentsWithNameExclulding(aL_augs, _toExclude));
                break;

            case AugmentDisplayType.ShowEquipped:
                if (wt_toolToCheck != null)
                {
                    foreach (Augment auggy in wt_toolToCheck.Augs)
                        if (auggy != null)
                        {
                            _aL_augmentsToShow.Add(auggy);
                        }
                }
                break;

            case AugmentDisplayType.ShowSameTypeExcluding:
                _aL_augmentsToShow.AddRange(DisplayAugmentsOfTypeExcluding(aL_augs, false, _toExclude));
                break;

            case AugmentDisplayType.None:
                _aL_augmentsToShow.Clear();
                break;
        }

        //if there are any augments that need to be displayed
        if (_aL_augmentsToShow != null)
        {
            //loop through all augments that should be showed
            for (int i = 0; i < _aL_augmentsToShow.Count; i++) // << This is only relevant in the workbench and microwave
            {
                if (goL_augmentButtonPool.Count <= i) // this means that goL_augmentButtonPool should only be used in the microwave and the workbench and go_augmentButton should be the clickable button. never in something that can be used by the vending machine
                    goL_augmentButtonPool.Add(Instantiate(go_clickableAugmentButton));

                GameObject b = goL_augmentButtonPool[i];

                // position stuff
                b.name = b.name.Replace("(Clone)", "");
                b.SetActive(true);
                b.transform.parent = go_listMover.transform;
                b.transform.localPosition = new Vector3(0, (-i * f_augmentButtonHeight) - 70, 0);
                RectTransform r = b.GetComponent<RectTransform>();
                b.GetComponent<RectTransform>().offsetMin = new Vector2(0, r.offsetMin.y);
                b.GetComponent<RectTransform>().offsetMax = new Vector2(0, r.offsetMax.y);

                // Set text and sprite
                b.GetComponentsInChildren<Text>()[0].text = _aL_augmentsToShow[i].Name;
                b.GetComponentsInChildren<Text>()[0].color = _aL_augmentsToShow[i].Stage == AugmentStage.full ? Color.white : Color.yellow;
                b.GetComponentsInChildren<Text>()[1].text = "Lv " + _aL_augmentsToShow[i]?.Level.ToString();
                b.GetComponentsInChildren<Image>()[1].sprite = fitIcons[(int)_aL_augmentsToShow[i].at_type];
                // Scale
                b.transform.localScale = Vector3.one;
                // set the buttons parent and index
                AugmentButton btn = goL_augmentButtonPool[i].GetComponent<AugmentButton>();
                btn.i_displayListIndex = i;// // << getting the value in the array of augments that the button is "holding"
                btn.i_purchasedListIndex = FindAugmentToShowIndexFromOwned(_aL_augmentsToShow[i].Name, _aL_augmentsToShow[i].Level);
                btn.Tup = (_aL_augmentsToShow[i].Name, _aL_augmentsToShow[i].Level);
                btn.Parent = transform.root.gameObject;
            }
        }

        rt_augmentButtonParent.sizeDelta = new Vector2(rt_augmentButtonParent.sizeDelta.x, f_augmentButtonHeight * (aL_augs.Count + 1));
        s_slider.value = 1;

        return _aL_augmentsToShow;

    }

    private List<Augment> UpdateAugmentListDisplay(List<Augment> aL_augs, AugmentDisplayType _adt_whichToShow)
    {

        return UpdateAugmentListDisplay(aL_augs, _adt_whichToShow, ("", 0));

    }

    private int FindAugmentToShowIndexFromOwned(string _name, int _level)
    {
        switch (adt_currentDisplayType)
        {
            case AugmentDisplayType.ShowEquipped:
                if (!Utils.ArrayIsNullOrZero(wt_toolToCheck.Augs))
                    for (int i = 0; i < wt_toolToCheck.Augs.Length; i++)
                    {
                        if (wt_toolToCheck.Augs[i] != null)
                            if (wt_toolToCheck.Augs[i].Name == _name && wt_toolToCheck.Augs[i].Level == _level)
                                return i;
                    }
                break;
            default:
                for (int i = 0; i < aL_allAugmentsOwned.Count; i++)
                {
                    if (aL_allAugmentsOwned[i].Name == _name && aL_allAugmentsOwned[i].Level == _level)
                    {
                        return i;
                    }
                }
                break;
        }
        return 0;
    }

    private void RefreshActiveButtonIndexes()
    {
        for (int i = 0; i < goL_augmentButtonPool.Count; i++)
        {
            goL_augmentButtonPool[i].GetComponent<AugmentButton>().i_displayListIndex = i;
        }
    }

    public void ClickAugment(int _i_augmentIndexClicked)
    {
        //this is called in both the microwave and the workbench.

        // Highlight button
        for (int i = 0; i < goL_augmentButtonPool.Count; i++)
        {
            goL_augmentButtonPool[i].GetComponentInChildren<Outline>().enabled = false;
        }
        i_currentAugmentIndex = _i_augmentIndexClicked;
        goL_augmentButtonPool[i_currentAugmentIndex].GetComponentInChildren<Outline>().enabled = true;

        switch (adt_currentDisplayType)
        {
            case AugmentDisplayType.ShowEquipped:
                RemoveAugmentProperties();
                try
                {
                    SetFitIcon((int)wt_toolToCheck.Augs[_i_augmentIndexClicked].at_type);
                    ad_display.t_augmentName.text = wt_toolToCheck.Augs[_i_augmentIndexClicked].Name;
                    ad_display.t_levelNumber.text = wt_toolToCheck.Augs[_i_augmentIndexClicked].Level.ToString();
                    UpdatePropertyText(wt_toolToCheck.Augs[_i_augmentIndexClicked]);

                }
                catch
                {

                }
                break;
            default:
                RemoveAugmentProperties();
                SetFitIcon((int)aL_allAugmentsOwned[i_currentAugmentIndex].at_type);
                ad_display.t_augmentName.text = aL_allAugmentsOwned[i_currentAugmentIndex].Name;
                ad_display.t_levelNumber.text = aL_allAugmentsOwned[_i_augmentIndexClicked]?.Level.ToString();
                UpdatePropertyText(aL_allAugmentsOwned[_i_augmentIndexClicked]);
                break;
        }

    }

    public void SetFitIcon(int f)
    {
        for (int i = 0; i < ad_display.goA_fitIcons.Length; i++)
        {
            ad_display.goA_fitIcons[i].SetActive(i == f);
        }
    }

    //This is never called in workbench???
    public void UpdatePropertyText(Augment _aug)
    {
        AugmentProperties ap = _aug.GetAugmentProperties();
        AugmentExplosion ae = _aug.GetExplosionProperties();

        float mod = _aug.GetAugmentLevelModifier(_aug.Level);

        //basically this looks through and checks all of the properties to see if they are different, if so then display them

        SetFitIcon((int)_aug.at_type);
        ad_display.t_augmentName.text = _aug.Name;
        ad_display.t_levelNumber.text = _aug.Level.ToString();

        //regular effects
        if (ap.f_weight != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Weight " + (ap.f_weight * mod).ToString("F1");

        }
        if (ap.i_damage != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Enemy Damage " + (ap.i_damage * mod).ToString("F1");

        }
        if (ap.i_lodeDamage != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Lode Damage " + (ap.i_lodeDamage * mod).ToString("F1");
        }
        if (ap.f_speed != 0)
        {
            switch (_aug.at_type)
            {
                case AugmentType.standard:
                    PlaceAugmentProperties(go_propertyText).text = "Attack Speed " + (ap.f_speed * mod).ToString("F1");
                    break;
                case AugmentType.projectile:
                    PlaceAugmentProperties(go_propertyText).text = "Fire Rate " + (ap.f_speed * mod).ToString("F1");
                    break;
                case AugmentType.cone:
                    PlaceAugmentProperties(go_propertyText).text = "Suck Speed " + (ap.f_speed * mod).ToString("F1");
                    break;

            }

        }
        if (ap.f_knockback != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Knockback " + (ap.f_knockback * mod).ToString("F1");

        }
        if (ap.f_energyGauge != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Energy Capacity " + (ap.f_energyGauge * mod).ToString("F1");

        }
        if (ap.f_heatsink != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Heatsink " + (ap.f_heatsink * mod).ToString("F1");

        }
        if (ap.f_recoil != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Recoil " + (ap.f_recoil * mod).ToString("F1");

        }

        if (!Utils.ArrayIsNullOrZero(_aug.AugElement))
            if (_aug.AugElement.Length > 0)
                foreach (Element elim in _aug.AugElement)
                {
                    Text elemText;
                    elemText = PlaceAugmentProperties(go_propertyText);
                    elemText.text = elim.ToString();
                    //switch (elim)
                    //{
                    //    case Element.goo:
                    //        elemText = PlaceAugmentProperties(go_propertyText);
                    //        elemText.text = "Goo";
                    //        //elemText.color = Color.magenta;
                    //        break;
                    //    case Element.hydro:
                    //        elemText = PlaceAugmentProperties(go_propertyText);
                    //        elemText.text = "Hydro";
                    //        //elemText.color = Color.blue;
                    //        break;
                    //    case Element.tasty:
                    //        PlaceAugmentProperties(go_propertyText).text = "Tasty";
                    //        break;
                    //    case Element.thunder:
                    //        elemText = PlaceAugmentProperties(go_propertyText);
                    //        elemText.text = "Thunder";
                    //        //elemText.color = Color.green;
                    //        break;
                    //    case Element.boom:
                    //        PlaceAugmentProperties(go_propertyText).text = "Boom";
                    //        break;
                    //    case Element.fire:
                    //        elemText = PlaceAugmentProperties(go_propertyText);
                    //        elemText.text = "Fire";
                    //        //elemText.color = Color.red;
                    //        break;
                    //    case Element.lava:
                    //        PlaceAugmentProperties(go_propertyText).text = "Lava";
                    //        break;
                    //    default:
                    //        break;
                    //}
                }

        // explosion effects

        if (ae.i_damage != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Explosion Enemy Damage " + (ae.i_damage * mod).ToString("F1");

        }
        if (ae.i_lodeDamage != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Explosion Lode Damage " + (ae.i_lodeDamage * mod).ToString("F1");

        }
        if (ae.f_explockBack != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Explosion Knockback " + (ae.f_explockBack * mod).ToString("F1");

        }
        if (ae.f_radius != 0)
        {
            PlaceAugmentProperties(go_propertyText).text = "Explosion Radius " + (ae.f_radius * mod).ToString("F1");

        }
        if (ae.b_impact)
        {
            PlaceAugmentProperties(go_propertyText).text = "Impact Explosion";

        }
        else
        {
            if (ae.f_detonationTime != 0)
            {
                PlaceAugmentProperties(go_propertyText).text = "Lifetime " + ae.f_detonationTime.ToString("F1");
            }
        }

        if (_aug is ProjectileAugment)
        {
            ProjectileAugment projectileCast;

            if (_aug.Stage == AugmentStage.full)
                projectileCast = AugmentManager.x.GetProjectileAugment(_aug.Name);
            else
            {
                int[] inds = AugmentManager.x.GetIndicesByName(_aug.Name);
                projectileCast = AugmentManager.x.GetProjectileAugmentAt(AugmentStage.fused, inds);
            }

            projectileCast.Level = _aug.Level;

            AugmentProjectile augmentProperties = projectileCast.GetProjectileData();

            if (augmentProperties.i_shotsPerRound != 0)
                PlaceAugmentProperties(go_propertyText).text = "Shots per round " + (augmentProperties.i_shotsPerRound * mod).ToString("F1");
            if (augmentProperties.f_bulletScale != 0)
                PlaceAugmentProperties(go_propertyText).text = "Bullet Size " + (augmentProperties.f_bulletScale * mod).ToString("F1");
            if (augmentProperties.f_gravity != 0)
                PlaceAugmentProperties(go_propertyText).text = "Bullet Weight " + (augmentProperties.f_gravity * mod).ToString("F1");

            if(augmentProperties.pm_phys == "Bouncy")
                PlaceAugmentProperties(go_propertyText).text = "Bouncy Bullets";

        }
        if (_aug is ConeAugment)
        {
            ConeAugment coneCast;

            if (_aug.Stage == AugmentStage.full)
                coneCast = AugmentManager.x.GetConeAugment(_aug.Name);
            else
            {
                int[] inds = AugmentManager.x.GetIndicesByName(_aug.Name);
                coneCast = AugmentManager.x.GetConeAugmentAt(AugmentStage.fused, inds);
            }

            AugmentCone coneProperties = coneCast.GetConeData();
            if (coneProperties.f_angle != 0)
                PlaceAugmentProperties(go_propertyText).text = "Cone Width " + (coneProperties.f_angle * mod).ToString("F1");
            if (coneProperties.f_radius != 0)
                PlaceAugmentProperties(go_propertyText).text = "Cone Length " + (coneProperties.f_radius * mod).ToString("F1");
        }

    }

    private Text PlaceAugmentProperties(GameObject _go_template)
    {

        //spawn a text to display the effects and augment will have
        //add it to the list
        //do some formatting shit to it

        GameObject propertyText = Instantiate(go_propertyText);

        goL_propertyList.Add(propertyText);
        propertyText.SetActive(true);
        RectTransform rt_button = propertyText.GetComponent<RectTransform>();
        propertyText.transform.parent = go_propertyParent.transform;
        propertyText.transform.localScale = Vector3.one;
        rt_button.sizeDelta = Vector2.up * 35;

        if (i_displayIter <= i_columnMax)
        {
            rt_button.anchoredPosition = new Vector2(rt_augmentButtonParent.rect.xMin + (rt_button.rect.width / 2), 0 - (i_yDisplacement * i_displayIter));
        }
        else
        {
            rt_button.anchoredPosition = new Vector2(rt_augmentButtonParent.rect.xMin + (rt_button.rect.width / 2) + i_xDisplacement, 0 - (34 * (i_displayIter - i_columnMax)));
        }

        i_displayIter++;

        return propertyText.GetComponent<Text>();
    }

    public List<Augment> DisplayCurrentType()
    {
        return InitAugmentList(aL_allAugmentsOwned, adt_currentDisplayType, false);
    }

    /// <summary>
    /// Destroys all properties that are being displayed and clears the list
    /// </summary>
    public void RemoveAugmentProperties()
    {

        //Get rid of currently displayed stuff
        SetFitIcon(-1);
        ad_display.t_augmentName.text = "";
        ad_display.t_levelNumber.text = "";

        //destroy all of the properties that are currently being displayed and then clear the list
        for (int i = 0; i < goL_propertyList.Count; i++)
        {
            Destroy(goL_propertyList[i]);
        }
        i_displayIter = 0;
        goL_propertyList.Clear();
    }
}

public enum AugmentDisplayType
{
    ShowAll,
    ShowEquipped,
    ShowSameType,
    ShowSameTypeNotFused,
    ShowSameTypeNotFusedExcluding,
    ShowSameName,
    ShowSameTypeExcluding,
    ShowSameNameExcluding,
    None
}