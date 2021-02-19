using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiggyBank : MonoBehaviour, IInteractible
{
    [SerializeField] private int i_targetAmount;
    [SerializeField] private int i_inputAmount;
    [SerializeField] private int i_storedAmount;
    [SerializeField] GameObject go_portal;
    private DifficultyManager dm_man;

    public void Init(DifficultyManager _diffMan)
    {
        dm_man = _diffMan;
        if (dm_man.MaximumDifficulty <= 10)
        {
            gameObject.SetActive(false);
        }
    }

    public void Interacted()
    {
        
    }

    public void Interacted(Transform interactor)
    {
        if(interactor?.GetComponent<NugManager>().Nugs >= i_inputAmount)
        {
            interactor.GetComponent<NugManager>().CollectNugs(-i_inputAmount, false);
            i_storedAmount += i_inputAmount;
            if(i_storedAmount >= i_targetAmount)
            {
                EnablePortal();
            }
        }
    }

    public void SetInputAmount(int _input)
    {
        i_inputAmount = _input;
    }

    private void EnablePortal()
    {
        go_portal.SetActive(true);
        gameObject.SetActive(false);
    }
}
