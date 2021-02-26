using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Surface
{
    ship, planet
}

public class FootstepAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource as_feetAudio;
    [SerializeField] private AudioClip ac_footstepShipL;
    [SerializeField] private AudioClip ac_footstepShipR;
    [SerializeField] private AudioClip ac_footstepPlanetR;
    [SerializeField] private AudioClip ac_footstepPlanetL;
    [SerializeField] private AudioClip ac_planetLand;
    [SerializeField] private AudioClip ac_shipLand;
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

    public void PlayLandingSound()
    {
        switch (e_surface)
        {
            case Surface.ship:
                as_feetAudio.pitch = Random.Range(0.95f, 1.05f);
                as_feetAudio.PlayOneShot(ac_shipLand, (float)rb_playerRB.velocity.magnitude * f_velocityToVolumeMultiplier);
                break;
            case Surface.planet:
                as_feetAudio.pitch = Random.Range(0.95f, 1.05f);
                as_feetAudio.PlayOneShot(ac_planetLand, (float)rb_playerRB.velocity.magnitude * f_velocityToVolumeMultiplier);
                break;
            default:
                break;
        }
    }

    public void PlayFootstepSoundL()
    {
        switch (e_surface)
        {
            case Surface.ship:
                as_feetAudio.pitch = Random.Range(0.95f, 1.05f);
                as_feetAudio.PlayOneShot(ac_footstepShipL, (float)rb_playerRB.velocity.magnitude * f_velocityToVolumeMultiplier);
                break;
            case Surface.planet:
                as_feetAudio.pitch = Random.Range(0.95f, 1.05f);
                as_feetAudio.PlayOneShot(ac_footstepPlanetL, (float)rb_playerRB.velocity.magnitude * f_velocityToVolumeMultiplier);
                break;
            default:
                break;
        }
    }
    
    public void PlayFootstepSoundR()
    {
        switch (e_surface)
        {
            case Surface.ship:
                as_feetAudio.pitch = Random.Range(0.95f, 1.05f);
                as_feetAudio.PlayOneShot(ac_footstepShipR, (float)rb_playerRB.velocity.magnitude * f_velocityToVolumeMultiplier);
                break;
            case Surface.planet:
                as_feetAudio.pitch = Random.Range(0.95f, 1.05f);
                as_feetAudio.PlayOneShot(ac_footstepPlanetR, (float)rb_playerRB.velocity.magnitude * f_velocityToVolumeMultiplier);
                break;
            default:
                break;
        }
    }

    public void ChangeFootstepSound(Surface surface)
    {
        e_surface = surface;
    }
}
