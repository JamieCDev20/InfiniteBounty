using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour, ObserverBase
{

    public static SaveManager x;
    private PlayerSaveData saveData;
    public PlayerSaveData SaveData { get { return saveData; } }
    private const string sv = "save.json";

    // Start is called before the first frame update
    void Start()
    {
    }


    public void Init()
    {
        if (x != null)
        {
            if (x != this)
                Destroy(gameObject);
        }
        else
            x = this;


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
            if (totalNugsString[i].Contains("tu_appearance"))
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

                if (psd.SaveData.A_appearance != null)
                    saveData.A_appearance = psd.SaveData.A_appearance;
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
                    foreach((int, int, Augment[]) blaug in psd.SaveData.tu_equippedAugments)
                        EquipNewAugment(blaug);
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
            case RemoveAugmentEvent rae:
                saveData.purchasedAugments = Utils.OrderedRemove<Augment>(saveData.purchasedAugments, rae.augIndex);
                string removedPurchaseData = JsonConvert.SerializeObject(saveData);
                File.WriteAllText(Application.persistentDataPath + sv, removedPurchaseData);
                break;
            case AddAugmentEvent aae:
                saveData.purchasedAugments = Utils.AddToArray<Augment>(saveData.purchasedAugments, aae.augToAdd);
                string addedPurchaseData = JsonConvert.SerializeObject(saveData);
                File.WriteAllText(Application.persistentDataPath + sv, addedPurchaseData);
                break;
            case UpdateToolsEvent ute:
                if (Utils.ArrayIsNullOrZero(saveData.tu_equipped))
                {
                    saveData.tu_equipped = new (int, int)[1] { (ute.toolToEquip.ToolID, (int)ute.slotToEquipIn) };
                    break;
                }
                for (int i = 0; i < saveData.tu_equipped.Length; i++)
                {
                    if(saveData.tu_equipped[i].slotID == (int)ute.slotToEquipIn)
                    {
                        saveData.tu_equipped[i] = (ute.toolToEquip.ToolID, (int)ute.slotToEquipIn);
                        break;
                    }
                }
                saveData.tu_equipped = Utils.AddToArray(saveData.tu_equipped, (ute.toolToEquip.ToolID, (int)ute.slotToEquipIn));
                string updatedTools = JsonConvert.SerializeObject(saveData);
                File.WriteAllText(Application.persistentDataPath + sv, updatedTools);
                break;
            case EquipAugEvent eae:
                EquipNewAugment(eae.iia_equippedAugment);
                string equippedAugs = JsonConvert.SerializeObject(saveData);
                File.WriteAllText(Application.persistentDataPath + sv, equippedAugs);
                break;
            case UnequipAugmentEvent uae:
                saveData.purchasedAugments = Utils.CombineArrays(saveData.purchasedAugments, uae.augsToUnequip.augs);
                RemoveEquippedAugments(uae.augsToUnequip);
                string unequippedAugs = JsonConvert.SerializeObject(saveData);
                File.WriteAllText(Application.persistentDataPath + sv, unequippedAugs);
                break;
        }
    }

    private void EquipNewAugment((int _toolID, int _slotID, Augment[] _aug) newEquip)
    {
        // If the array doesn't already exist, add the incoming equipped augs.
        if (Utils.ArrayIsNullOrZero(saveData.tu_equippedAugments))
        {
            saveData.tu_equippedAugments = new (int, int, Augment[])[] { newEquip };
        }
        else
        {
            // Check the current passed in augment
            for (int j = 0; j < newEquip._aug.Length; j++)
            {
                bool _jCombined = false;
                for (int i = 0; i < saveData.tu_equippedAugments.Length; i++)
                {
                    // When the tool and slot ID's match, add the augments together.
                    if (saveData.tu_equippedAugments[i].toolID == newEquip._toolID && saveData.tu_equippedAugments[i].slotID == newEquip._slotID)
                    {
                        saveData.tu_equippedAugments[i].equippedAugs = Utils.CombineArrays(saveData.tu_equippedAugments[i].equippedAugs, newEquip._aug);
                        _jCombined = true;
                    }
                }
                if (!_jCombined)
                {
                    // It hasn't been combined so add it to the end of the arrays.
                    saveData.tu_equippedAugments = Utils.AddToArray(saveData.tu_equippedAugments, newEquip);
                }
            }
        }
    }

    private void RemoveEquippedAugments((int _toolID, int _slotID, Augment[] _aug) augToDetach)
    {
        if (Utils.ArrayIsNullOrZero(saveData.tu_equippedAugments))
        {
            return;
        }
        for(int i = 0; i < augToDetach._aug.Length; i++)
        {
            RemoveAugment((augToDetach._toolID, augToDetach._slotID, augToDetach._aug[i]));
        }
    }

    private void RemoveAugment((int _toolID, int _slotID, Augment _aug) augToDetach)
    {
        for(int i = 0; i < saveData.tu_equippedAugments.Length; i++)
        {
            if(augToDetach._toolID == saveData.tu_equippedAugments[i].toolID && augToDetach._slotID == saveData.tu_equippedAugments[i].slotID)
            {
                for(int j = 0; j < saveData.tu_equippedAugments[i].equippedAugs.Length; j++)
                {
                    if (saveData.tu_equippedAugments[i].equippedAugs[j].Name == augToDetach._aug.Name)
                    {
                        saveData.tu_equippedAugments[i].equippedAugs = Utils.OrderedRemove(saveData.tu_equippedAugments[i].equippedAugs, j);
                    }
                }
            }
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
