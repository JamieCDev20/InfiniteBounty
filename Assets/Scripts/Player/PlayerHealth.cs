using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviourPunCallbacks, IHitable
{

    [SerializeField] private int i_maxHealth = 10;

    private int i_currentHealth;
    private PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

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
        if (!view.IsMine)
            return;
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("LobbyScene");

        GameObject go = new GameObject("Sacrificial Lamb");
        DontDestroyOnLoad(go);

        foreach (var root in go.scene.GetRootGameObjects())
            Destroy(root);

    }

}
