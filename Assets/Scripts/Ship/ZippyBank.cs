using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZippyBank : MonoBehaviour, IInteractible
{
    private int i_heldBBs;
    [SerializeField] private TextMeshPro tmp_currentMoneyText;


    private void Start()
    {
        transform.localScale = Vector3.one + Vector3.one * (i_heldBBs * 0.00001f);
    }



    public void Interacted() { }

    public void Interacted(Transform interactor)
    {
        NugManager nugMan = interactor.GetComponent<NugManager>();
        if (nugMan.Nugs > 999)
        {
            nugMan.CollectNugs(-1000, false);
            i_heldBBs += 1000;
            tmp_currentMoneyText.text = "£" + i_heldBBs;
        }
        transform.localScale = Vector3.one + Vector3.one * (i_heldBBs * 0.00001f);
    }
}
