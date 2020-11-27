using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviourPunCallbacks, IHitable
{

    [SerializeField] private int i_maxHealth = 10;
    [SerializeField] private float f_healthPerSecond = 0.5f;
    [SerializeField] private float f_afterHitRegenTime = 5;

    private float i_currentHealth;
    private float f_currentCount;
    private bool b_canRegen = true;
    private PhotonView view;
    internal HUDController hudControl;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        i_currentHealth = i_maxHealth;
    }


    private void Update()
    {

        if (b_canRegen)
        {
            i_currentHealth = Mathf.Clamp(i_currentHealth + (f_healthPerSecond * Time.deltaTime), 0, i_maxHealth);
        }
        else
        {
            f_currentCount -= Time.deltaTime;
            if (f_currentCount <= 0)
                b_canRegen = true;
        }

    }

    public void TakeDamage(int damage)
    {
        if (!view.IsMine)
            return;
        i_currentHealth -= damage;
        hudControl?.SetHealthBarValue(i_currentHealth, i_maxHealth);

        if (i_currentHealth <= 0)
        {
            StartCoroutine(Dieath());
        }
        b_canRegen = false;
        f_currentCount = f_afterHitRegenTime;
        
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

    IEnumerator Dieath()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        Die();
    }

}
