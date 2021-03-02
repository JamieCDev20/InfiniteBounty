using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingButton : MonoBehaviour
{
    [SerializeField] private int i_buttonIndex = 0;
    [SerializeField] private VendingMachine vm_machine;

    private void Update()
    {
        if (vm_machine.IsBeingUsed)
        {
            RaycastHit _hit;
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
                if (Physics.Raycast(_ray, out _hit))
                    if (_hit.transform == transform)
                        vm_machine.ClickedAugment(i_buttonIndex);
        }
    }

}
