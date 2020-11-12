using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private Button b_onlineButton;

    private void Start()
    {
        b_onlineButton.interactable = false;
        //Invoke("EnableButtons", 2);
    }

    internal void EnableButtons()
    {
        Debug.LogError("Did this get called when the server was reached? Probably not.");
        //b_onlineButton.interactable = true;
    }


    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
