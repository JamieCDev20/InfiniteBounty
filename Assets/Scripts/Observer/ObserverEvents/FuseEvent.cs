using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseEvent : ObserverEvent
{

    AugmentSave savedAug;
    public AugmentSave SavedAug { get { return savedAug; } }
    public FuseEvent(AugmentSave _aug)
    {
        savedAug = _aug;
    }
}
