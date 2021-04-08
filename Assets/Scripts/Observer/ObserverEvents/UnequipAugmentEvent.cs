using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnequipAugmentEvent : ObserverEvent
{
    public (int toolID, int slotID, AugmentSave augs) augsToUnequip;
    public UnequipAugmentEvent((int, int, AugmentSave) _unequip)
    {
        augsToUnequip = _unequip;
    }
    public UnequipAugmentEvent(int _tool, int _slot, AugmentSave _save)
    {
        augsToUnequip.toolID = _tool;
        augsToUnequip.slotID = _slot;
        augsToUnequip.augs = _save;
    }
}
