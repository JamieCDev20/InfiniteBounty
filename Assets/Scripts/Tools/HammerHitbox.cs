using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerHitbox : MonoBehaviour
{
    private bool b_active;
    private float f_knockBack;
    private int i_damage;
    private int i_lodeDamage;
    [SerializeField] private ParticleSystem p_hitParticles;
    private Vector3 v_forward;

    internal void Setup(int _i_damage, float _f_knockback, int _i_lode, Vector3 _v_forward)
    {
        i_damage = _i_damage;
        f_knockBack = _f_knockback;
        i_lodeDamage = _i_lode;
        v_forward = _v_forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        print(b_active + "/" + other.name);
        if (b_active)
        {
            p_hitParticles.Play();

            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<IHitable>().TakeDamage(i_damage, true);
            }
            else if (other.GetComponent<LodeBase>())
            {
                other.GetComponent<IHitable>().TakeDamage(i_lodeDamage, true);
            }
            else if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerMover>().HitKnockback(v_forward, f_knockBack);
                return;
            }

            other.attachedRigidbody.AddForce(v_forward * f_knockBack, ForceMode.Impulse);
            //else if (other.CompareTag("Nugget"))
            //    other.GetComponent<IHitable>().TakeDamage(i_lodeDamage, true);

        }
    }

    internal void SetHitBoxActive(bool _b_active)
    {
        print("Setting hitbox to " + _b_active);
        b_active = _b_active;
    }

}
