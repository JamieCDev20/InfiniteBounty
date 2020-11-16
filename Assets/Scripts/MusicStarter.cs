using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStarter : MonoBehaviour
{
    [SerializeField] private AudioClip ac_music;

    void Start()
    {
        GetComponent<AudioSource>().PlayOneShot(ac_music);
    }
}