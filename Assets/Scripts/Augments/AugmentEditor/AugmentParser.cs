using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AugmentParser
{
    public static string[] ParseAugmentData(string _s_jsonData)
    {
        List<string> augList = new List<string>();
        // Parse out each section. Magic numbers are bad but also...
        string augName = _s_jsonData.Split('=')[0];
        string augClassType = _s_jsonData.Split(':', '}')[1];
        string audioData = _s_jsonData.Split('=', '~')[3];
        string infoData = _s_jsonData.Split('=', '~')[5];
        string physicalData = _s_jsonData.Split('=', '~')[7];
        string explosionData = _s_jsonData.Split('=', '~')[9];
        string projectileData = _s_jsonData.Split('=', '~')[11];
        string coneData = _s_jsonData.Split('=', '~')[13];
        string meshData = _s_jsonData.Split('=', '~')[15];
        // Store the parsed versions into their own array slot.
        string[] arrayedData = new string[9];
        arrayedData[0] = augName;
        arrayedData[1] = augClassType;
        arrayedData[2] = audioData;
        arrayedData[3] = infoData;
        arrayedData[4] = physicalData;
        arrayedData[5] = explosionData;
        arrayedData[6] = projectileData;
        arrayedData[7] = coneData;
        arrayedData[8] = meshData;
        return arrayedData;
    }

    
}
