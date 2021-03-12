using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AugmentPropertyDisplayer : MonoBehaviour
{
    [SerializeField] private int i_xDisplacement;
    [SerializeField] private int i_yDisplacement;
    [SerializeField] private int i_columnMax;
    [SerializeField] private GameObject go_propertyButton;
    [SerializeField] private GameObject go_propertyParent;
    [SerializeField] private GameObject go_augmentButton;
    [SerializeField] private GameObject go_listMover;
    [SerializeField] private List<GameObject> goL_augmentButtonPool = new List<GameObject>();
    [SerializeField] private RectTransform rt_augmentButtonParent;
    [SerializeField] private Scrollbar s_slider;
    [SerializeField] private float f_augmentButtonHeight = 85;
    [SerializeField] private AugmentDisplay ad_display;
    List<Augment> aL_allAugmentsOwned = new List<Augment>();
    private int i_currentAugmentIndex = 0;

    private AugmentType at_type;
    public AugmentType AugType { set { at_type = value; } }
    public List<GameObject> AugmentButtons { get { return goL_augmentButtonPool; } }
    public AugmentDisplay AugDisplay { get { return ad_display; } }
    public int CurrentAugIndex { get { return i_currentAugmentIndex; } }
    private int i_displayIter = 0;

    public void Init()
    {

    }

    private void OnDestroy()
    {
        //if(!PoolManager.x.CheckIfPoolExists(go_propertyButton))
        //    PoolManager.x.RemovePool(go_propertyButton);
    }

    public List<Augment> InitAugmentList(List<Augment> aL_augs, AugmentDisplayType adt, bool _b_shouldAddToExistingList)
    {
        List<Augment> _augmentsInList = new List<Augment>();
        // Clear the display
        if (!_b_shouldAddToExistingList)
        {
            aL_augs.Clear();
            aL_allAugmentsOwned.Clear();
            foreach (GameObject btn in goL_augmentButtonPool)
                btn.GetComponent<IPoolable>().Die();
        }
        // Find all purchased Augments
        SaveManager saveMan = FindObjectOfType<SaveManager>();
        if (saveMan.SaveData.purchasedAugments != null)
        {
            Augment[] augs = saveMan.SaveData.purchasedAugments;
            Augment[] castedAugs = new Augment[augs.Length];
            if (augs != null && augs.Length != 0)
            {
                for (int i = 0; i < castedAugs.Length; i++)
                    if (AugmentManager.x.GetAugment(augs[i].Name) != null)
                    {
                        castedAugs[i] = AugmentManager.x.GetAugment(augs[i].Name).Aug;
                        _augmentsInList.Add(castedAugs[i]);
                    }
            }
        }
        // Update display from save file
        aL_allAugmentsOwned.AddRange(_augmentsInList);
        UpdateAugmentListDisplay(aL_allAugmentsOwned, adt);
        return aL_allAugmentsOwned;
    }

    private List<Augment> DisplayAugmentsOfType(List<Augment> aL_augs)
    {
        List<Augment> _augList = new List<Augment>();
        foreach (Augment aug in aL_augs)
            if (aug.at_type == at_type)
                _augList.Add(aug);
        return _augList;
    }

    private void UpdateAugmentListDisplay(List<Augment> aL_augs, AugmentDisplayType _adt_whichToShow)
    {
        List<Augment> _aL_augmentsToShow = new List<Augment>();
        // Currently only show all augments
        switch (_adt_whichToShow)
        {
            case AugmentDisplayType.ShowAll:
                _aL_augmentsToShow.AddRange(aL_augs);
                break;

            case AugmentDisplayType.ShowSameType:
                for (int i = 0; i < aL_augs.Count; i++)
                {
                    _aL_augmentsToShow.AddRange(DisplayAugmentsOfType(aL_augs));
                }
                break;
        }

        if(_aL_augmentsToShow != null)
        {
            for (int i = 0; i < _aL_augmentsToShow.Count; i++)
            {
                if (goL_augmentButtonPool.Count <= i)
                    goL_augmentButtonPool.Add(PoolManager.x.SpawnObject(go_augmentButton));
                goL_augmentButtonPool[i].SetActive(true);
                goL_augmentButtonPool[i].transform.parent = go_listMover.transform;
                goL_augmentButtonPool[i].transform.localPosition = new Vector3(0, (-i * f_augmentButtonHeight) - 70, 0);
                goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[0].text = _aL_augmentsToShow[i]?.Name;
                goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[1].text = "Lvl " + _aL_augmentsToShow[i]?.Level.ToString();
                goL_augmentButtonPool[i].transform.localScale = Vector3.one;
                AugmentButton btn = goL_augmentButtonPool[i].GetComponent<AugmentButton>();
                btn.i_buttonIndex = i;
                btn.Parent = transform.root.gameObject;
            }
        }

        rt_augmentButtonParent.sizeDelta = new Vector2(rt_augmentButtonParent.sizeDelta.x, f_augmentButtonHeight * (aL_augs.Count + 1));
        s_slider.value = 1;
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
        RemoveAugmentProperties();
        UpdatePropertyText(aL_allAugmentsOwned[_i_augmentIndexClicked]);
        //        UpdatePropertyText(_i_augmentIndexClicked);
    }
    public void UpdatePropertyText(Augment _aug)
    {
        AugmentProperties ap = _aug.GetAugmentProperties();
        AugmentExplosion ae = _aug.GetExplosionProperties();
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
            switch (_aug.at_type)
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
        if (_aug is ProjectileAugment)
        {
            ProjectileAugment projectileCast = (ProjectileAugment)_aug;
            AugmentProjectile augmentProperties = projectileCast.GetProjectileData();
            if (augmentProperties.i_shotsPerRound != 0)
                PlaceAugmentProperties(go_propertyButton).text = "Shots per round " + augmentProperties.i_shotsPerRound;
            if (augmentProperties.f_bulletScale != 0)
                PlaceAugmentProperties(go_propertyButton).text = "Bullet Size " + augmentProperties.f_bulletScale;
            if (augmentProperties.f_gravity != 0)
                PlaceAugmentProperties(go_propertyButton).text = "Bullet Weight " + augmentProperties.f_gravity;
        }
        if (_aug is ConeAugment)
        {
            ConeAugment coneCast = (ConeAugment)_aug;
            AugmentCone coneProperties = coneCast.GetConeData();
            if (coneProperties.f_angle != 0)
                PlaceAugmentProperties(go_propertyButton).text = "Cone Width " + coneProperties.f_angle;
            if (coneProperties.f_radius != 0)
                PlaceAugmentProperties(go_propertyButton).text = "Cone Length " + coneProperties.f_radius;
        }

    }
    private Text PlaceAugmentProperties(GameObject _go_template)
    {
        GameObject btn = PoolManager.x.SpawnObject(go_propertyButton);
        RectTransform rt_button = btn.GetComponent<RectTransform>();
        btn.transform.parent = go_propertyParent.transform;
        btn.transform.localScale = Vector3.one;
        if (i_displayIter <= i_columnMax)
        {

            rt_button.anchoredPosition = new Vector2(rt_augmentButtonParent.rect.xMin + (rt_button.rect.width / 2), 0 - (i_yDisplacement * i_displayIter));
        }
        else
        {
            rt_button.anchoredPosition = new Vector2(rt_augmentButtonParent.rect.xMin + (rt_button.rect.width / 2) + i_xDisplacement, 0 - (34 * (i_displayIter - i_columnMax)));
        }
        i_displayIter++;
        return btn?.GetComponent<Text>();
    }
    public void RemoveAugmentProperties()
    {
        i_displayIter = 0;
        PoolManager.x.KillAllObjects(go_propertyButton);
    }
}
