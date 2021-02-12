using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsValues
{
    public bool invertY;
    public float[] A_settingFloats;
    public SettingsValues()
    {

    }
    public SettingsValues(bool _y, float[] _A_settings)
    {
        invertY = _y;
        A_settingFloats = _A_settings;
    }
}
