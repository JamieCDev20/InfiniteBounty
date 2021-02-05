using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundOnEnter : MonoBehaviour
{
    [SerializeField] private AudioSource as_source;
    [SerializeField] private AudioClip ac_clip;

    private void OnTriggerEnter(Collider other)
    {
        as_source.pitch = 1;
        as_source.PlayOneShot(ac_clip);
    }
}
