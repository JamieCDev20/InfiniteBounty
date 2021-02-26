﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DynamicAudioManager : MonoBehaviour
{

    public static DynamicAudioManager x;

    [SerializeField] private AudioClip mainIntro;
    [SerializeField] private AudioClip mainLoop;
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

        mainSource.Play();
    }

    public void StartCombat()
    {
        inCombat = true;
    }

    public void EndCombat()
    {
        inCombat = false;
    }

    public void StartBoss()
    {
        isBoss = true;
    }

    public void EndBoss()
    {
        isBoss = false;
    }

    private void Update()
    {

        if (!mainSource.isPlaying)
            StartLoops();

        if (!bossSource.isPlaying && isBoss)
        {
            bossSource.clip = bossLoop;
            bossSource.Play();
        }

        combatMixer.GetFloat("Volume", out cVol);
        combatMixer.SetFloat("Volume", Mathf.Lerp(cVol, inCombat ? 0 : -80, 0.3f));

        bossMixer.SetFloat("Volume", isBoss ? 0 : -80);

    }

    private void StartLoops()
    {
        mainSource.clip = mainLoop;
        combatSource.clip = combatLoop;

        mainSource.Play();
        combatSource.Play();

    }

}
