using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineHandle : MonoBehaviour
{
    private SlotMachine sm_slots;

    private void Start()
    {
        sm_slots =  GetComponentInParent<SlotMachine>();
    }

    private void OnMouseDown()
    {
        sm_slots.PullLever();
    }
}
