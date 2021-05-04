using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableLode : Throwable
{
    [SerializeField] private float f_explodeRadius;
    private bool b_thrown;
    [SerializeField] private int i_damage;
    private int i_actualDamage;
    [SerializeField] private float f_knockbackForce;
    [SerializeField] private GameObject go_explodeParticles;

    private void Start()
    {
        i_actualDamage = Mathf.RoundToInt(i_damage * DifficultyManager.x.ReturnCurrentDifficulty().f_damageMult);
    }

    internal override void EnterAboutToBeThrownState()
    {
        b_thrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (b_thrown)
            Explode();
    }

    private void Explode()
    {
        go_explodeParticles.SetActive(true);
        go_explodeParticles.transform.parent = null;

        foreach (Collider item in Physics.OverlapSphere(transform.position, f_explodeRadius))
        {
            IHitable hit = item.GetComponent<IHitable>();

            if (hit != null)
                hit.TakeDamage(i_actualDamage, true);

            if (item.tag == "Player")
                item.GetComponent<PlayerMover>().HitKnockback((item.transform.position - transform.position).normalized, f_knockbackForce);
        }

        gameObject.SetActive(false);
    }
}
