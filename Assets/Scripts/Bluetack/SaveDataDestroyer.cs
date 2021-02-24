using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataDestroyer : MonoBehaviour
{
    public void DestroySaveData()
    {
        FindObjectOfType<SaveManager>()?.ClearSaveData();
    }
}
