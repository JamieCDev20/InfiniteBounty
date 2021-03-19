using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipAugEvent : ObserverEvent
{
    public (int toolID, int slotID, Augment[] aug) iia_equippedAugment;
    public EquipAugEvent((int, int, Augment[]) _newAugment)
    {
        iia_equippedAugment = _newAugment;
    }
    public EquipAugEvent(int ID, int slot, Augment[] _aug)
    {
        iia_equippedAugment = (ID, slot, _aug);
    }
}
