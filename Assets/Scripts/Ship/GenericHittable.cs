using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenericHittable : MonoBehaviour, IHitable
{
    [SerializeField] private bool b_isEnemy;
    [Space]
    [SerializeField] private int i_maxHealth;
    private int i_currentHealth;
    [SerializeField] private TextMeshPro tmp_damageText;
    [SerializeField] private Transform t_healthBarObject;
    [Space]
    [SerializeField] private ParticleSystem p_hitEffect;
    [SerializeField] private ParticleSystem p_deathEffect;
    [Space]
    [SerializeField] private MeshRenderer[] mrA_visuals = new MeshRenderer[0];
    [SerializeField] private SkinnedMeshRenderer[] mrA_enemyVisuals = new SkinnedMeshRenderer[0];
    private int i_currentLode;
    private bool b_canBeHit;
    [SerializeField] private string[] sA_lodeNames = new string[0];
    [SerializeField] private string s_loadingMessage;
    [Space]
    [SerializeField] private ParticleSystem p_creationParticles;

    [Header("Audio")]
    [SerializeField] private AudioClip ac_damageSound;
    [SerializeField] private AudioClip ac_loadingSound;
    private AudioSource as_source;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        //tmp_damageText.text = "";        
        StartCoroutine(NewLode());
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        if (b_canBeHit)
        {
            print($"OUCH , I'VE TAKEN {damage} DMG.");

            if (ac_damageSound != null)
                as_source.PlayOneShot(ac_damageSound);
            p_hitEffect.Play();
            i_currentHealth -= damage;
            t_healthBarObject.transform.localScale = new Vector3(1, 1, (float)i_currentHealth / i_maxHealth);

            if (i_currentHealth <= 0)
                Die();
        }
    }

    public void Die()
    {
        p_deathEffect.Play();

        if (b_isEnemy)
            mrA_enemyVisuals[i_currentLode].material.SetFloat("Visibility", 1);
        else
            mrA_visuals[i_currentLode].material.SetFloat("Visibility", 1);

        StartCoroutine(NewLode());

        TutorialManager.x.ThingDestroyed(b_isEnemy);
    }

    private IEnumerator NewLode()
    {
        if (b_isEnemy)
            i_currentLode = Random.Range(0, mrA_enemyVisuals.Length);
        else
            i_currentLode = Random.Range(0, mrA_visuals.Length);

        t_healthBarObject.transform.localScale = new Vector3(1, 1, 0);
        b_canBeHit = false;
        i_currentHealth = i_maxHealth;
        tmp_damageText.text = s_loadingMessage;
        yield return new WaitForSeconds(0.5f);

        as_source.PlayOneShot(ac_loadingSound);
        p_creationParticles.Play();
        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            if (b_isEnemy)
                mrA_enemyVisuals[i_currentLode].material.SetFloat("Visibility", 1 - (i * 0.02f));
            else
                mrA_visuals[i_currentLode].material.SetFloat("Visibility", 1 - (i * 0.02f));
            t_healthBarObject.transform.localScale = new Vector3(1, 1, (float)i / 100);
        }
        tmp_damageText.text = sA_lodeNames[i_currentLode];

        if (b_isEnemy)
            mrA_enemyVisuals[i_currentLode].material.SetFloat("Visibility", -1);
        else
            mrA_visuals[i_currentLode].material.SetFloat("Visibility", -1);

        t_healthBarObject.transform.localScale = new Vector3(1, 1, 1);

        b_canBeHit = true;
    }

    public bool IsDead() { return false; }

    public void TakeDamage(int damage, bool activatesThunder, float delay) { }
}
