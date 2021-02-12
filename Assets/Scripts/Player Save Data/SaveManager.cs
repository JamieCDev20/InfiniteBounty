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
                saveData = JsonUtility.FromJson<PlayerSaveData>(saveString);
        }
        else
            File.Create(Application.persistentDataPath + sv);
    }

    public void ClearSaveData()
    {
        if (File.Exists(Application.persistentDataPath + sv))
            File.WriteAllText(Application.persistentDataPath + sv, string.Empty);
    }

    public void OnNotify(ObserverEvent oe_event)
    {
        switch (oe_event)
        {
            case SaveEvent psd:
                saveData.i_currentNugs += psd.SaveData.i_currentNugs;
                saveData.i_totalNugs += psd.SaveData.i_totalNugs;
                if (saveData.purchasedAugments == null && psd.SaveData.purchasedAugments != null)
                {
                    saveData.purchasedAugments = psd.SaveData.purchasedAugments;
                }
                else if (saveData.purchasedAugments != null && psd.SaveData.purchasedAugments != null)
                {
                    saveData.purchasedAugments = Utils.CombineArrays(saveData.purchasedAugments, psd.SaveData.purchasedAugments);
                }

                if(saveData.playerOptions != null && psd.SaveData.playerOptions != null)
                {
                    saveData.playerOptions = psd.SaveData.playerOptions; 
                }
                string jsonData = JsonUtility.ToJson(saveData);
                File.WriteAllText(Application.persistentDataPath + sv, jsonData);
                break;
        }
    }
}
