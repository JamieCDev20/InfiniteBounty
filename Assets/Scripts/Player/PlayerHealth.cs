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
    internal HUDController hudControl;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public void TakeDamage(int damage)
    {

        i_currentHealth -= damage;

        hudControl.SetHealthBarValue(i_currentHealth, i_maxHealth);

        if (i_currentHealth <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        if (!view.IsMine)
            return;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif

    }

}
