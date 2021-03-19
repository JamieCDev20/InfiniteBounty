using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAugmentEvent : ObserverEvent
{
    public Augment augToRemove;
    public int augIndex;
    public RemoveAugmentEvent(Augment _aug, int _index)
    {
        augToRemove = _aug;
        augIndex = _index;
    }
}
