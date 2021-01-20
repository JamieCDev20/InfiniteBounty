using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerSaveData
{
    public int i_totalNugs;
    public int i_currentNugs;
    public Augment[] purchasedAugments;
    public PlayerSaveData(int _i_total, int _i_current, Augment[] _purchased)
    {
        i_totalNugs = _i_total;
        i_currentNugs = _i_current;
        purchasedAugments = _purchased;
    }
}
