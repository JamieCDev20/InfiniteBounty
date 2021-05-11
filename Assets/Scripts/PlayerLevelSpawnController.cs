using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class PlayerLevelSpawnController : MonoBehaviour
{

    private PlayerInputManager pim;
    [SerializeField] private GameObject go_impactEffects;
    [SerializeField] private float f_fireForce = 10;
    private PhotonView view;
    private bool b_hasDid;
    [SerializeField] private float f_timeToWaitBeforeActivating;
    [SerializeField] private float f_timeToPlayParticle = 1;
    [SerializeField] private GameObject go_cameraParent;

    [Header("UI References")]
    [SerializeField] private Text dimensionText;
    [SerializeField] private Image i_bonusText;
    [SerializeField] private GameObject go_bonusPart;

    [Header("Risk Section")]
    [SerializeField] private Text t_riskLevelText;
    [SerializeField] private Text t_riskEffectsText;
    [SerializeField] private Image[] iA_diversifierImages = new Image[0];

    [Header("Cross Faders")]
    [SerializeField] private Image[] iA_imagesToFadeIn = new Image[0];
    [SerializeField] private Text[] iA_textToFadeIn = new Text[0];

    [Header("Nugg Sprites")]
    [SerializeField] private Sprite s_magamSprite;
    [SerializeField] private Sprite s_boomSprite;
    [SerializeField] private Sprite s_thunderSprite;
    [SerializeField] private Sprite s_tastySprite;
    [SerializeField] private Sprite s_hydroSprite;
    [SerializeField] private Sprite s_gooSprite;

    public void SetupPlayer(GameObject _go_playerToSetup)
    {

        _go_playerToSetup.GetComponentInChildren<Animator>().SetTrigger("LevelStart");
        pim = _go_playerToSetup.GetComponent<PlayerInputManager>();
        go_cameraParent.SetActive(true);
        pim.GetCamera().enabled = false;
        _go_playerToSetup.transform.position = transform.position;

        int seed = Mathf.RoundToInt(UnityEngine.Random.value * 16581375);

        dimensionText.text = $"DIMENSION: {Convert.ToString(seed, 16)}";

        _go_playerToSetup.transform.forward = transform.forward;

        SetupRiskDisplay();
        SetupDiverDisplay();

        for (int i = 0; i < iA_imagesToFadeIn.Length; i++)
            iA_imagesToFadeIn[i].CrossFadeAlpha(0, 0, true);

        for (int i = 0; i < iA_textToFadeIn.Length; i++)
            iA_textToFadeIn[i].CrossFadeAlpha(0, 0, true);

        switch (DiversifierManager.x.ReturnBonusObjective())
        {
            case BonusObjective.None:
                go_bonusPart.SetActive(false);
                break;
            case BonusObjective.BonusGoo:
                i_bonusText.sprite = s_gooSprite;
                break;
            case BonusObjective.BonusHydro:
                i_bonusText.sprite = s_hydroSprite;
                break;
            case BonusObjective.BonusTasty:
                i_bonusText.sprite = s_tastySprite;
                break;
            case BonusObjective.BonusThunder:
                i_bonusText.sprite = s_thunderSprite;
                break;
            case BonusObjective.BonusBoom:
                i_bonusText.sprite = s_boomSprite;
                break;
            case BonusObjective.BonusMagma:
                i_bonusText.sprite = s_magamSprite;
                break;
        }

        StartCoroutine(LateSets());

        for (int i = 0; i < iA_imagesToFadeIn.Length; i++)
            iA_imagesToFadeIn[i].CrossFadeAlpha(1, 1, true);

        for (int i = 0; i < iA_textToFadeIn.Length; i++)
            iA_textToFadeIn[i].CrossFadeAlpha(1, 1, true);

    }

    private void SetupRiskDisplay()
    {
        t_riskLevelText.text = "Risk Level: " + DifficultyManager.x.ReturnCurrentDifficulty().s_name;
        t_riskEffectsText.text =
            "Enemy Toughness: " + DifficultyManager.x.ReturnCurrentDifficulty().f_scaleMult + "x \n" +
            "Enemy Power: " + DifficultyManager.x.ReturnCurrentDifficulty().f_damageMult + "x \n" +
            "Bounty Bucks Multiplier: " + DifficultyManager.x.ReturnCurrentDifficulty().f_moneyMult + "x";
    }

    private void SetupDiverDisplay()
    {
        for (int i = 0; i < iA_diversifierImages.Length; i++)
            iA_diversifierImages[i].sprite = DiversifierManager.x.ReturnActiveDiversifierDisplayInfo(i).s_image;
    }

    IEnumerator LateSets()
    {
        HUDController.x.StopShowing();
        yield return new WaitForEndOfFrame();


        pim.SetMoving(false);
        pim.b_shouldPassInputs = false;
        pim.GetCamera().transform.localEulerAngles = new Vector3(5, 0, 0);


        Invoke("PlayerImpact", f_timeToWaitBeforeActivating);
        Invoke("PlayParticle", f_timeToPlayParticle);

        yield return new WaitForSeconds(f_timeToWaitBeforeActivating - 1);

        for (int i = 0; i < iA_imagesToFadeIn.Length; i++)
            iA_imagesToFadeIn[i].CrossFadeAlpha(0, 1, true);

        for (int i = 0; i < iA_textToFadeIn.Length; i++)
            iA_textToFadeIn[i].CrossFadeAlpha(0, 1, true);
    }

    private void PlayParticle()
    {
        go_impactEffects.SetActive(true);
    }

    public void PlayerImpact()
    {
        pim.b_shouldPassInputs = true;
        pim.GetCamera().enabled = true;
        pim.SetMoving(true);
        go_cameraParent.SetActive(false);

        HUDController.x.StartShowing();
    }

}