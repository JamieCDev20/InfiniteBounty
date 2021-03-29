using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenericHittable : MonoBehaviour, IHitable
{
    [SerializeField] private int i_maxHealth;
    private int i_currentHealth;
    [SerializeField] private TextMeshPro tmp_damageText;
    [SerializeField] private Transform t_healthBarObject;
    [Space]
    [SerializeField] private ParticleSystem p_hitEffect;
    [SerializeField] private ParticleSystem p_deathEffect;
    [Space]
    [SerializeField] private MeshRenderer[] mrA_visuals = new MeshRenderer[0];
    private int i_currentLode;
    private bool b_canBeHit;
    [SerializeField] private string[] sA_lodeNames = new string[0];
    [SerializeField]private string s_loadingMessage;

    private void Start()
    {
        //tmp_damageText.text = "";        
        StartCoroutine(NewLode());
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        if (b_canBeHit)
        {
            print($"OUCH , I'VE TAKEN {damage}.");

            p_hitEffect.Play();
            t_healthBarObject.transform.localScale = new Vector3(1, 1, (float)i_currentHealth / i_maxHealth);
            i_currentHealth -= damage;

            if (i_currentHealth <= 0)
                Die();
        }
    }

    public void Die()
    {
        p_deathEffect.Play();
        mrA_visuals[i_currentLode].material.SetFloat("Visibility", 1);
        StartCoroutine(NewLode());
    }

    private IEnumerator NewLode()
    {
        i_currentLode = Random.Range(0, mrA_visuals.Length);
        b_canBeHit = false;
        i_currentHealth = i_maxHealth;
        tmp_damageText.text = s_loadingMessage;

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            mrA_visuals[i_currentLode].material.SetFloat("Visibility", 1 - (i * 0.02f));
            t_healthBarObject.transform.localScale = new Vector3(1, 1, (float)i / 100);
        }
        tmp_damageText.text = sA_lodeNames[i_currentLode];

        mrA_visuals[i_currentLode].material.SetFloat("Visibility", -1);
        t_healthBarObject.transform.localScale = new Vector3(1, 1, 1);

        b_canBeHit = true;
    }

    public bool IsDead() { return false; }

    public void TakeDamage(int damage, bool activatesThunder, float delay) { }
}
