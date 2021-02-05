using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviourPun, IHitable
{
    private BossAI boss;
    [SerializeField] private int i_maxHealth;
    private int i_currentHealth;
    [SerializeField] private GameObject go_deathParticles;
    [SerializeField] private RectTransform rt_healthBar;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);

        boss = GetComponent<BossAI>();
        i_maxHealth *= PhotonNetwork.CurrentRoom.PlayerCount;

        i_currentHealth = i_maxHealth;
        rt_healthBar.localScale = new Vector3(Mathf.Clamp((float)i_currentHealth / i_maxHealth, 0, Mathf.Infinity), 1, 1);
    }

    [PunRPC]
    public void Die()
    {
        go_deathParticles.SetActive(true);
        go_deathParticles.transform.parent = null;
        Invoke(nameof(ActualDie), 1);
    }
    private void ActualDie()
    {
        gameObject.SetActive(false);
    }

    public bool IsDead()
    {
        return !gameObject.activeInHierarchy;
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, i_currentHealth - damage);
    }
    [PunRPC]
    private void TakeDamageRPC(int _i_newHealth)
    {
        i_currentHealth = _i_newHealth;
        if (i_currentHealth <= 0)
            photonView.RPC(nameof(Die), RpcTarget.All);
        rt_healthBar.localScale = new Vector3(Mathf.Clamp((float)i_currentHealth / i_maxHealth, 0, Mathf.Infinity), 1, 1);
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay) { }

}
