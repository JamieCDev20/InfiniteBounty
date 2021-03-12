using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DynamicAudioManager : MonoBehaviour
{

    public static DynamicAudioManager x;

    [SerializeField] private AudioClip mainIntro;
    [SerializeField] private AudioClip mainLoop;
    [SerializeField] private AudioClip combatIntro;
    [SerializeField] private AudioClip combatLoop;
    [SerializeField] private AudioClip bossIntro;
    [SerializeField] private AudioClip bossLoop;

    [Space]

    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource combatSource;
    [SerializeField] private AudioSource bossSource;

    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixer combatMixer;
    [SerializeField] private AudioMixer bossMixer;

    [SerializeField] private float combatLerp = 0.3f;
    [SerializeField] private float mainLerp = 0.3f;

    private bool inCombat;
    private bool isBoss;

    private float cVol;

    private void Awake()
    {
        x = this;
    }

    private void Start()
    {
        mainSource.clip = mainIntro;
        combatSource.clip = combatLoop;
        bossSource.clip = bossIntro;

        mainMixer.SetFloat("Volume", 0);
        mainSource.Play();
    }

    public void StartCombat()
    {
        inCombat = true;
        combatSource.PlayOneShot(combatIntro);
        combatMixer.SetFloat("Volume", 0);
    }

    public void EndCombat()
    {
        inCombat = false;
    }

    public void StartBoss()
    {
        isBoss = true;
        bossSource.PlayOneShot(bossIntro);
        bossMixer.SetFloat("Volume", 0);
    }

    public void EndBoss()
    {
        isBoss = false;
    }

    private void Update()
    {

        float cMain;
        mainMixer.GetFloat("Volume", out cMain);

        float cCombat;
        combatMixer.GetFloat("Volume", out cCombat);

        combatMixer.SetFloat("Volume", Mathf.Lerp(cCombat, inCombat ? 0 : -80, combatLerp));
        mainMixer.SetFloat("Volume", Mathf.Lerp(cMain, inCombat ? -80 : 0, mainLerp));

        bossMixer.SetFloat("Volume", isBoss ? 0 : -80);

        if (!combatSource.isPlaying)
        {
            combatSource.clip = combatLoop;
            combatSource.Play();
        }

        if (!mainSource.isPlaying)
        {
            mainSource.clip = mainLoop;
            mainSource.Play();
        }

        if (isBoss)
            if (!bossSource.isPlaying)
            {
                bossSource.clip = bossLoop;
                bossSource.Play();
            }

    }

    private void StartLoops()
    {
        mainSource.clip = mainLoop;
        combatSource.clip = combatLoop;

        mainSource.Play();
        combatSource.Play();

    }

}
