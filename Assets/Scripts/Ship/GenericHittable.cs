﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenericHittable : MonoBehaviour, IHitable
{
    [SerializeField] private TextMeshPro tmp_damageText;

    private void Start()
    {
        tmp_damageText.text = "";
    }


    public void TakeDamage(int damage, bool activatesThunder)
    {
        tmp_damageText.text = damage + " DMG";


    }

    public void Die() { }

    public bool IsDead() { return false; }

    public void TakeDamage(int damage, bool activatesThunder, float delay) { }
}
