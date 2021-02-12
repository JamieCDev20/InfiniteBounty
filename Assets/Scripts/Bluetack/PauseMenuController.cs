using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private Canvas c_playCanvas;
    [SerializeField] private Canvas c_pauseCanvas;
    [SerializeField] private Canvas c_optionsMenu;
    [SerializeField] private Canvas c_spectatingCanvas;
    private bool b_isPaused;
    private PlayerInputManager pim;
    private CameraController cc_cam;
    private Rigidbody rb_playerPhysics;
    private bool b_isSpectating;

    [Header("Option References")]
    [SerializeField] private Slider s_sensitivitySlider;
    [SerializeField] private VolumeSliders vs_volSliders;

    [Header("Mixers")]
    [SerializeField] private AudioMixer am_ambienceMixer;
    [SerializeField] private AudioMixer am_musicMixer;
    [SerializeField] private AudioMixer am_sfxMixer;

    private void Start()
    {
        c_optionsMenu.enabled = false;
        c_pauseCanvas.enabled = false;
        c_playCanvas.enabled = true;

        cc_cam = GetComponentInParent<CameraController>();
        SetSensitivty();
        SetAmbienceVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

    #region Nick Stuff

    private void Update()
    {
        if (b_isPaused)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Resume();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
        if (pim.GetIsPaused())
        {
            if (!b_isPaused)
                Pause();
            else
                Resume();
        }
    }

    public void Resume()
    {
        b_isPaused = false;
        if (b_isSpectating)
        {
            c_spectatingCanvas.enabled = true;
        }
        else
        {
            c_playCanvas.enabled = true;
        }

        c_pauseCanvas.enabled = false;
        c_optionsMenu.enabled = false;

        pim.b_shouldPassInputs = true;
        cc_cam.enabled = true;
        pim.GetComponent<PlayerAnimator>().SetShootability(true);
        rb_playerPhysics.isKinematic = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        b_isPaused = true;
        if (b_isSpectating)
            c_spectatingCanvas.enabled = false;
        else
            c_playCanvas.enabled = false;

        c_pauseCanvas.enabled = true;
        pim.b_shouldPassInputs = false;
        cc_cam.enabled = false;
        pim.GetComponent<PlayerAnimator>().SetShootability(false);
        rb_playerPhysics.isKinematic = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Quit()
    {
        print("Quitting by pasue menu");
        UniversalOverlord.x.ReturnToMainMenu();
    }

    public void Suicude()
    {
        NetworkedPlayer.x.Suicide();
    }

    public void Options()
    {
        //Debug.LogError("It should've optioned, maybe it did, I dunno;");
        c_pauseCanvas.enabled = false;
        c_optionsMenu.enabled = true;
    }

    public void ReturnFromOptions()
    {
        //Debug.LogError("Are we back on the main pause-menu? Aight, sick.");
        c_pauseCanvas.enabled = true;
        c_optionsMenu.enabled = false;
    }

    public void DoOptionThings()
    {
        print("OPTIONED");
    }

    internal void SetPIM(PlayerInputManager _newPIM)
    {
        pim = _newPIM;
        rb_playerPhysics = pim.GetComponent<Rigidbody>();
    }

    internal void SetSpectating()
    {
        b_isSpectating = true;
        c_playCanvas.enabled = false;
        c_spectatingCanvas.enabled = true;
    }

    internal void StopSpectating()
    {
        b_isSpectating = false;
        c_playCanvas.enabled = true;
        c_spectatingCanvas.enabled = false;
    }

    #endregion

    #region Options

    public void SetSensitivty()
    {
        if (cc_cam && s_sensitivitySlider)
            cc_cam.v2_cameraSensitivity.x = s_sensitivitySlider.value;
    }

    public void SetAmbienceVolume()
    {
        am_ambienceMixer.SetFloat("Volume", vs_volSliders.s_ambienceVolumeSlider.value);
    }

    public void SetMusicVolume()
    {
        am_musicMixer.SetFloat("Volume", vs_volSliders.s_musicVolumeSilder.value);
    }

    public void SetSFXVolume()
    {
        am_sfxMixer.SetFloat("Volume", vs_volSliders.s_sfxVolumeSilder.value);
    }


    #endregion

    [System.Serializable]
    private struct VolumeSliders
    {
        public Slider s_ambienceVolumeSlider;
        public Slider s_musicVolumeSilder;
        public Slider s_sfxVolumeSilder;
    }

}