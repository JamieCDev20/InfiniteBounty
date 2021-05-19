using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRabbit : MonoBehaviour, IHitable
{

    [SerializeField] private GameObject deathParticles;
    [SerializeField] private AudioClip explosionClip;

    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Die()
    {
        if(deathParticles != null)
        {
            deathParticles = Instantiate(deathParticles, transform.position, Quaternion.identity);
            deathParticles.SetActive(false);
            deathParticles.SetActive(true);
            deathParticles.transform.SetParent(null);
            Destroy(deathParticles, 5);
            
        }
        if(source != null && explosionClip != null)
        {
            source.clip = explosionClip;
            source.Play();
        }
        Destroy(gameObject);
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

    public bool IsNug()
    {
        return false;
    }
}
