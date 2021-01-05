using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyToolSlot : MonoBehaviour
{
    [SerializeField] private int i_toolID;
    [SerializeField] private int i_rackID;
    private ToolSlot ts_slot;

    public int ToolID { get { return i_toolID; } set { i_toolID = value; } }
    public int RackID { get { return i_rackID; } set { i_rackID = value; } }
    public ToolSlot Slot { get { return ts_slot; } set { ts_slot = value; } }


}
