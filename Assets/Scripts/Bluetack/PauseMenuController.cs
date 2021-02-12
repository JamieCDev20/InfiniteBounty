using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : SubjectBase
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

    private float[] A_options = new float[5];

    [Header("Option References")]
    [SerializeField] private Slider s_sensitivitySliderX;
    [SerializeField] private Slider s_sensitivitySliderY;
    [SerializeField] private Toggle b_mouseInverted;
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
        SetXSensitivty();
        SetYSensitivity();
        SetAmbienceVolume();
        SetMusicVolume();
        SetSFXVolume();
        SaveManager sm = FindObjectOfType<SaveManager>();
        if(sm.SaveData.playerOptions != null)
            if(sm.SaveData.playerOptions.A_settingFloats.Length > 0)
                InitOptions(sm.SaveData.playerOptions);
        AddObserver(sm);
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
        SettingsValues sv = new SettingsValues();
        sv.invertY = b_mouseInverted;
        sv.A_settingFloats = A_options;
        PlayerSaveData pd = new PlayerSaveData(0, 0, null, null, sv);
        if(sv != null)
        {
            SaveEvent se = new SaveEvent(pd);
            Notify(se);
        }
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

    #region John Stuff
    public void SetQuality(int _i_qualityIndex)
    {
        QualitySettings.SetQualityLevel(_i_qualityIndex);
    }
    #endregion

    #region Options

    public void InitOptions(SettingsValues _options)
    {
        Debug.Log("Options Getting set");
        if(_options.A_settingFloats[(int)OptionNames.sensitivityX] >= s_sensitivitySliderX.minValue && _options.A_settingFloats[(int)OptionNames.sensitivityX] <= s_sensitivitySliderX.maxValue)
        {
            cc_cam.v2_cameraSensitivity.x = _options.A_settingFloats[(int)OptionNames.sensitivityX];
            s_sensitivitySliderX.value = _options.A_settingFloats[(int)OptionNames.sensitivityX];
        }
        if(_options.A_settingFloats[(int)OptionNames.sensitivityY] >= s_sensitivitySliderX.minValue && _options.A_settingFloats[(int)OptionNames.sensitivityY] <= s_sensitivitySliderX.maxValue)
        {
            cc_cam.v2_cameraSensitivity.x = _options.A_settingFloats[(int)OptionNames.sensitivityX];
            s_sensitivitySliderX.value = _options.A_settingFloats[(int)OptionNames.sensitivityX];
        }
        if(_options.A_settingFloats[(int)OptionNames.ambience] >= vs_volSliders.s_ambienceVolumeSlider.minValue && _options.A_settingFloats[(int)OptionNames.ambience] <= vs_volSliders.s_ambienceVolumeSlider.maxValue)
        {
            am_ambienceMixer.SetFloat("Volume", _options.A_settingFloats[(int)OptionNames.ambience]);
            vs_volSliders.s_ambienceVolumeSlider.value = _options.A_settingFloats[(int)OptionNames.ambience];
        }
        if(_options.A_settingFloats[(int)OptionNames.music] >= vs_volSliders.s_musicVolumeSilder.minValue && _options.A_settingFloats[(int)OptionNames.music] <= vs_volSliders.s_musicVolumeSilder.maxValue)
        {
            am_musicMixer.SetFloat("Volume", _options.A_settingFloats[(int)OptionNames.music]);
            vs_volSliders.s_musicVolumeSilder.value = _options.A_settingFloats[(int)OptionNames.music];
        }
        if(_options.A_settingFloats[(int)OptionNames.sfx] >= vs_volSliders.s_sfxVolumeSilder.minValue && _options.A_settingFloats[(int)OptionNames.sfx] <= vs_volSliders.s_sfxVolumeSilder.maxValue)
        {
            am_sfxMixer.SetFloat("Volume", (int)OptionNames.sfx);
            vs_volSliders.s_sfxVolumeSilder.value = _options.A_settingFloats[(int)OptionNames.sfx];
        }
    }

    public void SetXSensitivty()
    {
        if (cc_cam && s_sensitivitySliderX)
        {
            A_options[(int)OptionNames.sensitivityX] = s_sensitivitySliderX.value;
            cc_cam.v2_cameraSensitivity.x = s_sensitivitySliderX.value;
        }
    }

    public void SetYSensitivity()
    {
        if(cc_cam && s_sensitivitySliderY)
        {
            A_options[(int)OptionNames.sensitivityY] = s_sensitivitySliderY.value;
            cc_cam.v2_cameraSensitivity.y = s_sensitivitySliderY.value;
        }
    }

    public void SetAmbienceVolume()
    {
        A_options[(int)OptionNames.ambience] = vs_volSliders.s_ambienceVolumeSlider.value;
        am_ambienceMixer.SetFloat("Volume", vs_volSliders.s_ambienceVolumeSlider.value);
    }

    public void SetMusicVolume()
    {
        A_options[(int)OptionNames.music] = vs_volSliders.s_musicVolumeSilder.value;
        am_musicMixer.SetFloat("Volume", vs_volSliders.s_musicVolumeSilder.value);
    }

    public void SetSFXVolume()
    {
        A_options[(int)OptionNames.sfx] = vs_volSliders.s_sfxVolumeSilder.value;
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

public enum OptionNames
{
    sensitivityX,
    sensitivityY,
    ambience,
    music,
    sfx
}