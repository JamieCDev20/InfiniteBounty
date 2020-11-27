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
    private CameraController cc_cam;
    public CameraController Cam { set { cc_cam = value; } }

    private bool isDead;
    private float i_currentHealth;
    private float f_currentCount;
    private bool b_canRegen = true;
    private PhotonView view;
    internal HUDController hudControl;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        SetMaxHealth();
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
        i_currentHealth = Mathf.Clamp(i_currentHealth - damage, -1, i_maxHealth);
        hudControl?.SetHealthBarValue(i_currentHealth, i_maxHealth);

        if (i_currentHealth <= 0)
        {
            StartCoroutine(Dieath());
        }
        b_canRegen = false;
        f_currentCount = f_afterHitRegenTime;
        
    }

    public void SetMaxHealth()
    {
        i_currentHealth = i_maxHealth;
    }

    public bool IsDead()
    {
        return i_currentHealth <= 0;
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
        //gameObject.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = true;
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<PlayerMover>().enabled = false;
        isDead = true;
        PlayerInputManager newCam = FindObjectOfType<PlayerInputManager>();
        if(newCam != null)
            cc_cam.SetFollow(newCam.transform);
        yield return new WaitForSeconds(2);
        Die();
    }

}
