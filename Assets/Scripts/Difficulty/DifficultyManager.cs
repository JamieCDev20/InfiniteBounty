using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager x;

    [SerializeField] private List<DifficultySet> dsL_difficulties = new List<DifficultySet>();
    private int i_currentDifficulty;
    private int i_amountOfAuthoredDifs;
    [Space, SerializeField] private int i_maximumDifficulty;
    //Returns maximum difficulty as an index
    internal int MaximumDifficulty { get { return i_maximumDifficulty - 1; } }

    private void Awake()
    {
        if (x) Destroy(gameObject);
        else x = this;
        DontDestroyOnLoad(gameObject);
        i_amountOfAuthoredDifs = dsL_difficulties.Count;
    }

    internal DifficultySet ReturnCurrentDifficulty()
    {
        if (i_currentDifficulty >= dsL_difficulties.Count)
            CreateImpossibleDifficulty();

        return dsL_difficulties[i_currentDifficulty];
    }

    internal void ChangeCurrentDifficulty(int _i_newDiff)
    {
        i_currentDifficulty += _i_newDiff;

        if (i_currentDifficulty < 0)
            i_currentDifficulty = 0;
        if (i_currentDifficulty > i_maximumDifficulty)
            i_currentDifficulty = i_maximumDifficulty;

        if (i_currentDifficulty >= dsL_difficulties.Count)
            CreateImpossibleDifficulty();

        FindObjectOfType<SlotMachine>().SetDiversifiersByDifficulty(
            dsL_difficulties[i_currentDifficulty].dA_firstDiversifierSet,
            dsL_difficulties[i_currentDifficulty].dA_secondDiversifierSet,
            dsL_difficulties[i_currentDifficulty].dA_thirdDiversifierSet);
    }


    internal int ReturnCurrentDifficultyInt()
    {
        return i_currentDifficulty;
    }

    internal DifficultySet ReturnDifficultyByIndex(int _i_setToGet)
    {
        if (_i_setToGet >= dsL_difficulties.Count)
            CreateImpossibleDifficulty();

        return dsL_difficulties[_i_setToGet];
    }

    internal void IncreaseDifficultiesUnlocked()
    {
        i_maximumDifficulty++;
        print("YOU'VE UNLOCKED A NEW DIFFICULTY! IT SHOULD'VE SAVED THAT, BUT I DONT KNOW HOW");
    }

    #region Impossible Ranks

    private void CreateImpossibleDifficulty()
    {
        DifficultySet impossibleX = new DifficultySet();

        impossibleX.s_name = "Impossible " + ((i_currentDifficulty - i_amountOfAuthoredDifs + 2) > 1 ? (i_currentDifficulty - i_amountOfAuthoredDifs + 2).ToString() : "");

        dsL_difficulties.Add(impossibleX);
    }

    #endregion

}

[System.Serializable]
public struct DifficultySet
{
    [Header("Display Things")]
    public string s_name;

    [Header("Enemy Stats")]
    public float f_maxHealthMult; //A creature's current health value is set their max multiplied by this on start
    public float f_damageMult; //A creature's damage is multiplied by this
    public float f_scaleMult; //A creature's localScale is multiplied by this on start
    public float f_movementSpeedMult;
    public float f_decisionSpeedMult;
    public float f_accuracyMult;
    public float f_spawnFrequencyMult; //The number of seconds between waves is multiplied by this amount
    public float f_spawnAmountMult; //The number of enemies per wave is multiplied by this amount
    public float f_maxNumberOfEnemies;
    public float f_enemyVariantChance;
    public float f_goldEnemyChance;
    public int i_numberOfMiniBosses;

    [Header("Diversifiers")]
    public Diversifier[] dA_firstDiversifierSet;
    public Diversifier[] dA_secondDiversifierSet;
    public Diversifier[] dA_thirdDiversifierSet;
}
