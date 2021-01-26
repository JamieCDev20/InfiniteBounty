using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelect : MonoBehaviour, IInteractible
{

    private int i_currentIndex;
    [SerializeField] private LoadIntoLevel lil_teleportButton;
    [SerializeField] private GameObject go_highLight;
    [Space, SerializeField] private Vector3[] vA_highlightPositions = new Vector3[3];
    [Space, SerializeField] private string[] sA_sceneNames = new string[3];

    public void Interacted()
    {
        i_currentIndex++;

        if (i_currentIndex >= vA_highlightPositions.Length)
            i_currentIndex = 0;

        go_highLight.transform.localPosition = vA_highlightPositions[i_currentIndex];
        lil_teleportButton.levelToLoad = sA_sceneNames[i_currentIndex];
    }


    public void Interacted(Transform interactor) { }

}
