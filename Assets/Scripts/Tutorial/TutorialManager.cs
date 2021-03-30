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
    [SerializeField] private GameObject go_videoObject;

    [Header("Pre-level")]
    [SerializeField] private TutorialStepData[] tsdA_preLevelSteps = new TutorialStepData[0];
    private bool b_reflectronUsed;

    private void Awake()
    {
        x = this;
        //Uncomment the line below to only start tutorial when you should
        if (SaveManager.x.SaveData.Equals(null) || b_shouldTutorialAlways)
            StartCoroutine(StartTutorial());
    }

    public IEnumerator StartTutorial()
    {
        print("Starting Tutorial 'cause you had no data");
        go_videoObject.SetActive(true);

        yield return new WaitForSeconds(f_videoLength);
        go_videoObject.SetActive(false);

        go_tutorialCanvas.SetActive(true);
        StartCoroutine(DoTutorialSection(tsdA_preLevelSteps));
    }

    public void InteractedWithReflectron()
    {
        b_reflectronUsed = true;
    }

    private IEnumerator DoTutorialSection(TutorialStepData[] _tsdA_stepToWorkThrough)
    {
        while (pim_player == null)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
                pim_player = FindObjectOfType<PlayerInputManager>();
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < _tsdA_stepToWorkThrough.Length; i++)
        {
            f_inputTime = 0;
            //print($"Doing {i}, which is a {_tsdA_stepToWorkThrough[i].tst_stepType}-type.");

            switch (_tsdA_stepToWorkThrough[i].tst_stepType)
            {
                case TutorialStepType.PlayVoiceline:
                    as_source.clip = _tsdA_stepToWorkThrough[i].ac_voiceLine;
                    as_source.Play();
                    break;

                case TutorialStepType.WaitForInput:
                    if (_tsdA_stepToWorkThrough[i].b_hold)
                        while (f_inputTime < f_inputTimeToLookFor)
                        {
                            switch (tsdA_preLevelSteps[i].it_inputToWaitFor)
                            {
                                case InputType.WASD:
                                    if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0)
                                        f_inputTime += Time.deltaTime;
                                    break;
                                case InputType.Space:
                                    if (Input.GetButton("Jump"))
                                        f_inputTime += Time.deltaTime;
                                    break;
                                case InputType.Mouse:
                                    if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                                        f_inputTime += Time.deltaTime;
                                    break;
                                case InputType.Q:
                                    if (Input.GetButton("Mobility"))
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
                            switch (tsdA_preLevelSteps[i].it_inputToWaitFor)
                            {
                                case InputType.WASD:
                                    if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0)
                                        _b_shouldContinue = false;
                                    break;
                                case InputType.Space:
                                    if (Input.GetButton("Jump"))
                                        _b_shouldContinue = false;
                                    break;
                                case InputType.Mouse:
                                    if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                                        _b_shouldContinue = false;
                                    break;
                                case InputType.Q:
                                    if (Input.GetButton("Mobility"))
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
                    t_promptText.text = _tsdA_stepToWorkThrough[i].s_textMessage;
                    break;

                case TutorialStepType.WaitForReflectron:
                    while (!b_reflectronUsed)
                        yield return new WaitForEndOfFrame();
                    break;
            }

            yield return new WaitForSeconds(_tsdA_stepToWorkThrough[i].f_timeToWaitBeforeNextStep);
        }
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
    }

    private enum TutorialStepType
    {
        PlayVoiceline, WaitForInput, ConfettiShower, SetTextPrompt,
        WaitForReflectron
    }

    private enum InputType
    {
        WASD, Space, Mouse, Q
    }

}
