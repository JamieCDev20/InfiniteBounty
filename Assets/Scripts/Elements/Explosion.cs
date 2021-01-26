using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float f_radius = 2;
    [SerializeField] private int i_damage;
    [SerializeField] private int i_lodeDamage;
    [SerializeField] private float f_knockBack;
    [SerializeField] private float f_detonationTime;
    [SerializeField] private SphereCollider sc;
    [SerializeField] private GameObject[] go_particles;
    [SerializeField] private LayerMask lm_blastMask;

    
    internal void OnEnable()
    {
        // Check if it's a timed explosion
        if (f_detonationTime > 0)
            StartCoroutine("Explode");
        else Explode();
    }

    public void Setup(AugmentExplosion _ae, GameObject[] _go_particles)
    {
        i_damage        = _ae.i_damage;
        i_lodeDamage    = _ae.i_lodeDamage;
        f_knockBack     = _ae.f_explockBack;
        f_detonationTime= _ae.f_detonationTime;
        f_radius        = _ae.f_radius;
        go_particles    = _go_particles;
    }

    private void Explode()
    {
        // Turn on the hitbox
        Collider[] _cA = Physics.OverlapSphere(transform.position, f_radius, lm_blastMask);

        // Play the particle effect
        for (int i = 0; i < go_particles.Length; i++)
            PoolManager.x.SpawnObject(go_particles[i], transform);

        // Check for damage
        for (int i = 0; i < _cA.Length; i++)
        {
            IHitable _i = _cA[i].GetComponent<IHitable>();
            if (_i != null)
            {
                // Lodes don't take knockback
                if (_i is LodeBase)
                {
                    _i.TakeDamage(i_lodeDamage, true);
                }
                // Everything else does
                else
                {
                    _i.TakeDamage(i_damage, true);
                    _cA[i].GetComponent<Rigidbody>()?.AddExplosionForce(f_knockBack, transform.position, f_radius, -1);
                }
            }
        }
        StopAllCoroutines();
    }

    private IEnumerator DelayExposion()
    {
        yield return new WaitForSeconds(f_detonationTime);
        Explode();
    }
}
