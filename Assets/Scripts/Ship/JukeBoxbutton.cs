using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeBoxbutton : MonoBehaviour, IInteractible
{
    [SerializeField] private bool b_isSkipButton;
    private JukeBox jb_box;

    private void Start()
    {
        jb_box = GetComponentInParent<JukeBox>();
    }

    public void Interacted()
    {
        if (b_isSkipButton)
            jb_box.SkipSong();
        else
            jb_box.TogglePower();
    }

    public void Interacted(Transform interactor) { }
}
