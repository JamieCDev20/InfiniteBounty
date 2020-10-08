using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] private int i_maxHealth;
    private int i_currentHealth;


    private void Start()
    {
        i_currentHealth = i_maxHealth;
    }

    internal void TakeDamage(int _i_damage)
    {
        print("Ouch!");
        i_currentHealth -= _i_damage;
        if (i_currentHealth <= 0) Death();
    }

    private void Death()
    {
        gameObject.SetActive(false);

    }
}
