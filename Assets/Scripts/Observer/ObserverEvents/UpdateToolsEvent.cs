using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateToolsEvent : ObserverEvent
{
    public ToolBase toolToEquip;
    public ToolSlot slotToEquipIn;
    public UpdateToolsEvent(ToolBase _tool, ToolSlot _slot)
    {
        toolToEquip = _tool;
        slotToEquipIn = _slot;
    }
}
