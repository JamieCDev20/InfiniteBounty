using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviourPunCallbacks
{

    public static HUDController x;

    [Header("Heat Gauges")]
    [SerializeField] private RectTransform rt_healthBar;
    [SerializeField] private Image i_healthBar;
    [SerializeField] private GameObject go_healthbarParent;
    //[SerializeField] private RectTransform rt_rightHeatGuage;

    [Header("Health Stats")]
    [SerializeField] private List<SpriteArray> saA_faceSprites = new List<SpriteArray>();
    [Space, SerializeField] private Image i_faceImage;

    [SerializeField] private Gradient g_healthBarGradient;

    [Header("Nug Counter")]
    [SerializeField] private GameObject go_nugHudParent;
    [SerializeField] private GameObject go_bbObject;
    [SerializeField] private ScoreObjects texts;
    private int[] iA_nugcounts = new int[6];
    [SerializeField] private ParticleSystem[] psA_nugGainParticles = new ParticleSystem[0];

    [Space]
    [Header("Other UI")]
    [SerializeField] private Transform hudCanvas;

    [SerializeField] private Text t_myNameText;

    [Header("Other Player's Health Bars")]
    [SerializeField] private RectTransform[] rtA_healthBars = new RectTransform[0];
    [SerializeField] private Text[] tA_playerNamesTexts = new Text[0];
    private Dictionary<int, int> iiD_idMap = new Dictionary<int, int>();
    [SerializeField] private GameObject[] goA_healthBarParents = new GameObject[0];

    [Header("Tools")]
    [SerializeField] private GameObject go_teleportSign;
    private int i_headSprite;

    [Header("Crosshair")]
    [SerializeField] private RectTransform[] rtA_crossHairBits;

    [Header("Kill Timer")]
    [SerializeField] private GameObject go_killDisplay;
    [SerializeField] private Text t_killTimerText;
    [SerializeField] private Image i_killWarningImage;

    [Header("Bonus")]
    //[SerializeField] private Text t_bonusText;
    [SerializeField] private GameObject go_bonusParent;
    [SerializeField] private GameObject[] goA_nuggOutlines = new GameObject[0];

    private void Awake()
    {
        x = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeHeadSpriteIcon(int _i_headIndex)
    {
        i_headSprite = _i_headIndex;
        SetHealthBarValue(1, 1);
    }

    private void Start()
    {
        x = this;
        SetHealthBarValue(1, 1);
        SetBBTotal();
        SceneManager.sceneLoaded -= SceneLoad;
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

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoad;
    }

    public void ChangeBonusObjective(BonusObjective _bo_newBonus)
    {
        goA_nuggOutlines[(int)_bo_newBonus].SetActive(true);

        /*
        switch (_bo_newBonus)
        {
            case BonusObjective.None:
                go_bonusParent.SetActive(false);
                break;
            case BonusObjective.BonusGoo:
                break;
            case BonusObjective.BonusHydro:
                t_bonusText.text = "Collect 400 Hydro Nuggs";
                break;
            case BonusObjective.BonusTasty:
                t_bonusText.text = "Collect 400 Tasty Nuggs";
                break;
            case BonusObjective.BonusThunder:
                t_bonusText.text = "Collect 400 Thunder Nuggs";
                break;
            case BonusObjective.BonusBoom:
                t_bonusText.text = "Collect 400 Boom Nuggs";
                break;
            case BonusObjective.BonusMagma:
                t_bonusText.text = "Collect 400 Magma Nuggs";
                break;
        }
        */
    }

    public void SetHealthBarValue(float _i_currentHealth, int _i_maxHealth)
    {
        //print((float)_i_currentHealth / _i_maxHealth + "/" + Mathf.RoundToInt(((float)_i_currentHealth / _i_maxHealth) * sA_faceSprites.Count));
        //i_faceBackgroundImage.color = g_healthBarGradient.Evaluate((float)_i_currentHealth / _i_maxHealth);
        i_healthBar.color = g_healthBarGradient.Evaluate((float)_i_currentHealth / _i_maxHealth);
        rt_healthBar.localScale = new Vector3((float)_i_currentHealth / _i_maxHealth, 1, 1);


        i_faceImage.sprite = saA_faceSprites[i_headSprite].sA_sprites[2];

        if (_i_currentHealth < _i_maxHealth * 0.5f)
            i_faceImage.sprite = saA_faceSprites[i_headSprite].sA_sprites[1];

        if (_i_currentHealth <= 0)
            i_faceImage.sprite = saA_faceSprites[i_headSprite].sA_sprites[0];
    }

    public void SetNugValues(int[] _iA_nugCounts)
    {
        texts.gooText.text = _iA_nugCounts[0].ToString();
        texts.hydroText.text = _iA_nugCounts[1].ToString();
        texts.tastyText.text = _iA_nugCounts[2].ToString();
        texts.thunderText.text = _iA_nugCounts[3].ToString();
        texts.boomText.text = _iA_nugCounts[4].ToString();
        texts.magmaText.text = _iA_nugCounts[5].ToString();

        for (int i = 0; i < _iA_nugCounts.Length; i++)
        {
            /*
             * if (iA_nugcounts[i] != _iA_nugCounts[i])
                psA_nugGainParticles[i].Play();
            */
            iA_nugcounts[i] = _iA_nugCounts[i];
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        StopShowing();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        StopShowing();
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
        if (texts != null)
            if (texts.bucksText != null)
                texts.bucksText.text = NetworkedPlayer.x?.GetLocalNugManager().Nugs.ToString();
    }

    public void SceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (x == null)
            x = this;

        bool inLevel = !scene.name.Contains("obby");
        HudInLevel(inLevel);
        foreach (int i in iiD_idMap.Keys)
        {
            RemovePlayersBar(i);
        }
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

    public void SetCrosshairSize(float _f_size)
    {
        //Left Square
        rtA_crossHairBits[0].localPosition = (Vector3.left * _f_size) + Vector3.left * 10;
        //Right Square
        rtA_crossHairBits[1].localPosition = (Vector3.right * _f_size) + Vector3.right * 10;
        //Top Square
        rtA_crossHairBits[2].localPosition = (Vector3.up * _f_size) + Vector3.up * 10;
        //Bottom Square
        rtA_crossHairBits[3].localPosition = (Vector3.down * _f_size) + Vector3.down * 10;
    }


    #region Other Player's Bars

    public void UpdateRemoteHealth(string _s_name, int id, float _i_currentHealth, bool down)
    {
        if (iiD_idMap.Count <= 0)
            return;

        goA_healthBarParents[iiD_idMap[id]].SetActive(true);
        goA_healthBarParents[iiD_idMap[id]].transform.GetChild(0).gameObject.SetActive(down);
        rtA_healthBars[iiD_idMap[id]].localScale = new Vector3((float)_i_currentHealth / 100, 1, 1);
        tA_playerNamesTexts[iiD_idMap[id]].text = _s_name;
    }

    public void RemovePlayersBar(int id)
    {
        goA_healthBarParents[iiD_idMap[id]].SetActive(false);
    }

    #endregion

    #region Teleported
    internal void HideTeleportSign()
    {
        go_teleportSign?.SetActive(false);
    }

    internal void ShowTeleportSign()
    {
        go_teleportSign?.SetActive(true);
    }

    #endregion


    [System.Serializable]
    private struct SpriteArray
    {
        public Sprite[] sA_sprites;
    }

    internal void HideKillTimer()
    {
        go_killDisplay.SetActive(false);
    }

    internal void ShowKillTimer(float _f_time)
    {
        go_killDisplay.SetActive(true);
        t_killTimerText.text = Mathf.RoundToInt(_f_time).ToString();
        i_killWarningImage.color = new Color(1, 0, 0, Mathf.Abs(Mathf.Sin(_f_time + _f_time)) * 0.1f);
    }

}
