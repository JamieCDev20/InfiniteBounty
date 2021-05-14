using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteInfoTextEvent : ObserverEvent
{
    private GameObject go_toRemove;
    public GameObject ToRemove { get { return go_toRemove; } }
    public DeleteInfoTextEvent(GameObject _remove)
    {
        go_toRemove = _remove;
    }
}
