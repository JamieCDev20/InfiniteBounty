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
    [SerializeField] private List<GameObject> goL_augmentButtonPool = new List<GameObject>();
    [SerializeField] private RectTransform rt_augmentButtonParent;
    [SerializeField] private Scrollbar s_slider;
    [SerializeField] private float f_augmentButtonHeight = 85;
    [SerializeField] private AugmentDisplay ad_display;
    public List<GameObject> AugmentButtons { get { return goL_augmentButtonPool; } }
    public AugmentDisplay AugDisplay { get { return ad_display; } }
    private int i_displayIter = 0;

    public void Init()
    {
        if(go_propertyButton != null)
            if (!PoolManager.x.CheckIfPoolExists(go_propertyButton))
                PoolManager.x.CreateNewPool(go_propertyButton, 20);
    }

    private void OnDestroy()
    {
        if(!PoolManager.x.CheckIfPoolExists(go_propertyButton))
            PoolManager.x.RemovePool(go_propertyButton);
    }

    public List<Augment> InitAugmentList(List<Augment> aL_augs, Augment[] _aA_augmentsInList, bool _b_shouldAddToExistingList)
    {
        // Clear the display
        if (!_b_shouldAddToExistingList)
            aL_augs.Clear();
        // Update display from save file
        aL_augs.AddRange(_aA_augmentsInList);
        UpdateAugmentListDisplay(aL_augs, AugmentDisplayType.ShowAll);
        return aL_augs;
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
                    /*
                        if(aL_allAugmentsOwned[i].type == currentWeapon.type)
                            _aL_augmentsToShow.Add(aL_allAugmentsOwned[i]);
                    */
                }
                break;
        }

        for (int i = 0; i < aL_augs.Count; i++)
        {
            if (goL_augmentButtonPool.Count <= i)
                goL_augmentButtonPool.Add(Instantiate(go_augmentButton, rt_augmentButtonParent));
            goL_augmentButtonPool[i].SetActive(true);
            goL_augmentButtonPool[i].transform.localPosition = new Vector3(0, (-i * f_augmentButtonHeight) - 70, 0);
            goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[1].text = "Lvl " + aL_augs[i]?.Level.ToString();
            goL_augmentButtonPool[i].GetComponentsInChildren<Text>()[0].text = aL_augs[i]?.Name;
            goL_augmentButtonPool[i].GetComponent<AugmentButton>().i_buttonIndex = i;
        }

        rt_augmentButtonParent.sizeDelta = new Vector2(rt_augmentButtonParent.sizeDelta.x, f_augmentButtonHeight * (aL_augs.Count + 1));
        s_slider.value = 1;
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

            rt_button.anchoredPosition = new Vector2(rt_augmentButtonParent.rect.xMin + (rt_button.rect.width / 2), rt_augmentButtonParent.rect.yMax + (rt_button.rect.height) - (i_yDisplacement * i_displayIter));
        }
        else
        {
            rt_button.anchoredPosition = new Vector2(rt_augmentButtonParent.rect.xMin + (rt_button.rect.width / 2) + i_xDisplacement, rt_augmentButtonParent.rect.yMax + (rt_button.rect.height) - (34 * (i_displayIter - i_columnMax)));
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
