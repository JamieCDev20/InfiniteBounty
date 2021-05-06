﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingHealth : MonoBehaviourPun, IHitable
{

    [SerializeField] private int i_maxHealth = 300;
    [SerializeField] private GameObject go_deathEffect;
    private int i_curHealth;
    [SerializeField] private ParticleSystem p_damageParticles;
    private SkinnedMeshRenderer[] mA_myRenderers = new SkinnedMeshRenderer[0];

    private void OnEnable()
    {
        mA_myRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        Invoke(nameof(TimedDeath), Random.Range(90, 130));
        go_deathEffect.transform.parent = transform;
        go_deathEffect.SetActive(false);
    }

    private void Start()
    {
        DifficultySet _ds = DifficultyManager.x.ReturnCurrentDifficulty();
        i_maxHealth = Mathf.RoundToInt(i_maxHealth * _ds.f_maxHealthMult);

        transform.localScale = (Vector3.one * 2) * _ds.f_scaleMult;
        if (DiversifierManager.x.ReturnIfDiverIsActive(Diversifier.MiniFlying))
        {
            transform.localScale *= DiversifierManager.x.EnemyShrink;
            i_maxHealth = Mathf.RoundToInt(i_maxHealth * DiversifierManager.x.EnemyShrink);
        }
        else if (DiversifierManager.x.ReturnIfDiverIsActive(Diversifier.MaxiFlying))
        {
            transform.localScale *= DiversifierManager.x.EnemyGrow;
            i_maxHealth = Mathf.RoundToInt(i_maxHealth * DiversifierManager.x.EnemyGrow);
        }
        i_curHealth = i_maxHealth;
    }

    private void TimedDeath()
    {
        TakeDamage(10000, true);
    }

    private void OnDestroy()
    {
        go_deathEffect.SetActive(true);
        go_deathEffect.transform.parent = null;
    }

    public void Die()
    {

        PhotonNetwork.Destroy(gameObject);
        if (photonView.IsMine)
            EnemySpawner.x?.EnemyDied(false);
    }

    public bool IsDead()
    {
        return true;
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        p_damageParticles.Play();
        if (PhotonNetwork.IsMasterClient)
        {
            MasterTakeDamage(damage, activatesThunder);
            return;
        }
        photonView.RPC(nameof(RemoteTakeDamage), RpcTarget.Others, damage, activatesThunder);

    }


    private void MasterTakeDamage(int damage, bool activatesThunder)
    {
        i_curHealth -= damage;
        if (i_curHealth <= 0)
        {
            Die();
            return;
        }

        for (int i = 0; i < mA_myRenderers.Length; i++)
            mA_myRenderers[i].material.SetFloat("DamageFlash", 1);
        StartCoroutine(DamageFlash());
    }
    private IEnumerator DamageFlash()
    {
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < mA_myRenderers.Length; i++)
            mA_myRenderers[i].material.SetFloat("DamageFlash", 0);
    }

    [PunRPC]
    public void RemoteTakeDamage(int _dmg, bool _thun)
    {
        if (PhotonNetwork.IsMasterClient)
            MasterTakeDamage(_dmg, _thun);
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {

    }
}
