using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviourPunCallbacks, IHitable
{

    [SerializeField] private int i_maxHealth = 10;
    [SerializeField] private float f_healthPerSecond = 0.5f;
    [SerializeField] private float f_downTime = 20;
    [SerializeField] private float f_afterHitRegenTime = 5;
    [SerializeField] private GameObject go_reviveObject;
    [SerializeField] private AudioClip[] acA_hurtClips;
    private CameraController cc_cam;
    public CameraController Cam { set { cc_cam = value; } }

    private bool isDead = false;
    private bool b_canBeRevived = false;
    private float f_currentHealth;
    private float f_downHealth;
    private float f_currentCount;
    private bool b_canRegen = true;
    private bool b_downed = false;
    private int playerID;
    private PhotonView view;
    internal HUDController hudControl;
    private PlayerAnimator pa_anim;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        SetMaxHealth();
    }


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //    TakeDamage(10000);
        if(b_downed)
        {
            f_downHealth -= Time.deltaTime;
            if(f_downHealth <= 0)
            {
                ClientFullDie();
            }
        }
        if (b_canRegen)
        {
            f_currentHealth = Mathf.Clamp(f_currentHealth + (f_healthPerSecond * Time.deltaTime), 0, i_maxHealth);
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
        if (!view.IsMine || isDead)
            return;
        //print(damage + " DMG taken");

        if (acA_hurtClips.Length > 0)
            AudioSource.PlayClipAtPoint(acA_hurtClips[acA_hurtClips.Length], transform.position);
        f_currentHealth = Mathf.Clamp(f_currentHealth - damage, -1, i_maxHealth);
        hudControl?.SetHealthBarValue(f_currentHealth, i_maxHealth);

        if (f_currentHealth <= 0)
        {
            ClientDie();
        }
        b_canRegen = false;
        f_currentCount = f_afterHitRegenTime;

    }

    [PunRPC]
    public void SetMaxHealth()
    {
        f_currentHealth = i_maxHealth;
        f_downHealth = f_downTime;
        hudControl?.SetHealthBarValue(f_currentHealth, i_maxHealth);
    }

    [PunRPC]
    public void Respawn()
    {
        SetMaxHealth();
        isDead = false;
        b_downed = false;
        ToggleAlive(true);

    }

    public void FullRespawn()
    {
        cc_cam.SetFollow(transform);
        view.RPC("Respawn", RpcTarget.All);
    }

    public void ToggleAlive(bool val)
    {
        GetComponent<Rigidbody>().isKinematic = !val;
        transform.GetChild(0).gameObject.SetActive(val);
        GetComponent<PlayerMover>().enabled = val;
        GetComponent<ToolHandler>().enabled = val;
    }

    private void ClientDie()
    {
        view.RPC("RemoteDie", RpcTarget.All);
        pa_anim.PlayerDied();
    }

    [PunRPC]
    public void RemoteDie()
    {
        isDead = true;
        b_downed = true;
        b_canBeRevived = true;
        go_reviveObject.SetActive(true);
        StartCoroutine(DeathDelay());
    }

    [PunRPC]
    private void GetRevived()
    {
        if (NetworkedPlayer.x.PlayerID == playerID)
            ClientRevive();
    }

    private void ClientRevive()
    {
        view.RPC("RemoteRevive", RpcTarget.All);
    }

    [PunRPC]
    public void RemoteRevive()
    {
        isDead = false;
        b_canBeRevived = false;
        pa_anim?.PlayerRevived();
        go_reviveObject.SetActive(false);
        SetMaxHealth();

    }

    [PunRPC]
    private void DoRevive()
    {
        if (NetworkedPlayer.x.PlayerID == playerID)
            pa_anim.DoReviveAnim();
    }

    public void SetAnimator(PlayerAnimator anim)
    {
        pa_anim = anim;
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public void SetID(int id)
    {
        playerID = id;
    }

    public void ClientFullDie()
    {
        view.RPC("RemoteFullDie", RpcTarget.All);
        b_downed = false;
        foreach (PlayerHealth ph in FindObjectsOfType<PlayerHealth>())
        {
            if (!ph.isDead)
            {
                cc_cam.SetFollow(ph.transform);
            }
        }
    }

    [PunRPC]
    public void RemoteFullDie()
    {
        ToggleAlive(false);
        b_downed = false;
    }

    public IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(2);
        NetworkManager.x.PlayerDied();

    }

}
