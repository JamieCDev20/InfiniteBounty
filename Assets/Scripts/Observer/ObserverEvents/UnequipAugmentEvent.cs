using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnequipAugmentEvent : ObserverEvent
{
    public (int toolID, int slotID, Augment[] augs) augsToUnequip;
    public UnequipAugmentEvent((int, int, Augment[]) _unequip)
    {
        augsToUnequip = _unequip;
    }
}
