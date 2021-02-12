using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandymanHealth : MonoBehaviour, IHitable
{

    [SerializeField] private int i_maxHealth = 300;
    
    private int i_curHealth;

    private void OnEnable()
    {
        i_curHealth = i_maxHealth;
    }

    public void Die()
    {
        Destroy(gameObject);

    }

    public bool IsDead()
    {
        return true;
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        i_curHealth -= damage;
        if (i_curHealth <= 0)
            Die();
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {

    }
}
