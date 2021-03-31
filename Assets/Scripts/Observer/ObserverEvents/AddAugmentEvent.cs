using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAugmentEvent : ObserverEvent
{
    public AugmentSave augToAdd;
    public AddAugmentEvent(AugmentSave _aug)
    {
        augToAdd = _aug;
    }
}
