using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandymanHealth : MonoBehaviourPun, IHitable
{

    [SerializeField] private int i_maxHealth = 300;

    private int i_curHealth;
    [SerializeField] private GameObject go_deathParticles;

    private void OnEnable()
    {
        DifficultySet _ds = DifficultyManager.x.ReturnCurrentDifficulty();
        i_maxHealth = Mathf.RoundToInt(i_maxHealth * _ds.f_maxHealthMult);
        transform.localScale = Vector3.one * _ds.f_scaleMult;

        if (DiversifierManager.x.ReturnIfDiverIsActive(Diversifier.MiniMiniboss))
        {
            transform.localScale *= 0.75f;
            i_maxHealth = Mathf.RoundToInt(i_maxHealth * 0.75f);
        }
        else if (DiversifierManager.x.ReturnIfDiverIsActive(Diversifier.Maxiboss))
        {
            transform.localScale *= 1.5f;
            i_maxHealth = Mathf.RoundToInt(i_maxHealth * 1.5f);
        }

        i_curHealth = i_maxHealth;
        Invoke(nameof(TimedDeath), Random.Range(90, 130));

    }

    private void TimedDeath()
    {
        TakeDamage(10000, true);
    }

    public void Die()
    {
        GetComponentInChildren<Animator>().SetTrigger("Death");
        GetComponentInChildren<Animator>().SetBool("WakeUp", false);
        GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(DelayedDeath());
    }

    IEnumerator DelayedDeath()
    {
        GetComponent<Collider>().isTrigger = true;
        yield return new WaitForSeconds(2.8f);
        GetComponentInChildren<Animator>().SetBool("WakeUp", true);
        go_deathParticles.transform.parent = null;
        go_deathParticles.SetActive(true);
        PhotonNetwork.Destroy(gameObject);
        if (photonView.IsMine)
            EnemySpawner.x?.EnemyDied(true);
    }

    public bool IsDead()
    {
        return true;
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            MasterTakeDamage(damage, activatesThunder);
            return;
        }
        photonView.RPC("RemoteTakeDamage", RpcTarget.Others, damage, activatesThunder);
    }

    private void MasterTakeDamage(int damage, bool activatesThunder)
    {
        i_curHealth -= damage;
        if (i_curHealth <= 0)
            Die();

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
