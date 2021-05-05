using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrooberHealth : MonoBehaviour, IHitable
{
    [SerializeField] private int i_maxHealth;
    private int i_currentHealth;
    [SerializeField] private GameObject go_deathParticles;
    [SerializeField] private ParticleSystem ps_hitParticles;
    private float f_deathTimer;

    public void Die()
    {
        gameObject.SetActive(false);
        go_deathParticles.transform.parent = null;
        go_deathParticles.SetActive(true);
    }

    private void Update()
    {
        f_deathTimer += Time.deltaTime;
        if (f_deathTimer > 60)
            Die();
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.Delete))
            TakeDamage(1000, false);

#endif
    }

    public bool IsDead()
    {
        return false;
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        i_currentHealth -= damage;

        ps_hitParticles.Play();
        if (i_currentHealth <= 0)
            Die();

        EnemySpawner.x?.EnemyDied(false);
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {
        TakeDamage(damage, activatesThunder);
    }


    private void Start()
    {
        DifficultySet _ds = DifficultyManager.x.ReturnCurrentDifficulty();

        i_currentHealth = Mathf.RoundToInt(i_maxHealth * _ds.f_maxHealthMult);
        transform.localScale = (Vector3.one * 2) * _ds.f_scaleMult;
        if (DiversifierManager.x.ReturnIfDiverIsActive(Diversifier.GoofyGroobers))
        {
            transform.localScale *= 0.75f;
            i_maxHealth = Mathf.RoundToInt(i_maxHealth * 0.75f);
        }
        else if (DiversifierManager.x.ReturnIfDiverIsActive(Diversifier.NastyGroobers))
        {
            transform.localScale *= 1.5f;
            i_maxHealth = Mathf.RoundToInt(i_maxHealth * 1.5f);
        }
    }
}
