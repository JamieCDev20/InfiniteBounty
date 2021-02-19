using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsValues
{
    public bool[] invert;
    public bool fullScreen;
    public float[] A_settingFloats;
    public int[] displaySettings;

    public SettingsValues()
    {

    }
    public SettingsValues(bool[] _inv, bool _full, float[] _A_settings, int[] _display)
    {
        invert = _inv;
        fullScreen = _full;
        A_settingFloats = _A_settings;
        displaySettings = _display;
    }
}
