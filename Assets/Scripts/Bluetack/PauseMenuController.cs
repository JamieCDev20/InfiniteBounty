using Photon.Pun;
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
    [SerializeField] private Canvas c_mainMenu;
    [SerializeField] private Canvas c_settingsMenu;
    [SerializeField] private Canvas c_controlsMenu;
    [SerializeField] private Canvas c_displayMenu;
    [SerializeField] private Canvas c_audioMenu;
    [SerializeField] private Canvas c_spectatingCanvas;
    private bool b_isPaused;
    private PlayerInputManager pim;
    private CameraController cc_cam;
    private Rigidbody rb_playerPhysics;
    private bool b_isSpectating;
    private int[] A_display = new int[2];
    private float[] A_options = new float[6];

    [Header("Option References")]
    [SerializeField] private Slider s_sensitivitySliderX;
    [SerializeField] private Slider s_sensitivitySliderY;
    [SerializeField] private float f_cameraSpeedMult;
    [Space, SerializeField] private Toggle b_mouseInverted;
    [SerializeField] private VolumeSliders vs_volSliders;

    [Header("Mixers")]
    [SerializeField] private AudioMixer am_masterMixer;
    [SerializeField] private AudioMixer am_ambienceMixer;
    [SerializeField] private AudioMixer am_musicMixer;
    [SerializeField] private AudioMixer am_sfxMixer;
    [Space, SerializeField] private AudioClip[] acA_musicTestClips = new AudioClip[0];
    [SerializeField] private AudioClip[] acA_ambienceTestClips = new AudioClip[0];
    [SerializeField] private AudioClip[] acA_soundEffectsTestClips = new AudioClip[0];


    private void Start()
    {
        c_settingsMenu.enabled = false;
        c_pauseCanvas.enabled = false;
        c_playCanvas.enabled = true;

        cc_cam = GetComponentInParent<CameraController>();
        SetAmbienceVolume();
        SetMusicVolume();
        SetSFXVolume();
        SaveManager sm = FindObjectOfType<SaveManager>();
        if (sm != null)
            if (sm.SaveData.A_playerSliderOptions != null)
                if (sm.SaveData.A_playerSliderOptions.Length > 0)
                    InitOptions(sm.SaveData.b_inverted, sm.SaveData.A_playerSliderOptions);
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
        c_settingsMenu.enabled = false;
        c_mainMenu.enabled = true;
        c_controlsMenu.enabled = false;
        c_displayMenu.enabled = false;
        c_audioMenu.enabled = false;

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
        c_pauseCanvas.enabled = false;
        c_settingsMenu.enabled = true;
    }

    public void ReturnFromOptions()
    {
        c_pauseCanvas.enabled = true;
        SaveSettings();
        c_settingsMenu.enabled = false;
    }

    public void SaveSettings()
    {
        SettingsValues sv = new SettingsValues();
        sv.invert = new bool[] { b_mouseInverted };
        sv.A_settingFloats = A_options;
        sv.displaySettings = A_display;
        PlayerSaveData pd = new PlayerSaveData(-1, -1, null, null, null, null, null, sv, -1);
        if (sv != null)
        {
            SaveEvent se = new SaveEvent(pd);
            Notify(se);
        }
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
        A_display[(int)DisplaySettings.quality] = _i_qualityIndex;
        QualitySettings.SetQualityLevel(_i_qualityIndex);
    }

    public void SetResolutions(int _i_resolutionIndex)
    {
        A_display[(int)DisplaySettings.resolution] = _i_resolutionIndex;
    }

    #endregion

    #region Options

    public void InitOptions(bool _b_inv, float[] _options)
    {
        if (s_sensitivitySliderX != null)
        {
            if (_options[(int)OptionNames.sensitivityX] / f_cameraSpeedMult >= s_sensitivitySliderX.minValue && _options[(int)OptionNames.sensitivityX] / f_cameraSpeedMult <= s_sensitivitySliderX.maxValue)
            {
                cc_cam.v2_cameraSensitivity.x = _options[(int)OptionNames.sensitivityX];
                s_sensitivitySliderX.value = _options[(int)OptionNames.sensitivityX] / f_cameraSpeedMult;
            }
        }
        if (s_sensitivitySliderY != null)
        {
            if (_options[(int)OptionNames.sensitivityY] / f_cameraSpeedMult >= s_sensitivitySliderY.minValue && _options[(int)OptionNames.sensitivityY] / f_cameraSpeedMult <= s_sensitivitySliderY.maxValue)
            {
                cc_cam.v2_cameraSensitivity.y = _options[(int)OptionNames.sensitivityY];
                s_sensitivitySliderY.value = _options[(int)OptionNames.sensitivityY] / f_cameraSpeedMult;
            }
        }

        if (_options[(int)OptionNames.ambience] >= -80 && _options[(int)OptionNames.ambience] <= vs_volSliders.s_ambienceVolumeSlider.maxValue)
        {
            am_ambienceMixer.SetFloat("Volume", _options[(int)OptionNames.ambience]);
            if (_options[(int)OptionNames.ambience] < vs_volSliders.s_ambienceVolumeSlider.minValue)
                vs_volSliders.s_ambienceVolumeSlider.value = vs_volSliders.s_ambienceVolumeSlider.minValue;
            else
                vs_volSliders.s_ambienceVolumeSlider.value = _options[(int)OptionNames.ambience];
        }
        if (_options[(int)OptionNames.music] >= -80 && _options[(int)OptionNames.music] <= vs_volSliders.s_musicVolumeSilder.maxValue)
        {
            am_musicMixer.SetFloat("Volume", _options[(int)OptionNames.music]);
            if (_options[(int)OptionNames.music] < vs_volSliders.s_musicVolumeSilder.minValue)
                vs_volSliders.s_musicVolumeSilder.value = vs_volSliders.s_musicVolumeSilder.minValue;
            else
                vs_volSliders.s_musicVolumeSilder.value = _options[(int)OptionNames.music];
        }
        if (_options[(int)OptionNames.sfx] >= -80 && _options[(int)OptionNames.sfx] <= vs_volSliders.s_sfxVolumeSilder.maxValue)
        {
            am_sfxMixer.SetFloat("Volume", (int)OptionNames.sfx);
            if (_options[(int)OptionNames.sfx] < vs_volSliders.s_sfxVolumeSilder.minValue)
                vs_volSliders.s_sfxVolumeSilder.value = vs_volSliders.s_sfxVolumeSilder.minValue;
            else
                vs_volSliders.s_sfxVolumeSilder.value = _options[(int)OptionNames.sfx];
        }
    }

    public void SetXSensitivty(float val)
    {
        A_options[(int)OptionNames.sensitivityX] = s_sensitivitySliderX.value * f_cameraSpeedMult;
        cc_cam.v2_cameraSensitivity.x = s_sensitivitySliderX.value * f_cameraSpeedMult;
        //if (cc_cam && s_sensitivitySliderX)
        //{
        //}
    }

    public void SetYSensitivity(float val)
    {
        A_options[(int)OptionNames.sensitivityY] = val * f_cameraSpeedMult;
        cc_cam.v2_cameraSensitivity.y = val * f_cameraSpeedMult;
        //if (cc_cam && s_sensitivitySliderY)
        //{
        //}
    }

    public void SetAmbienceVolume()
    {
        A_options[(int)OptionNames.ambience] = (vs_volSliders.s_ambienceVolumeSlider.value == vs_volSliders.s_ambienceVolumeSlider.minValue ? -80 : vs_volSliders.s_ambienceVolumeSlider.value);
        am_ambienceMixer.SetFloat("Volume", (vs_volSliders.s_ambienceVolumeSlider.value == vs_volSliders.s_ambienceVolumeSlider.minValue ? -80 : vs_volSliders.s_ambienceVolumeSlider.value));
        if (acA_ambienceTestClips.Length > 0)
            AudioSource.PlayClipAtPoint(acA_ambienceTestClips[Random.Range(0, acA_ambienceTestClips.Length)], cc_cam.transform.position);
    }

    public void SetMusicVolume()
    {
        A_options[(int)OptionNames.music] = (vs_volSliders.s_musicVolumeSilder.value == vs_volSliders.s_musicVolumeSilder.minValue ? -80 : vs_volSliders.s_musicVolumeSilder.value);
        am_musicMixer.SetFloat("Volume", (vs_volSliders.s_musicVolumeSilder.value == vs_volSliders.s_musicVolumeSilder.minValue ? -80 : vs_volSliders.s_musicVolumeSilder.value));
        if (acA_musicTestClips.Length > 0)
            AudioSource.PlayClipAtPoint(acA_musicTestClips[Random.Range(0, acA_musicTestClips.Length)], cc_cam.transform.position);
    }

    public void SetSFXVolume()
    {
        A_options[(int)OptionNames.sfx] = (vs_volSliders.s_sfxVolumeSilder.value == vs_volSliders.s_sfxVolumeSilder.minValue ? -80 : vs_volSliders.s_sfxVolumeSilder.value);
        am_sfxMixer.SetFloat("Volume", (vs_volSliders.s_sfxVolumeSilder.value == vs_volSliders.s_sfxVolumeSilder.minValue ? -80 : vs_volSliders.s_sfxVolumeSilder.value));
        if (acA_soundEffectsTestClips.Length > 0)
            AudioSource.PlayClipAtPoint(acA_soundEffectsTestClips[Random.Range(0, acA_soundEffectsTestClips.Length)], cc_cam.transform.position);
    }

    public void SetMasterVolume()
    {
        A_options[(int)OptionNames.master] = (vs_volSliders.s_masterVolumeSlider.value == vs_volSliders.s_masterVolumeSlider.minValue ? -80 : vs_volSliders.s_masterVolumeSlider.value);
        am_masterMixer.SetFloat("Volume", (vs_volSliders.s_masterVolumeSlider.value == vs_volSliders.s_masterVolumeSlider.minValue ? -80 : vs_volSliders.s_masterVolumeSlider.value));
        if (acA_soundEffectsTestClips.Length > 0)
            AudioSource.PlayClipAtPoint(acA_soundEffectsTestClips[Random.Range(0, acA_soundEffectsTestClips.Length)], cc_cam.transform.position);
    }

    #endregion

    [System.Serializable]
    private struct VolumeSliders
    {
        public Slider s_masterVolumeSlider;
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
    sfx,
    master
}

public enum DisplaySettings
{
    quality,
    resolution
}