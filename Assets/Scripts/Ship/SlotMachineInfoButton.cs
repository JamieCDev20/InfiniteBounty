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

    private void Update()
    {
        if (sm_machine.IsBeingUsed)
        {
            RaycastHit _hit;
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
                if (Physics.Raycast(_ray, out _hit))
                    if (_hit.transform == transform)
                        sm_machine.DisplayDiversifierInfo(i_myIndex);
        }
    }
}
