using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAugmentEvent : ObserverEvent
{
    public Augment augToAdd;
    public AddAugmentEvent(Augment _aug)
    {
        augToAdd = _aug;
    }
}
