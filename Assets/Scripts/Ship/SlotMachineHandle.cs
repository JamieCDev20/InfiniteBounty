using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineHandle : MonoBehaviour
{
    private SlotMachine sm_slots;

    private void Start()
    {
        sm_slots = GetComponentInParent<SlotMachine>();
    }

    private void Update()
    {
        if (sm_slots.IsBeingUsed)
        {
            RaycastHit _hit;
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
                if (Physics.Raycast(_ray, out _hit))
                    if (_hit.transform == transform)
                        sm_slots.PullLever();
        }
    }
}
