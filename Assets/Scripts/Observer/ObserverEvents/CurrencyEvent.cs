using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyEvent : ObserverEvent
{
    private int i_playerID;
    private int i_amountToChange;
    private bool b_addOrSubtract;
    private Nug n_nug;
    public int PlayerID { get { return i_playerID; } }
    public int AmountToChange { get { return i_amountToChange; } }
    public bool AddOrSubtract { get { return b_addOrSubtract; } }
    public Nug Nug { get { return n_nug; } }

    public CurrencyEvent(int _i_ID, int _i_amount, bool _b_add, Nug _n_nug)
    {
        i_playerID = _i_ID;
        i_amountToChange = _i_amount;
        b_addOrSubtract = _b_add;
        n_nug = _n_nug;
    }

}
