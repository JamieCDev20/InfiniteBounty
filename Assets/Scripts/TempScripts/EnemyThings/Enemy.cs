using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Fields")]
    [SerializeField] protected int i_maxHealth;
    protected int i_currentHealth;
    protected bool b_isHunting;

    protected virtual void Start()
    {
        i_currentHealth = i_maxHealth;
    }

    internal virtual void Begin()
    {
        b_isHunting = true;
    }

    internal virtual void TakeDamage(int _i_damage)
    {
        i_currentHealth -= _i_damage;
        if (i_currentHealth <= 0) Death();
    }

    internal virtual void Death()
    {
        gameObject.SetActive(false);
    }

    /*
    private void BeamBackToShip()
    {
        for (int i = 0; i < LocationController.x.goA_playerObjects.Length; i++)
            LocationController.x.goA_playerObjects[i].transform.position = new Vector3(0, 0, i * 1.5f);
        LocationController.x.UnloadArea();
    }
    */
}
