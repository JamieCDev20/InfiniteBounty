using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletTech : MonoBehaviour
{

    [SerializeField] private AudioClip[] acA_toiletNoises = new AudioClip[0];
    private AudioSource as_source;
    [SerializeField] private Vector2 v_toiletNoiseGap;
    private float f_time;
    private float f_timeToHit;
    [SerializeField] private ParticleSystem p_stinkCloud;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        f_timeToHit = Random.Range(v_toiletNoiseGap.x, v_toiletNoiseGap.y);
    }

    private void Update()
    {
        f_time += Time.deltaTime;

        if (f_time >= f_timeToHit)
        {
            f_timeToHit = Random.Range(v_toiletNoiseGap.x, v_toiletNoiseGap.y);
            f_time = 0;
            as_source.PlayOneShot(acA_toiletNoises[Random.Range(0, acA_toiletNoises.Length)]);
            p_stinkCloud.Play();
        }
    }


}
