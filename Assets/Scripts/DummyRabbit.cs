using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRabbit : MonoBehaviour, IHitable
{

    [SerializeField] private GameObject deathParticles;

    public void Die()
    {
        Destroy(gameObject);
        if(deathParticles != null)
        {
            deathParticles.SetActive(false);
            deathParticles.SetActive(true);
            deathParticles.transform.SetParent(null);
            Destroy(deathParticles, 5);

        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Die();
        }
    }

    public bool IsDead()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        Die();
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {
        Die();
    }
}
