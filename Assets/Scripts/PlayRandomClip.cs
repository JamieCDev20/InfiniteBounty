using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomClip : MonoBehaviour
{

    [SerializeField] private AudioClip ac_clipToPlayAtStart;
    [SerializeField] private AudioClip[] acA_clipsToPlayRandomly;
    private List<AudioClip> acL_clipsUInplayed = new List<AudioClip>();
    [Space, SerializeField] private bool b_shouldOnlyPlayClipsOnce;
    private AudioSource as_source;
    [SerializeField] private Vector2 v_gapBetweenClips;
    private float f_currentTime;
    private float f_timeToWaitTo;

    void Start()
    {
        as_source = GetComponent<AudioSource>();
        acL_clipsUInplayed.AddRange(acA_clipsToPlayRandomly);

        if (ac_clipToPlayAtStart)
            as_source.PlayOneShot(ac_clipToPlayAtStart);

        f_timeToWaitTo = Random.Range(v_gapBetweenClips.x, v_gapBetweenClips.y);
    }

    void Update()
    {
        if (acL_clipsUInplayed.Count > 0)
        {
            f_currentTime += Time.deltaTime;

            if (f_currentTime >= f_timeToWaitTo)
            {
                f_timeToWaitTo = Random.Range(v_gapBetweenClips.x, v_gapBetweenClips.y);
                f_currentTime = 0;

                int _i_soundIndex = Random.Range(0, acL_clipsUInplayed.Count);
                as_source.PlayOneShot(acL_clipsUInplayed[_i_soundIndex]);

                if (b_shouldOnlyPlayClipsOnce)
                    acL_clipsUInplayed.RemoveAt(_i_soundIndex);
            }
        }
    }
}
