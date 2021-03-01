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
    [SerializeField] private RectTransform rt_augmentButtonParent;
    private int i_displayIter = 0;

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
