using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandymanSkelemeshAnimator : MonoBehaviour
{

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip footClip;

    public void PlayFootstep()
    {
        source.PlayOneShot(footClip);
    }

}
