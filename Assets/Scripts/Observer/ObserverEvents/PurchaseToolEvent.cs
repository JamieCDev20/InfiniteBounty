using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseToolEvent : ObserverEvent
{
    (int i_toolID, int i_rackID, bool b_isWeapon) purchasedTool;
    public (int, int, bool) PurchasedTool { get { return purchasedTool; } }
    public PurchaseToolEvent((int, int, bool) _pTool)
    {
        purchasedTool = _pTool;
    }
    public PurchaseToolEvent(int _tool, int _rack, bool _isWeapon)
    {
        purchasedTool.i_toolID = _tool;
        purchasedTool.i_rackID = _rack;
        purchasedTool.b_isWeapon = _isWeapon;
    }
}
