using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Heat Guages")]
    [SerializeField] private RectTransform rt_leftHeatGuage;
    [SerializeField] private RectTransform rt_rightHeatGuage;

    [Header("Health Stats")]
    [SerializeField] private List<Sprite> sA_faceSprites = new List<Sprite>();
    [SerializeField] private Image i_faceImage;
    [Space, SerializeField] private Image i_faceBackgroundImage;
    [SerializeField] private Gradient g_healthBarGradient;

    private void Start()
    {
        SetHealthBarValue(0, 1);
        SetLeftHeatGuage(0, 1);
        SetRightHeatGuage(0, 1);
    }

    public void SetHealthBarValue(int _i_currentHealth, int _i_maxHealth)
    {
        i_faceBackgroundImage.color = g_healthBarGradient.Evaluate(_i_currentHealth / _i_maxHealth);
        i_faceImage.sprite = sA_faceSprites[Mathf.RoundToInt((_i_currentHealth / _i_maxHealth) * sA_faceSprites.Count)];
    }

    public void SetLeftHeatGuage(int _i_currentHeat, int _i_maxHeat)
    {
        rt_leftHeatGuage.localScale = new Vector3(1 - (_i_currentHeat / _i_maxHeat), 1, 1);
    }

    public void SetRightHeatGuage(int _i_currentHeat, int _i_maxHeat)
    {
        rt_rightHeatGuage.localScale = new Vector3(1 - (_i_currentHeat / _i_maxHeat), 1, 1);
    }


}
