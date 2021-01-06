using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Heat Guages")]
    [SerializeField] private RectTransform rt_healthBar;
    [SerializeField] private Image i_healthBar;
    //[SerializeField] private RectTransform rt_rightHeatGuage;

    [Header("Health Stats")]
    [SerializeField] private List<Sprite> sA_faceSprites = new List<Sprite>();
    [SerializeField] private Image i_faceImage;
    [Space, SerializeField] private Image i_faceBackgroundImage;
    [SerializeField] private Gradient g_healthBarGradient;

    [Header("Nug Counter")]
    [SerializeField] private GameObject go_moneyUpParticle;
    private List<GameObject> goL_moneyUpParts = new List<GameObject>();
    [SerializeField] private GameObject go_moneyDownParticle;
    private List<GameObject> goL_moneyDownParts = new List<GameObject>();

    private void Start()
    {
        SetHealthBarValue(1, 1);
        //SetLeftHeatGuage(1, 1);
        //SetRightHeatGuage(1, 1);
        for (int i = 0; i < 100; i++)
        {
            goL_moneyDownParts.Add(Instantiate(go_moneyDownParticle, go_moneyDownParticle.transform.parent));
            goL_moneyUpParts.Add(Instantiate(go_moneyUpParticle, go_moneyUpParticle.transform.parent));
        }
    }

    public void SetHealthBarValue(float _i_currentHealth, int _i_maxHealth)
    {
        //print((float)_i_currentHealth / _i_maxHealth + "/" + Mathf.RoundToInt(((float)_i_currentHealth / _i_maxHealth) * sA_faceSprites.Count));
        //i_faceBackgroundImage.color = g_healthBarGradient.Evaluate((float)_i_currentHealth / _i_maxHealth);

        i_healthBar.color = g_healthBarGradient.Evaluate((float)_i_currentHealth / _i_maxHealth);
        rt_healthBar.localScale = new Vector3((float)_i_currentHealth / _i_maxHealth, 1, 1);
        i_faceImage.sprite = sA_faceSprites[Mathf.Clamp(Mathf.RoundToInt(((float)_i_currentHealth / _i_maxHealth) * sA_faceSprites.Count), 0, 4)];
    }

    public void SetLeftHeatGuage(int _i_currentHeat, int _i_maxHeat)
    {
        rt_healthBar.localScale = new Vector3(1 - (_i_currentHeat / _i_maxHeat), 1, 1);
    }

    public void SetRightHeatGuage(int _i_currentHeat, int _i_maxHeat)
    {
        //rt_rightHeatGuage.localScale = new Vector3(1 - (_i_currentHeat / _i_maxHeat), 1, 1);
    }

    public void GainNug()
    {
        GameObject _go = goL_moneyUpParts[0];
        goL_moneyUpParts.RemoveAt(0);
        _go.SetActive(true);
        StartCoroutine(ReturnNugSignToPool(_go));
    }
    private IEnumerator ReturnNugSignToPool(GameObject _go)
    {
        yield return new WaitForSeconds(1);
        _go.SetActive(false);
        goL_moneyUpParts.Add(_go);
    }


    public void LoseMoney()
    {
        GameObject _go = goL_moneyDownParts[0];
        goL_moneyDownParts.RemoveAt(0);
        _go.SetActive(true);
        StartCoroutine(ReturnMoneyDownSignToPool(_go));
    }
    private IEnumerator ReturnMoneyDownSignToPool(GameObject _go)
    {
        yield return new WaitForSeconds(1);
        _go.SetActive(false);
        goL_moneyDownParts.Add(_go);
    }
}
