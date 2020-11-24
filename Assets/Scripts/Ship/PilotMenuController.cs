using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotMenuController : MonoBehaviour
{

    [SerializeField] private GameObject[] goA_pilotCanvases = new GameObject[2];


    private void Start()
    {
        for (int i = 0; i < goA_pilotCanvases.Length; i++)
        {
            goA_pilotCanvases[i].SetActive(false);
        }
    }

    internal void BeginPiloting()
    {
        print("How did you trigger this, cause I didn't?");
        for (int i = 0; i < goA_pilotCanvases.Length; i++)
        {
            goA_pilotCanvases[i].SetActive(true);
        }
    }

    #region Gamemodes

    public void SelectedNuggetRun()
    {
        print("Should've selected Nugget Run");
    }

    public void SelectedMotherlode()
    {
        print("Should've selected Motherlode");
    }

    public void SelectedUhhh()
    {
        print("Uhh...");
    }

    #endregion

    #region Planet buttons

    public void ChooseIgnova()
    {
        print("Should've selected Ignova");
    }

    #endregion

}
