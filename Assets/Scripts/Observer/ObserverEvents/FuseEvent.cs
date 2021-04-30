using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseEvent : ObserverEvent
{

    AugmentSave savedAug;
    AugmentStage prevStage;
    int prevALevel;
    int prevBLevel;
    public AugmentSave SavedAug { get { return savedAug; } }
    public AugmentStage PreviousStage { get { return prevStage; } } 
    public int ALevel { get { return prevALevel; } }
    public int BLevel { get { return prevBLevel; } }
    public FuseEvent(AugmentSave _aug, AugmentStage _prevStage)
    {
        savedAug = _aug;
        prevStage = _prevStage;
    }
    public FuseEvent(AugmentSave _aug, AugmentStage _prevStage, int _aLevel, int _bLevel)
    {
        savedAug = _aug;
        prevStage = _prevStage;
        prevALevel = _aLevel;
        prevBLevel = _bLevel;
    }
}
