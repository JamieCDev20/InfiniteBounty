using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviourPunCallbacks, IHitable
{

    [SerializeField] private int i_maxHealth = 10;

    private int i_currentHealth;

    public void TakeDamage(int damage)
    {

        i_currentHealth -= damage;

        if(i_currentHealth <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("LobbyScene");

        GameObject go = new GameObject("Sacrificial Lamb");
        DontDestroyOnLoad(go);

        foreach (var root in go.scene.GetRootGameObjects())
            Destroy(root);

    }

}
