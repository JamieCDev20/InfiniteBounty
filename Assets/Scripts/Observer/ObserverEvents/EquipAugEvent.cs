using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipAugEvent : ObserverEvent
{
    public (int toolID, int slotID, AugmentSave[] aug) iia_equippedAugment;
    public EquipAugEvent((int, int, AugmentSave[]) _newAugment)
    {
        iia_equippedAugment = _newAugment;
    }
    public EquipAugEvent(int ID, int slot, AugmentSave[] _aug)
    {
        iia_equippedAugment = (ID, slot, _aug);
    }
}
