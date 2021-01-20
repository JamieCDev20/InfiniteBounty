using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour, ObserverBase
{
    private PlayerSaveData saveData;
    public PlayerSaveData SaveData { get { return saveData; } }
    private const string rs = "/Resources/";
    private const string sv = "save.json";

    // Start is called before the first frame update
    void Start()
    {
        if (File.Exists(Application.dataPath + rs + sv))
        {
            string saveString = File.ReadAllText(Application.dataPath + rs + sv);
            if(saveString != string.Empty)
                saveData = JsonUtility.FromJson<PlayerSaveData>(saveString);
        }
        else
            File.Create(Application.dataPath + rs + sv);
        FindObjectOfType<Workbench>().Init(this);
    }

    public void OnNotify(ObserverEvent oe_event)
    {
        switch (oe_event)
        {
            case SaveEvent psd:
                saveData.i_currentNugs += psd.SaveData.i_currentNugs;
                saveData.i_totalNugs += psd.SaveData.i_totalNugs;
                foreach (Augment org in psd.SaveData.purchasedAugments)
                    Debug.Log(org.Name);
                if (saveData.purchasedAugments == null && psd.SaveData.purchasedAugments != null)
                {
                    saveData.purchasedAugments = psd.SaveData.purchasedAugments;
                }
                else if (saveData.purchasedAugments != null && psd.SaveData.purchasedAugments != null)
                {
                    saveData.purchasedAugments = Utils.CombineArrays(saveData.purchasedAugments, psd.SaveData.purchasedAugments);
                }
                string jsonData = JsonUtility.ToJson(saveData);
                File.WriteAllText(Application.dataPath + rs + sv, jsonData);
                break;
        }
    }
}
