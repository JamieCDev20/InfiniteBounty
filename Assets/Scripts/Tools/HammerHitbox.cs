using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerHitbox : MonoBehaviour
{
    private Collider c_hitbox;
    private float f_knockBack;
    private int i_damage;
    private int i_lodeDamage;
    [SerializeField] private ParticleSystem p_hitParticles;
    [SerializeField] private ElementalObject elements;
    private Vector3 v_forward;

    private void Start()
    {
        c_hitbox = GetComponent<Collider>();
        c_hitbox.enabled = false;
    }

    internal void Setup(int _i_damage, float _f_knockback, int _i_lode, Vector3 _v_forward, Element[] _elems)
    {
        i_damage = _i_damage;
        f_knockBack = _f_knockback;
        i_lodeDamage = _i_lode;
        v_forward = _v_forward;
        if(!Utils.ArrayIsNullOrZero(_elems))
            elements.Init(_elems);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            p_hitParticles.Play();
            other.GetComponent<IHitable>().TakeDamage(i_damage, true);
        }
        else if (other.GetComponent<LodeBase>() || other.GetComponent<GenericHittable>())
        {
            other.GetComponent<IHitable>().TakeDamage(i_lodeDamage, true);
            p_hitParticles.Play();
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMover>().HitKnockback(v_forward, f_knockBack);
            p_hitParticles.Play();
            return;
        }
        else if (other.attachedRigidbody != null)
        {
            other.attachedRigidbody?.AddForce(v_forward * f_knockBack, ForceMode.Impulse);
            p_hitParticles.Play();
        }
        IElementable ie = other.gameObject.GetComponent<IElementable>();
        if (ie != null)
        {
            ie.RecieveElements(elements.GetActiveElements());
        }
    }

    internal void SetHitBoxActive(bool _b_active)
    {
        c_hitbox.enabled = _b_active;
    }

}
