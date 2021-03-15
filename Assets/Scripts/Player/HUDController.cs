using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController x;


    [Header("Heat Guages")]
    [SerializeField] private RectTransform rt_healthBar;
    [SerializeField] private Image i_healthBar;
    [SerializeField] private GameObject go_healthbarParent;
    //[SerializeField] private RectTransform rt_rightHeatGuage;

    [Header("Health Stats")]
    [SerializeField] private List<Sprite> sA_faceSprites = new List<Sprite>();
    [SerializeField] private Image i_faceImage;
    [Space, SerializeField] private Image i_faceBackgroundImage;
    [SerializeField] private Gradient g_healthBarGradient;

    [Header("Nug Counter")]
    /*[SerializeField] private GameObject go_moneyUpParticle;
    private List<GameObject> goL_moneyUpParts = new List<GameObject>();
    [SerializeField] private GameObject go_moneyDownParticle;
    private List<GameObject> goL_moneyDownParts = new List<GameObject>();
    [SerializeField] private Text t_nugCountText;*/
    [SerializeField] private GameObject go_nugHudParent;
    [SerializeField] private GameObject go_bbObject;
    [SerializeField] private ScoreObjects texts;

    [Header("Other UI")]
    [SerializeField] private Transform hudCanvas;
    [SerializeField] private Text t_myNameText;

    [Header("Other Player's Health Bars")]
    [SerializeField] private RectTransform[] rtA_healthBars = new RectTransform[0];
    [SerializeField] private Text[] tA_playerNamesTexts = new Text[0];
    private Dictionary<int, int> iiD_idMap = new Dictionary<int, int>();
    [SerializeField] private GameObject[] goA_healthBarParents = new GameObject[0];

    private void Awake()
    {
        x = this;

    }

    private void Start()
    {
        SetHealthBarValue(1, 1);
        SetBBTotal();
        SceneManager.sceneLoaded += SceneLoad;

        int _int = 0;
        for (int i = 0; i < 4; i++)
        {
            if (i != NetworkedPlayer.x.PlayerID)
            {
                iiD_idMap.Add(i, _int);
                _int++;
            }
        }
        t_myNameText.text = PhotonNetwork.NickName;
    }

    public void SetHealthBarValue(float _i_currentHealth, int _i_maxHealth)
    {
        //print((float)_i_currentHealth / _i_maxHealth + "/" + Mathf.RoundToInt(((float)_i_currentHealth / _i_maxHealth) * sA_faceSprites.Count));
        //i_faceBackgroundImage.color = g_healthBarGradient.Evaluate((float)_i_currentHealth / _i_maxHealth);

        i_healthBar.color = g_healthBarGradient.Evaluate((float)_i_currentHealth / _i_maxHealth);
        rt_healthBar.localScale = new Vector3((float)_i_currentHealth / _i_maxHealth, 1, 1);
        i_faceImage.sprite = sA_faceSprites[Mathf.Clamp(Mathf.RoundToInt(((float)_i_currentHealth / _i_maxHealth) * sA_faceSprites.Count), 0, 4)];
    }

    public void SetNugValues(int[] _iA_nugCounts)
    {
        texts.gooText.text = _iA_nugCounts[0].ToString();
        texts.hydroText.text = _iA_nugCounts[1].ToString();
        texts.tastyText.text = _iA_nugCounts[2].ToString();
        texts.thunderText.text = _iA_nugCounts[3].ToString();
        texts.boomText.text = _iA_nugCounts[4].ToString();
        texts.magmaText.text = _iA_nugCounts[5].ToString();
    }

    internal void StartShowing()
    {
        hudCanvas.gameObject.SetActive(true);
    }
    internal void StopShowing()
    {
        hudCanvas.gameObject.SetActive(false);
    }

    public void SetBBTotal()
    {
        texts.bucksText.text = NetworkedPlayer.x?.GetLocalNugManager().Nugs.ToString();
    }

    public void SceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (this == null)
            return;
        HudInLevel(!scene.name.Contains("Lobby"));
    }

    private void HudInLevel(bool inLevel)
    {
        go_healthbarParent.gameObject.SetActive(inLevel);
        go_nugHudParent.SetActive(inLevel);
        go_bbObject.SetActive(!inLevel);



    }

    public Transform GetHudCanvasTransform()
    {
        return hudCanvas;
    }



    #region Other Player's Bars

    public void UpdateRemoteHealth(string _s_name, int id, float _i_currentHealth)
    {
        for (int i = 0; i < goA_healthBarParents.Length; i++)
            goA_healthBarParents[i].SetActive(false);

        if (iiD_idMap.Count <= 0)
            return;

        goA_healthBarParents[iiD_idMap[id]].SetActive(true);

        rtA_healthBars[iiD_idMap[id]].localScale = new Vector3((float)_i_currentHealth / 100, 1, 1);
        tA_playerNamesTexts[iiD_idMap[id]].text = _s_name;
    }


    #endregion
}
