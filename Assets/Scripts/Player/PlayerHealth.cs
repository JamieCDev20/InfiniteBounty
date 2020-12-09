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
    [SerializeField] private GameObject go_reviveObject;
    [SerializeField] private AudioClip[] acA_hurtClips;
    private CameraController cc_cam;
    public CameraController Cam { set { cc_cam = value; } }

    private bool isDead = false;
    private bool b_canBeRevived = false;
    private float i_currentHealth;
    private float f_currentCount;
    private bool b_canRegen = true;
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
        if (!view.IsMine || isDead)
            return;
        if (acA_hurtClips.Length > 0)
            AudioSource.PlayClipAtPoint(acA_hurtClips[acA_hurtClips.Length], transform.position);
        i_currentHealth = Mathf.Clamp(i_currentHealth - damage, -1, i_maxHealth);
        hudControl?.SetHealthBarValue(i_currentHealth, i_maxHealth);

        if (i_currentHealth <= 0)
        {
            ClientDie();
        }
        b_canRegen = false;
        f_currentCount = f_afterHitRegenTime;

    }

    [PunRPC]
    public void SetMaxHealth()
    {
        i_currentHealth = i_maxHealth;
        hudControl?.SetHealthBarValue(i_currentHealth, i_maxHealth);
    }

    [PunRPC]
    public void Respawn()
    {
        SetMaxHealth();
        GetComponent<Rigidbody>().isKinematic = false;
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<PlayerMover>().enabled = true;
        GetComponent<ToolHandler>().enabled = true;

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
        b_canBeRevived = true;
        go_reviveObject.SetActive(true);
        NetworkManager.x.PlayerDied();
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

}
