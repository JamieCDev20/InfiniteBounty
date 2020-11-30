using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private Canvas c_playCanvas;
    [SerializeField] private Canvas c_pauseCanvas;
    [SerializeField] private Canvas c_optionsMenu;
    private bool b_isPaused;

    private void Start()
    {
        c_optionsMenu.enabled = false;
        c_pauseCanvas.enabled = false;
        c_playCanvas.enabled = true;
    }

    private void Update()
    {
        //if (PlayerInputManager.x.GetIsPaused())
        //{
        //    if (!b_isPaused)
        //        Pause();
        //    else
        //        Resume();
        //}
    }

    public void Resume()
    {
        c_pauseCanvas.enabled = false;
        c_playCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.LogError("Should've resumed, but I don't know how");
    }

    public void Pause()
    {
        c_playCanvas.enabled = false;
        c_pauseCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.LogError("Should've paused, but I don't know how");
    }

    public void Quit()
    {
        Debug.LogError("It should've quit, I don't know how");
    }

    #region Option Menu things

    public void Options()
    {
        Debug.LogError("It should've optioned, maybe it did, I dunno;");
        c_pauseCanvas.enabled = false;
        c_optionsMenu.enabled = true;
    }

    public void ReturnFromOptions()
    {
        Debug.LogError("Are we back on the main pause-menu? Aight, sick.");
        c_pauseCanvas.enabled = true;
        c_optionsMenu.enabled = false;
    }

    public void DoOptionThings()
    {
        print("OPTIONED");
    }

    #endregion

}