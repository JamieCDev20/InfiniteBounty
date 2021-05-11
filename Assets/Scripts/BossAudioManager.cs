using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BossAudioManager : MonoBehaviour
{

    [SerializeField] private AudioClip bossIntro;
    [SerializeField] private AudioClip bossLoop;

    [SerializeField] private float lerpOutTime = 5;

    [SerializeField] private AudioSource bossSource;

    [SerializeField] private AudioMixer bossMixer;

    private bool playing;

    private void Start()
    {
        bossSource.clip = bossIntro;
        bossSource.Play();
    }

    private void Update()
    {
        if (!bossSource.isPlaying && playing)
        {
            bossSource.clip = bossLoop;
            bossSource.Play();
        }
    }

    public void StopPlaying()
    {
        playing = false;
        StartCoroutine(FadeBossMusic());
    }

    IEnumerator FadeBossMusic()
    {
        float t = 0;

        while (t < lerpOutTime)
        {
            bossMixer.SetFloat("Volume", Mathf.Lerp(0, -80, t / lerpOutTime));
            t += Time.deltaTime;
            yield return null;
        }

    }

}
