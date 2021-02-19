using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    public void CreateSaveData()
    {
        if (File.Exists(Application.persistentDataPath + sv))
        {
            string saveString = File.ReadAllText(Application.persistentDataPath + sv);
            if (saveString != string.Empty)
            {
                saveData = JsonUtility.FromJson<PlayerSaveData>(saveString);
                if (saveData.CheckNull())
                {
                    saveData = UpdateSaveData(saveString);
                    File.WriteAllText(Application.persistentDataPath + sv, JsonUtility.ToJson(saveData));
                }
            }
        }
        else
            File.Create(Application.persistentDataPath + sv);
    }

    public void ClearSaveData()
    {
        if (File.Exists(Application.persistentDataPath + sv))
            File.WriteAllText(Application.persistentDataPath + sv, string.Empty);
    }

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
            else if (totalNugsString[i].Contains("i_currentNugs"))
            {
                psd.i_currentNugs = int.Parse(totalNugsString[i+1].Split(',')[0]);
            }
            else if (totalNugsString[i].Contains("tb_equippedTools"))
            {
                psd.tb_equippedTools = ReadArrayFromJson<WeaponTool>(_saveData, new string[] { "tb_equippedTools\":["}, '}');
            }
            else if (totalNugsString[i].Contains("tb_purchasedTools"))
            {
                psd.tb_purchasedTools = ReadArrayFromJson<ToolBase>(_saveData, new string[] { "tb_purchasedTools" }, '}');
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
                        Debug.Log(newFloat);
                        floatVals.Add(float.Parse(newFloat));
                        break;
                    }
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
            else if (totalNugsString[i].Contains("i_difficulty"))
            {
                psd.i_difficulty = int.Parse(totalNugsString[i + 1]);
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
                    break;
                if (jsonData[0] == ',')
                    jsonData = jsonData.Substring(1);
                jsonData += '}';
                Debug.Log(jsonData);
                output.Add(JsonUtility.FromJson<T>(jsonData));
            }
        }
        return output.ToArray();

    }

    public void OnNotify(ObserverEvent oe_event)
    {
        switch (oe_event)
        {
            case SaveEvent psd:
                saveData.i_currentNugs = psd.SaveData.i_currentNugs;
                saveData.i_totalNugs = psd.SaveData.i_totalNugs;
                saveData.tb_equippedTools = psd.SaveData.tb_equippedTools;
                if (saveData.purchasedAugments == null && psd.SaveData.purchasedAugments != null)
                {
                    saveData.purchasedAugments = psd.SaveData.purchasedAugments;
                }
                else if (saveData.purchasedAugments != null && psd.SaveData.purchasedAugments != null)
                {
                    saveData.purchasedAugments = Utils.CombineArrays(saveData.purchasedAugments, psd.SaveData.purchasedAugments);
                }
                if(saveData.tb_purchasedTools == null && psd.SaveData.tb_purchasedTools != null)
                {
                    saveData.tb_purchasedTools = psd.SaveData.tb_purchasedTools;
                }
                else if(saveData.tb_purchasedTools != null && psd.SaveData.tb_purchasedTools != null)
                {
                    saveData.tb_purchasedTools = Utils.CombineArrays(saveData.tb_purchasedTools, psd.SaveData.tb_purchasedTools);
                }
                if (psd.SaveData.A_playerSliderOptions != null)
                {
                    saveData.A_playerSliderOptions = psd.SaveData.A_playerSliderOptions;
                }
                if (psd.SaveData.i_difficulty != 0)
                {
                    saveData.i_difficulty = psd.SaveData.i_difficulty;
                }
                string jsonData = JsonUtility.ToJson(saveData);
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
