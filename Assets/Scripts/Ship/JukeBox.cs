﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeBox : MonoBehaviour
{

    private AudioSource as_source;
    [SerializeField] private AudioClip[] acA_songs = new AudioClip[0];
    private int i_currentSong;
    private bool b_isPoweredOn;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        b_isPoweredOn = true;
        as_source.clip = acA_songs[i_currentSong];
        as_source.Play();
    }

    private void Update()
    {
        if (b_isPoweredOn)
            if (!as_source.isPlaying)
                SkipSong();
    }

    internal void SkipSong()
    {
        i_currentSong++;
        if (i_currentSong >= acA_songs.Length)
            i_currentSong = 0;

        as_source.clip = acA_songs[i_currentSong];
        as_source.Play();
        b_isPoweredOn = true;
    }

    internal void TogglePower()
    {
        if (b_isPoweredOn)
        {
            b_isPoweredOn = false;
            as_source.Pause();
        }
        else
        {
            b_isPoweredOn = true;
            as_source.Play();
        }
    }

}
