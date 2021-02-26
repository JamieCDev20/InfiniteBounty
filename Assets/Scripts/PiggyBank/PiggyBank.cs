using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PiggyBank : SubjectBase, IInteractible
{
    [SerializeField] private int i_targetAmount;
    [SerializeField] private int i_inputAmount;
    [SerializeField] private int i_storedAmount;
    [SerializeField] GameObject go_portal;
    private DifficultyManager dm_man;
    [SerializeField] private TextMeshPro tmp_currentMoneyText;
    [SerializeField] private GameObject go_stand;

    public void Start()
    {
        if (DifficultyManager.x.MaximumDifficulty < 9)
            gameObject.SetActive(false);
                
        SaveManager _sm = FindObjectOfType<SaveManager>();
        i_storedAmount = _sm.SaveData.i_zippyBank;
        if (i_storedAmount != 0)
            tmp_currentMoneyText.text = "£" + i_storedAmount;
        AddObserver(_sm);
        transform.localScale = Vector3.one + Vector3.one * (i_storedAmount * 0.00001f);
    }

    public void Interacted() { }

    public void Interacted(Transform interactor)
    {
        if (interactor?.GetComponent<NugManager>().Nugs >= i_inputAmount)
        {
            NugManager nugMan = interactor.GetComponent<NugManager>();
            nugMan.GetComponent<NugManager>().CollectNugs(-i_inputAmount, false);
            i_storedAmount += i_inputAmount;
            SaveEvent _se = new SaveEvent(new PlayerSaveData(nugMan.Nugs, -1, i_storedAmount, null, null, null, null, null, null, -1));
            Notify(_se);
            transform.localScale = Vector3.one + Vector3.one * (i_storedAmount * 0.00001f);
            tmp_currentMoneyText.text = "£" + i_storedAmount;

            if (i_storedAmount >= i_targetAmount)
                EnablePortal();
        }
    }

    public void SetInputAmount(int _input)
    {
        i_inputAmount = _input;
    }

    private void EnablePortal()
    {
        go_portal.SetActive(true);
        go_portal.transform.parent = null;

    }
}
