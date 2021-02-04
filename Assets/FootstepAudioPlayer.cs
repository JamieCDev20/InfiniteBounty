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

    private Surface e_surface;

    public void ChangeSurfaceEnum(Surface _surface)
    {
        e_surface = _surface;
    }

    public void PlayFootstepSound()
    {
        switch (e_surface)
        {
            case Surface.ship:
                print("played ship step sound");
                as_feetAudio.PlayOneShot(ac_footstepShip);
                break;
            case Surface.planet:
                print("played planet step sound");
                as_feetAudio.PlayOneShot(ac_footstepPlanet);
                break;
            default:
                break;
        }
    }
}
