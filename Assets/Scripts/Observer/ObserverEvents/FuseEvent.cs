using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseEvent : ObserverEvent
{

    AugmentSave savedAug;
    AugmentStage prevStage;
    public AugmentSave SavedAug { get { return savedAug; } }
    public AugmentStage PreviousStage { get { return prevStage; } } 
    public FuseEvent(AugmentSave _aug, AugmentStage _prevStage)
    {
        savedAug = _aug;
        prevStage = _prevStage;
    }
}
