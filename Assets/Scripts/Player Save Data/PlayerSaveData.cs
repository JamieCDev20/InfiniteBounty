using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct PlayerSaveData
{
    
    public int i_totalNugs;
    public int i_currentNugs;
    public int i_zippyBank;
    public int[] A_appearance;
    public (int toolID, int slotID)[] tu_equipped;
    public (int toolID, int rackID)[] tu_toolsPurchased;
    public (int toolID, int slotID, AugmentSave[] equippedAugs)[] tu_equippedAugments;
    public AugmentSave[] purchasedAugments;
    public float[] A_playerSliderOptions;
    public int[] A_displaySettings;
    public bool b_inverted;
    public int i_difficulty;

    /// <summary>
    /// Create save data to be sent to the save manager
    /// </summary>
    /// <param name="_i_total">Total number of nuggs. -1 if you don't want to send nuggs.</param>
    /// <param name="_i_current">Current number of nuggs. -1 if you don't want to send nuggs.</param>
    /// <param name="_tb_tools">The tools currently equipped. Null if you don't want to update.</param>
    /// <param name="_tb_purchased">All purchased tools</param>
    /// <param name="_purchased">All purchased augments</param>
    /// <param name="_options">Settings</param>
    /// <param name="_diff">Difficulty. -1 if no changes.</param>
    public PlayerSaveData(int _i_total, int _i_current, int _i_zip, int[] _appearance, (int, int)[] _tu_equip, (int, int)[] _tu_purchased, (int, int, AugmentSave[])[] _tu_equippedAugs,  AugmentSave[] _purchased, SettingsValues _options, int _diff)
    {
        i_totalNugs = _i_total;
        i_currentNugs = _i_current;
        i_zippyBank = _i_zip;
        A_appearance = _appearance;
        tu_equipped = new (int toolID, int slotID)[3];
        if(_tu_equip != null)
            foreach ((int toolID, int slotID) weapon in _tu_equip)
            {
                if (weapon.slotID <= 3)
                    if(weapon.toolID != -1 && weapon.slotID != -1)
                        tu_equipped[weapon.slotID] = weapon;
            }
        tu_equipped = _tu_equip;
        tu_toolsPurchased = _tu_purchased;
        tu_equippedAugments = _tu_equippedAugs;
        purchasedAugments = _purchased;
        if(_options != null)
        {
            A_playerSliderOptions = _options.A_settingFloats;
            A_displaySettings = _options.displaySettings;
            b_inverted = _options.invert[0];
        }
        else
        {
            A_playerSliderOptions = null;
            A_displaySettings = null;
            b_inverted = false;
        }
        i_difficulty = _diff;
    }
}
