using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Surface
{
    ship, planet
}

public class FootstepAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip ac_footstepShip;
    [SerializeField] private AudioClip ac_footstepPlanet;
    [SerializeField] private AudioSource as_feetAudio;
    [SerializeField] private float f_velocityToVolumeMultiplier = 0.035f;

    private Rigidbody rb_playerRB;

    private Surface e_surface;
    private void Start()
    {
        rb_playerRB = GetComponentInParent<Rigidbody>();
    }

    public void ChangeSurfaceEnum(Surface _surface)
    {
        e_surface = _surface;
    }

    public void PlayFootstepSound()
    {
        switch (e_surface)
        {
            case Surface.ship:                
                as_feetAudio.PlayOneShot(ac_footstepShip, (float)rb_playerRB.velocity.magnitude * f_velocityToVolumeMultiplier);
                break;
            case Surface.planet:                
                as_feetAudio.PlayOneShot(ac_footstepPlanet, (float)rb_playerRB.velocity.magnitude * f_velocityToVolumeMultiplier);
                break;
            default:
                break;
        }
    }
}
