using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerSaveData
{
    public int i_totalNugs;
    public int i_currentNugs;
    public ToolBase[] tb_equippedTools;
    public Augment[] purchasedAugments;
    public float[] A_playerSliderOptions;
    public bool b_inverted;
    public PlayerSaveData(int _i_total, int _i_current, ToolBase[] _tb_tools, Augment[] _purchased, SettingsValues _options)
    {
        i_totalNugs = _i_total;
        i_currentNugs = _i_current;
        tb_equippedTools = _tb_tools;
        purchasedAugments = _purchased;
        if(_options != null)
        {
            A_playerSliderOptions = _options.A_settingFloats;
            b_inverted = _options.invertY;
        }
        else
        {
            A_playerSliderOptions = null;
            b_inverted = false;
        }

    }
    public bool CheckNull()
    {
        if (i_currentNugs == 0 && i_totalNugs == 0 && A_playerSliderOptions == null && tb_equippedTools == null && purchasedAugments == null)
            return true;
        return false;
    }
}
