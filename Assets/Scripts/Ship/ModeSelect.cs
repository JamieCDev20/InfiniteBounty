using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelect : MonoBehaviour, IInteractible
{

    private int i_currentIndex;
    [SerializeField] private LoadIntoLevel lil_teleportButton;
    [Space, SerializeField] private GameObject[] goA_highlightPositions = new GameObject[3];
    [Space, SerializeField] private string[] sA_sceneNames = new string[3];

    public void Interacted()
    {
        goA_highlightPositions[i_currentIndex].SetActive(false);

        i_currentIndex++;

        if (i_currentIndex >= goA_highlightPositions.Length)
            i_currentIndex = 0;

        goA_highlightPositions[i_currentIndex].SetActive(true);
        lil_teleportButton.levelToLoad = sA_sceneNames[i_currentIndex];
    }


    public void Interacted(Transform interactor) { }

}
