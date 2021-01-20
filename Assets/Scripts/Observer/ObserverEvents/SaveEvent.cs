using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveEvent : ObserverEvent
{
    private PlayerSaveData psd;
    public PlayerSaveData SaveData { get { return psd; } }

    public SaveEvent(PlayerSaveData _psd)
    {
        psd = _psd;
    }
}
