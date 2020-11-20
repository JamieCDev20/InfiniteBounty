﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardInfoSetter : MonoBehaviour
{
    [Header("Player Faces")]
    [SerializeField] private Image[] iA_playerFaceImages = new Image[0];

    [Header("Nugget Collected Info")]
    [SerializeField] private Text[] tA_nuggetCollectedTexts = new Text[0];

    [Header("Random Taxes")]
    [SerializeField] private Text t_randomTaxTextOneHeader;
    [SerializeField] private Text[] tA_randomTaxTextsOne = new Text[0];
    [SerializeField] private Text t_randomTaxTextTwoHeader;
    [SerializeField] private Text[] tA_randomTaxTextsTwo = new Text[0];
    [Space, SerializeField] private string[] sA_randomTaxStarts = new string[0];
    [SerializeField] private string[] sA_randomTaxEnds = new string[0];

    [Header("Total Texts")]
    [SerializeField] private Text[] tA_totalsVert = new Text[0];
    [SerializeField] private Text[] tA_totalsHori = new Text[0];
    [SerializeField] private Text t_absoluteTotal;


    [Header("TEST VALUES")]
    [SerializeField] private Sprite[] TEMP_sA_playerFaces = new Sprite[0];

    private void Start()
    {
        GainStats(TEMP_sA_playerFaces,
            new int[] { Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000) },
            new int[] { Random.Range(0, 5000), Random.Range(0, 5000), Random.Range(0, 5000), Random.Range(0, 5000) },
            new int[] { Random.Range(0, 5000), Random.Range(0, 5000), Random.Range(0, 5000), Random.Range(0, 5000) });
        SetTaxOneHeader();
        SetTaxTwoHeader();

        for (int i = 0; i < 4; i++)
        {
            if (PlayerPrefs.HasKey($"{i}NugCount"))
                SetPlayerNuggetYield(i, PlayerPrefs.GetInt($"{i}NugCount"));
            else
                SetPlayerNuggetYield(i, 0);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_sA_playerFaces">The sprites of the player's faces, in order from P1 to P4.</param>
    /// <param name="_iA_nuggetsCollectedPerPlayer">The int value of nuggets collected by that player.</param>
    /// <param name="_iA_randomTaxPerPlayerOne">The amount of tax to be applied to this player.</param>
    /// <param name="_iA_randomTaxPerPlayerTwo">The amount of tax to be applied to this player, but again.</param>
    public void GainStats(Sprite[] _sA_playerFaces, int[] _iA_nuggetsCollectedPerPlayer, int[] _iA_randomTaxPerPlayerOne, int[] _iA_randomTaxPerPlayerTwo)
    {
        int[] _iA_totalCostPerPlayer = new int[4];
        int _i_netGain = 0;

        int _i_totalTaxOne = 0;
        int _i_totalTaxTwo = 0;
        int _i_totalHarvest = 0;

        for (int i = 0; i < 4; i++)
        {
            iA_playerFaceImages[i].sprite = _sA_playerFaces[i];
            tA_nuggetCollectedTexts[i].text = "£" + _iA_nuggetsCollectedPerPlayer[i];

            _iA_totalCostPerPlayer[i] += _iA_nuggetsCollectedPerPlayer[i];
            _i_totalHarvest += _iA_nuggetsCollectedPerPlayer[i];

            _iA_totalCostPerPlayer[i] -= _iA_randomTaxPerPlayerOne[i];
            _iA_totalCostPerPlayer[i] -= _iA_randomTaxPerPlayerTwo[i];

            tA_randomTaxTextsOne[i].text = "-£" + _iA_randomTaxPerPlayerOne[i];
            _i_totalTaxOne -= _iA_randomTaxPerPlayerOne[i];
            tA_randomTaxTextsTwo[i].text = "-£" + _iA_randomTaxPerPlayerTwo[i];
            _i_totalTaxTwo -= _iA_randomTaxPerPlayerTwo[i];

            tA_totalsVert[i].text = "£" + _iA_totalCostPerPlayer[i];

            _i_netGain += _iA_totalCostPerPlayer[i];
        }

        tA_totalsHori[0].text = "£" + _i_totalHarvest;
        tA_totalsHori[1].text = "£" + _i_totalTaxOne;
        tA_totalsHori[2].text = "£" + _i_totalTaxTwo;
        t_absoluteTotal.text = "£" + _i_netGain;
    }

    public void SetTaxOneHeader(string _s_taxHeader)
    {
        t_randomTaxTextOneHeader.text = _s_taxHeader;
    }
    public void SetTaxOneHeader()
    {
        t_randomTaxTextOneHeader.text = sA_randomTaxStarts[Random.Range(0, sA_randomTaxStarts.Length)] + " " + sA_randomTaxEnds[Random.Range(0, sA_randomTaxEnds.Length)];
    }

    public void SetTaxTwoHeader(string _s_taxHeader)
    {
        t_randomTaxTextTwoHeader.text = _s_taxHeader;
    }
    public void SetTaxTwoHeader()
    {
        t_randomTaxTextTwoHeader.text = sA_randomTaxStarts[Random.Range(0, sA_randomTaxStarts.Length)] + " " + sA_randomTaxEnds[Random.Range(0, sA_randomTaxEnds.Length)];
    }

    public void SetPlayerNuggetYield(int index, int yield)
    {
        tA_nuggetCollectedTexts[index].text = "£" + yield;
    }

}