using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsValues
{
    public bool[] invert;
    public float[] A_settingFloats;
    public SettingsValues()
    {

    }
    public SettingsValues(bool[] _inv, float[] _A_settings)
    {
        invert = _inv;
        A_settingFloats = _A_settings;
    }
}
