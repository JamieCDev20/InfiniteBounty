using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerSaveData
{
    public int i_totalNugs;
    public int i_currentNugs;
    public ToolBase[] tb_equippedTools;
    public Augment[] purchasedAugments;
    public float[] playerOptions;
    public PlayerSaveData(int _i_total, int _i_current, ToolBase[] _tb_tools, Augment[] _purchased, float[] _options)
    {
        i_totalNugs = _i_total;
        i_currentNugs = _i_current;
        tb_equippedTools = _tb_tools;
        purchasedAugments = _purchased;
        playerOptions = _options;
    }
}
