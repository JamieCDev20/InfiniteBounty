using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : SubjectBase
{
    public static DifficultyManager x;

    [SerializeField] private List<DifficultySet> dsL_difficulties = new List<DifficultySet>();
    private int i_currentDifficulty;
    [SerializeField]private int i_amountOfAuthoredDifs;
    [Space, SerializeField] private int i_maximumDifficulty;
    //Returns maximum difficulty as an index
    internal int MaximumDifficulty { get { return i_maximumDifficulty - 1; } }

    [Header("Impossibles")]
    [SerializeField] private DifficultySet ds_changeInStatsPerImpossible;

    private void Awake()
    {
    }

//#if UNITY_EDITOR

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            i_maximumDifficulty++;
    }

//#endif

    public void Init()
    {
        if (x) Destroy(gameObject);
        else x = this;
        DontDestroyOnLoad(gameObject);

        i_maximumDifficulty = FindObjectOfType<SaveManager>().SaveData.i_difficulty + 1;
        if (i_maximumDifficulty == 1)
            i_maximumDifficulty = 5;

        //i_amountOfAuthoredDifs = dsL_difficulties.Count;
        AddObserver(FindObjectOfType<SaveManager>());

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

        FindObjectOfType<SlotMachine>().SetDiversifiersByDifficulty(dsL_difficulties[i_currentDifficulty].dA_diversifierSet);
    }


    internal int ReturnCurrentDifficultyInt()
    {
        return i_currentDifficulty;
    }

    internal void SetCurrentDifficulty(int i)
    {
        i_currentDifficulty = i;

        if (i_currentDifficulty < 0)
            i_currentDifficulty = 0;
        /*if (i_currentDifficulty > i_maximumDifficulty)
            i_currentDifficulty = i_maximumDifficulty;*/

        if (i_currentDifficulty >= dsL_difficulties.Count)
            CreateImpossibleDifficulty();

        FindObjectOfType<SlotMachine>().SetDiversifiersByDifficulty(dsL_difficulties[i_currentDifficulty].dA_diversifierSet);
    }

    internal DifficultySet ReturnDifficultyByIndex(int _i_setToGet)
    {
        if (_i_setToGet >= dsL_difficulties.Count)
            CreateImpossibleDifficulty();

        return dsL_difficulties[_i_setToGet];
    }

    internal void IncreaseDifficultiesUnlocked()
    {
        if (i_currentDifficulty >= i_maximumDifficulty)
            i_maximumDifficulty++;
        Notify(new SaveEvent(new PlayerSaveData(-1, -1, -1, null, null, null, null, null, null, i_maximumDifficulty)));
    }

    #region Impossible Ranks

    private void CreateImpossibleDifficulty()
    {
        DifficultySet impossibleX = new DifficultySet();
        int _i_currentImpossible = (i_currentDifficulty - i_amountOfAuthoredDifs + 2);

        //Display Things
        impossibleX.s_name = "Impossible " + (_i_currentImpossible > 1 ? _i_currentImpossible.ToString() : "");
        impossibleX.f_moneyMult += (ds_changeInStatsPerImpossible.f_moneyMult * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_moneyMult;

        //Enemy Stats
        impossibleX.f_maxHealthMult += (ds_changeInStatsPerImpossible.f_maxHealthMult * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_maxHealthMult;
        impossibleX.f_damageMult += (ds_changeInStatsPerImpossible.f_damageMult * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_damageMult;
        impossibleX.f_scaleMult += (ds_changeInStatsPerImpossible.f_scaleMult * _i_currentImpossible) + +dsL_difficulties[i_amountOfAuthoredDifs - 1].f_scaleMult;
        impossibleX.f_movementSpeedMult += (ds_changeInStatsPerImpossible.f_movementSpeedMult * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_movementSpeedMult;
        impossibleX.f_decisionSpeedMult += (ds_changeInStatsPerImpossible.f_decisionSpeedMult * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_decisionSpeedMult;
        impossibleX.f_accuracyMult += (ds_changeInStatsPerImpossible.f_accuracyMult * _i_currentImpossible) + +dsL_difficulties[i_amountOfAuthoredDifs - 1].f_accuracyMult;
        impossibleX.f_spawnFrequencyMult += (ds_changeInStatsPerImpossible.f_spawnFrequencyMult * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_spawnFrequencyMult;
        impossibleX.f_spawnAmountMult += (ds_changeInStatsPerImpossible.f_spawnAmountMult * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_spawnAmountMult;
        impossibleX.f_maxNumberOfEnemies += (ds_changeInStatsPerImpossible.f_maxNumberOfEnemies * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_maxNumberOfEnemies;
        impossibleX.f_enemyVariantChance += (ds_changeInStatsPerImpossible.f_enemyVariantChance * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_enemyVariantChance;
        impossibleX.f_goldEnemyChance += (ds_changeInStatsPerImpossible.f_goldEnemyChance * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_goldEnemyChance;
        impossibleX.i_numberOfMiniBosses += (ds_changeInStatsPerImpossible.i_numberOfMiniBosses * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].i_numberOfMiniBosses;

        //Diversifiers
        impossibleX.dA_diversifierSet = dsL_difficulties[i_amountOfAuthoredDifs - 1].dA_diversifierSet;

        //Boss
        impossibleX.f_bossHealthMult += (ds_changeInStatsPerImpossible.f_bossHealthMult * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].f_bossHealthMult;
        impossibleX.vi_enemiesPerBossWave += (ds_changeInStatsPerImpossible.vi_enemiesPerBossWave * _i_currentImpossible) + dsL_difficulties[i_amountOfAuthoredDifs - 1].vi_enemiesPerBossWave;

        dsL_difficulties.Add(impossibleX);
    }
    #endregion


    internal void SetNewMaxDifficulty(int _i)
    {
        i_maximumDifficulty = _i;
    }

}

[System.Serializable]
public struct DifficultySet
{
    [Header("Display & Common")]
    public string s_name;
    public float f_moneyMult;

    [Header("Enemy Stats")]
    public float f_maxHealthMult; //A creature's current health value is set their max multiplied by this on start
    public float f_damageMult; //A creature's damage is multiplied by this
    public float f_scaleMult; //A creature's localScale is multiplied by this on start
    public float f_movementSpeedMult; //A creature's movement is multiplied this when they move
    public float f_decisionSpeedMult;
    public float f_accuracyMult;
    public float f_spawnFrequencyMult; //The number of seconds between waves is multiplied by this amount
    public float f_spawnAmountMult; //The number of enemies per wave is multiplied by this amount
    [Tooltip("The actual cap on the number of enemies that can be alive at any given time")]
    public float f_maxNumberOfEnemies; //Used in the if statement when checking the number of living enemies
    public float f_enemyVariantChance;
    public float f_goldEnemyChance;
    public int i_numberOfMiniBosses; //The number of indices added to the miniboss spawn list 

    [Header("Diversifiers")]
    public Diversifier[] dA_diversifierSet; //The diversifiers avaliable on each wheel

    [Header("Boss")]
    public float f_bossHealthMult; //The boss's health is multiplied by this on start
    public Vector2Int vi_enemiesPerBossWave; //The number of flappers the boss spawns periodically

}