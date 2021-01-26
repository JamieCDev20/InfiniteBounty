using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineInfoButton : MonoBehaviour
{
    private SlotMachine sm_machine;
    [SerializeField] private int i_myIndex;

    void Start()
    {
        sm_machine = GetComponentInParent<SlotMachine>();
    }

    private void OnMouseDown()
    {
        sm_machine.DisplayDiversifierInfo(i_myIndex);
    }
}
