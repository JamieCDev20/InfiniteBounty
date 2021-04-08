using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAugmentEvent : ObserverEvent
{
    public AugmentSave augToRemove;
    public RemoveAugmentEvent(AugmentSave _save)
    {
        augToRemove = _save;
    }
}
