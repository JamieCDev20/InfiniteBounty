using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour, ObserverBase
{
    private PlayerSaveData saveData;
    public PlayerSaveData SaveData { get { return saveData; } }
    private const string sv = "save.json";

    // Start is called before the first frame update
    void Start()
    {
        CreateSaveData();
        FindObjectOfType<Workbench>().Init(this);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Create Player Save Data reference from file
    /// </summary>
    public void CreateSaveData()
    {
        if (File.Exists(Application.persistentDataPath + sv))
        {
            string saveString = File.ReadAllText(Application.persistentDataPath + sv);
            if (saveString != string.Empty)
            {
                // The save data must have all the required data to be read
                if (CheckIfSaveDataClean(saveString))
                {
                    saveData = JsonConvert.DeserializeObject<PlayerSaveData>(saveString);
                }
                else
                {
                    // Rip out any usable data
                    saveData = UpdateSaveData(saveString);
                    File.WriteAllText(Application.persistentDataPath + sv, JsonUtility.ToJson(saveData));
                }
            }
        }
        else
            File.Create(Application.persistentDataPath + sv);
    }

    /// <summary>
    /// Checks if the save data contains all the data required
    /// </summary>
    /// <param name="_saveData">Save data to check</param>
    /// <returns>true if the save data is clean</returns>
    private bool CheckIfSaveDataClean(string _saveData)
    {
        if (!_saveData.Contains("i_totalNugs")) return false;
        if (!_saveData.Contains("i_currentNugs")) return false;
        if (!_saveData.Contains("i_zippyBank")) return false;
        if (!_saveData.Contains("A_appearance")) return false;
        if (!_saveData.Contains("tu_equippedAugments")) return false;
        if (!_saveData.Contains("tu_toolsPurchased")) return false;
        if (!_saveData.Contains("tu_equippedAugments")) return false;
        if (!_saveData.Contains("purchasedAugments")) return false;
        if (!_saveData.Contains("A_playerSliderOptions")) return false;
        if (!_saveData.Contains("A_displaySettings")) return false;
        if (!_saveData.Contains("b_inverted")) return false;
        if (!_saveData.Contains("i_difficulty")) return false;
        return true;
    }

    /// <summary>
    /// Replace all text in file with an empty string
    /// </summary>
    public void ClearSaveData()
    {
        if (File.Exists(Application.persistentDataPath + sv))
            File.WriteAllText(Application.persistentDataPath + sv, string.Empty);
    }

    /// <summary>
    /// Read the data and take any usable field
    /// </summary>
    /// <param name="_saveData">Current Save File</param>
    /// <returns>New Player Save data</returns>
    public PlayerSaveData UpdateSaveData(string _saveData)
    {
        PlayerSaveData psd = new PlayerSaveData();
        // Manually parse out old data and create new data from it
        string[] totalNugsString = _saveData.Split(':', ',');
        for (int i = 0; i < totalNugsString.Length; i++)
        {
            if (totalNugsString[i].Contains("i_totalNugs"))
            {
                psd.i_totalNugs = int.Parse(totalNugsString[i+1].Split(',')[0]);
            }
            if (totalNugsString[i].Contains("i_currentNugs"))
            {
                psd.i_currentNugs = int.Parse(totalNugsString[i+1].Split(',')[0]);
            }
            if (totalNugsString[i].Contains("i_zippyBank"))
            {
                psd.i_zippyBank = int.Parse(totalNugsString[i + 1].Split(',')[0]);
            }
            if (totalNugsString[i].Contains("A_appearance"))
            {
                psd.A_appearance = ReadArrayFromJson<int>(_saveData, new string[] { "A_appearance\":[" }, '}');
            }
            if (totalNugsString[i].Contains("tu_equipped"))
            {
                psd.tu_equipped = ReadArrayFromJson<(int, int)>(_saveData, new string[] { "tu_equipped" }, '}');
            }
            if (totalNugsString[i].Contains("tu_toolsPurchased"))
            {
                psd.tu_toolsPurchased = ReadArrayFromJson<(int, int)>(_saveData, new string[] { "tu_toolsPurchased" }, '}');
            }
            if (totalNugsString[i].Contains("tu_equippedAugments"))
            {
                psd.tu_equippedAugments = ReadArrayFromJson<(int, int, Augment[])>(_saveData, new string[] { "tu_equippedAugments" }, '}');
            }
            else if (totalNugsString[i].Contains("purchasedAugments"))
            {
                psd.purchasedAugments = ReadArrayFromJson<Augment>(_saveData, new string[] { "purchasedAugments\":["}, '}');
                foreach (Augment aug in psd.purchasedAugments)
                    Debug.Log(aug.Name);
            }
            else if (totalNugsString[i].Contains("A_playerSliderOptions"))
            {
                int floatSep = 0;
                for (int j = i; j < totalNugsString.Length; j++)
                    if (totalNugsString[j].Contains("b_inverted"))
                        floatSep = j;
                List<float> floatVals = new List<float>();
                for(int j = i+1; j < floatSep; j++)
                {
                    string newFloat = totalNugsString[j];
                    if (newFloat.Contains("["))
                    {
                        newFloat = newFloat.Substring(1);
                    }
                    if (newFloat.Contains(","))
                    {
                        newFloat = newFloat.Replace(',', '\0');
                    }
                    if (newFloat.Contains("]"))
                    {
                        newFloat = newFloat.Replace(']', '\0');
                        if(string.IsNullOrEmpty(newFloat) || newFloat != "\0")
                            floatVals.Add(float.Parse(newFloat));
                        break;
                    }
                    if(string.IsNullOrEmpty(newFloat) || newFloat != "\0")
                        floatVals.Add(float.Parse(newFloat));
                }
                psd.A_playerSliderOptions = floatVals.ToArray();
            }
            else if (totalNugsString[i].Contains("b_inverted"))
            {
                if (totalNugsString[i + 1].Contains("}"))
                {
                    totalNugsString[i + 1] = totalNugsString[i + 1].Replace('}', '\0');
                }
                psd.b_inverted = bool.Parse(totalNugsString[i + 1]);
            }
            else if (totalNugsString[i].Contains("A_displaySettings"))
            {
                psd.A_displaySettings = ReadArrayFromJson<int>(_saveData, new string[] { "A_displaySettings\":[" }, ']');
            }
            else if (totalNugsString[i].Contains("i_difficulty"))
            {
                string nugString = totalNugsString[i + 1];
                if (nugString.Contains("}"))
                {
                    nugString = nugString.Replace('}', '\0');
                }
                psd.i_difficulty = int.Parse(nugString);
            }
        }
        return psd;
    }

    private T[] ReadArrayFromJson<T>(string _saveData, string[] _saveSeperators, char _lineSeperator)
    {
        string newData = _saveData.Split(_saveSeperators, System.StringSplitOptions.None)[1];
        int sepCount = 0;
        List<T> output = new List<T>();
        if(newData[0] != ']')
        {
            for (int i = 0; i < newData.Length; i++)
                if (newData[i] == _lineSeperator)
                    sepCount++;

            for (int i = 0; i < sepCount; i++)
            {
                string jsonData = newData.Split(_lineSeperator)[i];
                if (jsonData[0] == ']')
                {
                    break;
                }
                if (jsonData[0] == ',')
                    jsonData = jsonData.Substring(1);
                jsonData += '}';
                output.Add(JsonConvert.DeserializeObject<T>(jsonData));
            }
        }
        return output.ToArray();
    }

    public void OnNotify(ObserverEvent oe_event)
    {
        switch (oe_event)
        {
            case SaveEvent psd:
                // Get current and Total nuggets
                if(psd.SaveData.i_currentNugs != -1)
                    saveData.i_currentNugs = psd.SaveData.i_currentNugs;

                if(psd.SaveData.i_totalNugs != -1)
                    saveData.i_totalNugs = psd.SaveData.i_totalNugs;

                if (psd.SaveData.i_zippyBank != -1)
                    saveData.i_zippyBank = psd.SaveData.i_zippyBank;
                if (psd.SaveData.tu_equipped != null)
                {
                    // if there's no previously equipped tools, then just equip the tool.
                    if(saveData.tu_equipped == null)
                    {
                        saveData.tu_equipped = psd.SaveData.tu_equipped;
                    }
                    else if(psd.SaveData.tu_equipped != null)
                    {
                        foreach((int toolID, int slotID) tool in psd.SaveData.tu_equipped)
                        {
                            if (tool.toolID == -1 || tool.slotID == -1)
                                continue;
                            switch (tool.slotID)
                            {
                                case (int)ToolSlot.leftHand:
                                    saveData.tu_equipped[(int)ToolSlot.leftHand] = tool;
                                    break;
                                case (int)ToolSlot.rightHand:
                                    saveData.tu_equipped[(int)ToolSlot.rightHand] = tool;
                                    break;
                                case (int)ToolSlot.moblility:
                                    saveData.tu_equipped[(int)ToolSlot.moblility] = tool;
                                    break;
                            }
                        }
                    }
                }

                if(saveData.tu_equippedAugments == null && psd.SaveData.tu_equippedAugments != null)
                {
                    saveData.tu_equippedAugments = psd.SaveData.tu_equippedAugments;
                }
                else if(saveData.tu_equippedAugments != null && psd.SaveData.tu_equippedAugments != null)
                {
                    bool comb = false;
                    for(int i = 0; i< psd.SaveData.tu_equippedAugments.Length; i++)
                    {
                        comb = false;
                        for(int j = 0; j < saveData.tu_equippedAugments.Length; j++)
                        {
                            if(saveData.tu_equippedAugments[j].toolID == psd.SaveData.tu_equippedAugments[i].toolID && saveData.tu_equippedAugments[j].slotID == psd.SaveData.tu_equippedAugments[i].slotID)
                            {
                                List<Augment> augs = new List<Augment>();
                                foreach(Augment aug in psd.SaveData.tu_equippedAugments[i].equippedAugs)
                                {
                                    switch (aug.at_type)
                                    {
                                        case AugmentType.cone:
                                            augs.Add((ConeAugment)aug);
                                            break;
                                        case AugmentType.projectile:
                                            augs.Add((ProjectileAugment)aug);
                                            break;
                                        case AugmentType.standard:
                                            augs.Add(aug);
                                            break;
                                    }
                                }
                                psd.SaveData.tu_equippedAugments[i].equippedAugs = augs.ToArray();
                                saveData.tu_equippedAugments[j].equippedAugs = Utils.CombineArrays(saveData.tu_equippedAugments[j].equippedAugs, psd.SaveData.tu_equippedAugments[i].equippedAugs);
                                comb = true;
                            }
                        }
                        if (!comb)
                            saveData.tu_equippedAugments = Utils.AddToArray(saveData.tu_equippedAugments, psd.SaveData.tu_equippedAugments[i]);
                    }
                }
                if (saveData.purchasedAugments == null && psd.SaveData.purchasedAugments != null)
                {
                    saveData.purchasedAugments = psd.SaveData.purchasedAugments;
                }
                else if (saveData.purchasedAugments != null && psd.SaveData.purchasedAugments != null)
                {
                    saveData.purchasedAugments = Utils.CombineArrays(saveData.purchasedAugments, psd.SaveData.purchasedAugments);
                }

                if (psd.SaveData.A_playerSliderOptions != null)
                {
                    saveData.A_playerSliderOptions = psd.SaveData.A_playerSliderOptions;
                }

                if(psd.SaveData.A_displaySettings != null)
                {
                    saveData.A_displaySettings = psd.SaveData.A_displaySettings;
                }

                if (psd.SaveData.i_difficulty != -1)
                {
                    saveData.i_difficulty = psd.SaveData.i_difficulty;
                }


                string jsonData = JsonConvert.SerializeObject(saveData);
                File.WriteAllText(Application.persistentDataPath + sv, jsonData);
                break;
        }
    }

    private bool CheckStringArray(string _data)
    {
        if (_data.Contains("[") && _data.Contains("]"))
            return false;
        else if (_data.Contains("[") && !_data.Contains("]"))
            return true;
        else if (!_data.Contains("[") && _data.Contains("]"))
            return true;
        else return false;
    }
}
