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
    [SerializeField] private Text t_bonusText;
    [SerializeField] private GameObject go_bonusPart;

    public void SetupPlayer(GameObject _go_playerToSetup)
    {

        _go_playerToSetup.GetComponentInChildren<Animator>().SetTrigger("LevelStart");
        pim = _go_playerToSetup.GetComponent<PlayerInputManager>();
        go_cameraParent.SetActive(true);
        pim.GetCamera().enabled = false;
        _go_playerToSetup.transform.position = transform.position;

        int seed = Mathf.RoundToInt(UnityEngine.Random.value * 16581375);

        dimensionText.text = $"DIMENSION : {Convert.ToString(seed, 16)}";

        _go_playerToSetup.transform.forward = transform.forward;

        switch (DiversifierManager.x.ReturnBonusObjective())
        {
            case BonusObjective.None:
                go_bonusPart.SetActive(false);
                break;
            case BonusObjective.BonusGoo:
                t_bonusText.text = "Collect 400 Goo Nuggs";
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

        StartCoroutine(LateSets());
    }

    IEnumerator LateSets()
    {
        yield return new WaitForEndOfFrame();


        pim.SetMoving(false);
        pim.b_shouldPassInputs = false;
        pim.GetCamera().transform.localEulerAngles = new Vector3(5, 0, 0);


        Invoke("PlayerImpact", f_timeToWaitBeforeActivating);
        Invoke("PlayParticle", f_timeToPlayParticle);
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
    }

}