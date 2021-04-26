using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager x;

    [SerializeField] private bool b_shouldTutorialAlways;
    [Space]
    [SerializeField] private GameObject go_confetti;
    private float f_inputTime;
    [SerializeField] private float f_inputTimeToLookFor = 1;

    private PlayerInputManager pim_player;
    private AudioSource as_source;
    [SerializeField] private float f_videoLength;

    [Header("UI References")]
    [SerializeField] private GameObject go_tutorialCanvas;
    [SerializeField] private Text t_promptText;
    [SerializeField] private GameObject go_textObject;
    [SerializeField] private GameObject go_videoObject;

    [Header("Pre-level")]
    [SerializeField] private TutorialChunk[] tcA_tutorial = new TutorialChunk[0];
    private bool b_reflectronUsed;
    private bool b_zippyInvested;
    private bool b_hasTools;
    private bool b_hasBackPack;
    private bool b_lodeDestroyed;
    private bool b_enemyDestroyed;
    private bool b_hasChangedRisk;
    private bool b_hasChangedShift;
    private bool b_isPlayingVideo;
    private bool b_hasFinished;
    [SerializeField] private GameObject go_tutorialWalls;

    private void Awake()
    {
        x = this;

        go_tutorialCanvas.SetActive(true);
        StartCoroutine(DoTutorialSection(tcA_tutorial));

        as_source = GetComponent<AudioSource>();
    }

    public void Init() { }

    private void Update()
    {
        if (b_isPlayingVideo)
            if (Input.anyKeyDown)
            {
                go_videoObject.SetActive(false);
                b_isPlayingVideo = false;
            }

        //if(Input.GetKeyDown(KeyCode.Escape))

    }

    internal void SetShouldTutorial(bool isOn)
    {
        b_shouldTutorialAlways = isOn;
    }

    #region Interact Checks

    public void InteractedWithReflectron()
    {
        b_reflectronUsed = true;
    }

    internal void ThingDestroyed(bool _b_isEnemy)
    {
        if (_b_isEnemy)
            b_enemyDestroyed = true;
        else
            b_lodeDestroyed = true;
    }

    public void InteractedWithZippyBack()
    {
        b_zippyInvested = true;
    }

    public void PickedUpBothTools()
    {
        b_hasTools = true;
    }

    public void PickedUpBackPack()
    {
        b_hasBackPack = true;
    }

    public void UsedShiftChanger()
    {
        b_hasChangedShift = true;
    }

    public void UsedRiskSelector()
    {
        b_hasChangedRisk = true;
    }

    #endregion

    private IEnumerator DoTutorialSection(TutorialChunk[] _tcA_chunksToWorkThrough)
    {
        while (pim_player == null)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
                pim_player = FindObjectOfType<PlayerInputManager>();
            yield return new WaitForEndOfFrame();
        }
        go_tutorialWalls.SetActive(false);

        if (SaveManager.x.SaveData.Equals(null) || b_shouldTutorialAlways)
        {
            go_tutorialWalls.transform.position = go_tutorialWalls.transform.localPosition;
            go_tutorialWalls.SetActive(true);

            b_isPlayingVideo = true;
            go_videoObject.SetActive(true);

            for (int i = 0; i < f_videoLength; i++)
                if (b_isPlayingVideo)
                    yield return new WaitForSeconds(1);

            go_videoObject.SetActive(false);
            b_isPlayingVideo = false;

            for (int i = 0; i < _tcA_chunksToWorkThrough.Length; i++)
            {
                for (int x = 0; x < _tcA_chunksToWorkThrough[i].tsdA_stepsInChunk.Length; x++)
                {
                    f_inputTime = 0;

                    switch (_tcA_chunksToWorkThrough[i].tsdA_stepsInChunk[x].tst_stepType)
                    {
                        case TutorialStepType.PlayVoiceline:
                            as_source.clip = _tcA_chunksToWorkThrough[i].tsdA_stepsInChunk[x].ac_voiceLine;
                            as_source.Play();

                            yield return new WaitForSeconds(_tcA_chunksToWorkThrough[i].tsdA_stepsInChunk[x].ac_voiceLine.length);
                            break;

                        case TutorialStepType.WaitForInput:
                            if (_tcA_chunksToWorkThrough[i].tsdA_stepsInChunk[x].b_hold)
                                while (f_inputTime < f_inputTimeToLookFor)
                                {
                                    switch (_tcA_chunksToWorkThrough[i].tsdA_stepsInChunk[x].it_inputToWaitFor)
                                    {
                                        case InputType.WASD:
                                            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0)
                                                f_inputTime += Time.deltaTime;
                                            break;
                                        case InputType.Space:
                                            if (Input.GetButton("Jump"))
                                                f_inputTime += Time.deltaTime;
                                            break;
                                        case InputType.MouseButtons:
                                            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                                                f_inputTime += Time.deltaTime;
                                            break;
                                        case InputType.Q:
                                            if (Input.GetButton("Mobility"))
                                                f_inputTime += Time.deltaTime;
                                            break;
                                        case InputType.Shift:
                                            if (Input.GetButton("Sprint"))
                                                f_inputTime += Time.deltaTime;
                                            break;
                                        case InputType.MouseMovement:
                                            if (Mathf.Abs(Input.GetAxisRaw("Mouse X")) > 0.3f || Mathf.Abs(Input.GetAxisRaw("Mouse Y")) > 0.3f)
                                                f_inputTime += Time.deltaTime;
                                            break;
                                    }
                                    yield return new WaitForEndOfFrame();
                                }
                            else
                            {
                                bool _b_shouldContinue = true;
                                while (_b_shouldContinue)
                                {
                                    switch (_tcA_chunksToWorkThrough[i].tsdA_stepsInChunk[x].it_inputToWaitFor)
                                    {
                                        case InputType.WASD:
                                            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0)
                                                _b_shouldContinue = false;
                                            break;
                                        case InputType.Space:
                                            if (Input.GetButton("Jump"))
                                                _b_shouldContinue = false;
                                            break;
                                        case InputType.MouseButtons:
                                            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                                                _b_shouldContinue = false;
                                            break;
                                        case InputType.Q:
                                            if (Input.GetButton("Mobility"))
                                                _b_shouldContinue = false;
                                            break;
                                        case InputType.Shift:
                                            if (Input.GetButton("Sprint"))
                                                _b_shouldContinue = false;
                                            break;
                                        case InputType.MouseMovement:
                                            if (Mathf.Abs(Input.GetAxisRaw("Mouse X")) > 0.3f || Mathf.Abs(Input.GetAxisRaw("Mouse Y")) > 0.3f)
                                                _b_shouldContinue = false;
                                            break;
                                    }
                                    yield return new WaitForEndOfFrame();
                                }
                            }
                            break;

                        case TutorialStepType.ConfettiShower:
                            go_confetti.transform.position = pim_player.transform.position;
                            go_confetti.SetActive(false);
                            go_confetti.SetActive(true);
                            break;

                        case TutorialStepType.SetTextPrompt:
                            t_promptText.text = _tcA_chunksToWorkThrough[i].tsdA_stepsInChunk[x].s_textMessage;

                            if (t_promptText.text == "")
                                go_textObject.SetActive(false);
                            else
                                go_textObject.SetActive(true);
                            break;

                        case TutorialStepType.WaitForReflectron:
                            b_reflectronUsed = false;
                            while (!b_reflectronUsed)
                                yield return new WaitForEndOfFrame();
                            break;

                        case TutorialStepType.WaitForZippy:
                            while (!b_zippyInvested)
                                yield return new WaitForEndOfFrame();
                            break;

                        case TutorialStepType.WaitForTools:
                            while (!b_hasTools)
                                yield return new WaitForEndOfFrame();
                            break;

                        case TutorialStepType.WaitForBackPack:
                            while (!b_hasBackPack)
                                yield return new WaitForEndOfFrame();
                            break;

                        case TutorialStepType.WaitForProximity:
                            while (Vector3.Distance(pim_player.transform.position, _tcA_chunksToWorkThrough[i].tsdA_stepsInChunk[x].go_distanceChecker.transform.position) > 10)
                                yield return new WaitForEndOfFrame();
                            break;

                        case TutorialStepType.WaitForLodeDestroyed:
                            while (!b_lodeDestroyed)
                                yield return new WaitForEndOfFrame();
                            break;

                        case TutorialStepType.WaitForEnemyDestroyed:
                            while (!b_enemyDestroyed)
                                yield return new WaitForEndOfFrame();
                            break;

                        case TutorialStepType.WaitForGameMode:
                            b_hasChangedShift = false;
                            while (!b_hasChangedShift)
                                yield return new WaitForEndOfFrame();
                            break;

                        case TutorialStepType.WaitForRiskLevel:
                            b_hasChangedRisk = false;
                            while (!b_hasChangedRisk)
                                yield return new WaitForEndOfFrame();
                            break;

                        case TutorialStepType.DisableShield:
                            go_tutorialWalls.SetActive(false);
                            break;
                    }

                    yield return new WaitForSeconds(_tcA_chunksToWorkThrough[i].tsdA_stepsInChunk[x].f_timeToWaitBeforeNextStep);
                }
            }

            //go_tutorialCanvas.SetActive(false);
        }
    }

    [System.Serializable]
    private struct TutorialChunk
    {
        public string s_name;
        public TutorialStepData[] tsdA_stepsInChunk;
    }

    [System.Serializable]
    private struct TutorialStepData
    {
        public string s_name;
        public TutorialStepType tst_stepType;
        public float f_timeToWaitBeforeNextStep;

        [Header("Play Voiceline")]
        public AudioClip ac_voiceLine;

        [Header("Wait For Input")]
        [Tooltip("True = hold for 1 second, false = instant press")] public bool b_hold;
        public InputType it_inputToWaitFor;

        [Header("Text Prompt")]
        public string s_textMessage;

        [Header("Proximity")]
        public GameObject go_distanceChecker;
    }

    private enum TutorialStepType
    {
        PlayVoiceline,
        ConfettiShower,
        SetTextPrompt,
        WaitForInput,
        WaitForReflectron,
        WaitForZippy,
        WaitForTools,
        WaitForBackPack,
        WaitForProximity,
        WaitForLodeDestroyed,
        WaitForEnemyDestroyed,
        WaitForGameMode,
        WaitForRiskLevel,
        DisableShield
    }

    private enum InputType
    {
        WASD, Space, MouseButtons, Q, Shift, MouseMovement
    }

}
