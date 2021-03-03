using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JukeBox : MonoBehaviour, IHitable
{

    private AudioSource as_source;
    [SerializeField] private string[] sA_trackNames = new string[0];
    [SerializeField] private AudioClip[] acA_songs = new AudioClip[0];
    [Space, SerializeField] private TextMeshPro tmp_trackNameText;
    private int i_currentSong;
    private bool b_isPoweredOn;
    [SerializeField] private ParticleSystem p_deathEffect;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        b_isPoweredOn = true;
        as_source.clip = acA_songs[i_currentSong];
        as_source.Play();
        tmp_trackNameText.text = sA_trackNames[i_currentSong];

        TogglePower();
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
        tmp_trackNameText.text = sA_trackNames[i_currentSong];
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


    public void TakeDamage(int damage, bool activatesThunder)
    {
        p_deathEffect.transform.parent = null;
        p_deathEffect.Play();
        gameObject.SetActive(false);
        b_isPoweredOn = true;
        TogglePower();
    }

    public bool IsDead() { return false; }

    public void Die() { }

    public void TakeDamage(int damage, bool activatesThunder, float delay) { }
}
