using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentButton : MonoBehaviour
{
    public int i_buttonIndex;

    public void Clicked(GameObject wb)
    {
        wb?.GetComponent<Workbench>()?.ClickAugment(i_buttonIndex);
    }
}
