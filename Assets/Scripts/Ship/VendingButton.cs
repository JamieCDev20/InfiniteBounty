using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingButton : MonoBehaviour
{
    [SerializeField] private int i_buttonIndex;
    [SerializeField] private VendingMachine vm_machine;

    private void OnMouseDown()
    {
        vm_machine.ClickedAugment(i_buttonIndex);
    }

}
