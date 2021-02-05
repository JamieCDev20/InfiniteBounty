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
    private float f_maxDownTime;
    [SerializeField] private float f_afterHitRegenTime = 5;
    [SerializeField] private GameObject go_reviveObject;
    [SerializeField] private AudioClip[] acA_hurtClips;
    [SerializeField] private bool Damageable = true;
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
    [Space, SerializeField] private RectTransform rt_downedTimer;
    private PlayerInputManager pim;
    [SerializeField] private GameObject go_reviveSymbol;
    [SerializeField] private ParticleSystem ps_burningBumParticles;

    [Header("Sound FX")]
    [SerializeField] private AudioClip ac_slowHeartBeat;
    [SerializeField] private AudioClip ac_fastHeartBeat;
    [SerializeField] private AudioSource as_heartBeatSource;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        SetMaxHealth();
        pim = GetComponent<PlayerInputManager>();
    }


    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Delete))
            TakeDamage(1, false);
#endif
        if (b_downed)
        {
            f_downHealth -= Time.deltaTime;
            rt_downedTimer.localScale = new Vector3((float)(f_downHealth / f_maxDownTime), 1, 1);
            if (f_downHealth <= 0)
            {
                ClientFullDie();
            }
        }
        if (b_canRegen)
        {
            if (!b_downed)
            {
                f_currentHealth = Mathf.Clamp(f_currentHealth + (f_healthPerSecond * Time.deltaTime), 0, i_maxHealth);
                hudControl?.SetHealthBarValue(f_currentHealth, i_maxHealth);
            }
        }
        else
        {
            f_currentCount -= Time.deltaTime;
            if (f_currentCount <= 0)
                b_canRegen = true;
        }

        CheckSound();
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
#if UNITY_EDITOR
        if (!Damageable)
            return;
#endif
        if (!view.IsMine || isDead)
            return;
        //print(damage + " DMG taken");

        if (acA_hurtClips.Length > 0 && damage != 0)
            AudioSource.PlayClipAtPoint(acA_hurtClips[Random.Range(0, acA_hurtClips.Length)], transform.position);
        f_currentHealth = Mathf.Clamp(f_currentHealth - damage, -1, i_maxHealth);
        hudControl?.SetHealthBarValue(f_currentHealth, i_maxHealth);

        if (f_currentHealth <= 0)
        {
            ClientDie();
        }
        b_canRegen = false;
        f_currentCount = f_afterHitRegenTime;
    }

    private void CheckSound()
    {
        if (f_currentHealth < (i_maxHealth * 0.2f))
        {
            as_heartBeatSource.clip = ac_fastHeartBeat;
            as_heartBeatSource.Play();
        }
        else if (f_currentHealth < (i_maxHealth * 0.5f))
        {
            as_heartBeatSource.clip = ac_slowHeartBeat;
            as_heartBeatSource.Play();
        }
        else
            as_heartBeatSource.Stop();
    }

    public void TakeDamage(int damage, bool activatesThunder, float _delay)
    {
        StartCoroutine(DelayedTakeDamage(damage, activatesThunder, _delay));
    }

    IEnumerator DelayedTakeDamage(int damage, bool activatesThunder, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        TakeDamage(damage, activatesThunder);
    }

    public void Die()
    {

    }

    [PunRPC]
    public void SetMaxHealth()
    {
        f_currentHealth = i_maxHealth;
        f_downHealth = f_downTime;
        f_maxDownTime = f_downTime;
        hudControl?.SetHealthBarValue(f_currentHealth, i_maxHealth);
        cc_cam?.StopSpectating();
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
        transform.parent = null;
        isDead = true;
        b_downed = true;
        b_canBeRevived = true;
        go_reviveObject.SetActive(true);

        if (pim?.GetID() == NetworkedPlayer.x?.PlayerID)
            go_reviveSymbol.SetActive(false);

        GetComponent<Collider>().enabled = false;
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
        GetComponent<Collider>().enabled = true;
        isDead = false;
        b_downed = false;
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
                cc_cam.SetFollow(ph.transform, true);
            }
        }

        PvPManager.x?.PlayerDied();
    }

    [PunRPC]
    public void RemoteFullDie()
    {
        ToggleAlive(false);
        b_downed = false;
        transform.parent = null;
    }

    public IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(2);
        NetworkManager.x.PlayerDied();

    }

    public bool IsDead()
    {
        return b_downed || isDead;
    }

    public void StartBurningBum(Vector3 _v_bounceDirection, bool _b_shouldCatchFire)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;//new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(_v_bounceDirection);

        if (_b_shouldCatchFire)
        {
            ps_burningBumParticles.Play();
            Invoke("StopParticles", 1);
        }
    }
    private void StopParticles()
    {
        ps_burningBumParticles.Stop();
    }

}
