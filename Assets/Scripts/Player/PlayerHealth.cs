﻿using Photon.Pun;
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

    private float f_lavaBounceTime = 0;
    private bool isDead = false;
    private bool b_canBeRevived = false;
    private float f_currentHealth;
    private float f_downHealth;
    private float f_currentCount;
    private bool b_canRegen = true;
    private bool b_downed = false;
    private int playerID;
    private PhotonView view;
    private PlayerAnimator pa_anim;
    [Space, SerializeField] private RectTransform rt_downedTimer;
    private PlayerInputManager pim;
    private Animator anim;
    private bool IAMNOTIMPORTANT = false;

    // The outline is here, started it but stopped because I realised there are about 50 outlines on the player.... sigh. Some lines are commented out which are 
    [SerializeField] private Gradient healthGradient;

    [SerializeField] private GameObject go_reviveSymbol;
    [SerializeField] private ParticleSystem ps_burningBumParticles;

    [Header("Sound FX")]
    [SerializeField] private AudioClip ac_slowHeartBeat;
    [SerializeField] private AudioClip ac_fastHeartBeat;
    [SerializeField] private AudioSource as_heartBeatSource;
    private AudioSource as_mainAudioSource;

    [Header("Toggles")]
    [SerializeField] private GameObject[] toggles;
    private SkinnedMeshRenderer[] mA_mySkinRenderers = new SkinnedMeshRenderer[0];
    private MeshRenderer[] mA_myRenderers = new MeshRenderer[0];


    private void Start()
    {
        view = GetComponent<PhotonView>();
        SetMaxHealth();
        pim = GetComponent<PlayerInputManager>();
        anim = GetComponentInChildren<Animator>();
        as_mainAudioSource = GetComponent<AudioSource>();

        mA_mySkinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        mA_myRenderers = GetComponentsInChildren<MeshRenderer>(true);
        //print(mA_myRenderers.Length);
    }

    private void Update()
    {
        if (IAMNOTIMPORTANT)
            return;
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Delete))
            TakeDamage(1, false);
#endif
        if (NetworkedPlayer.x.GetPlayer() != transform)
            IAMNOTIMPORTANT = true;

        if (b_downed)
        {
            f_downHealth -= Time.deltaTime;
            if (rt_downedTimer != null)
                rt_downedTimer.localScale = new Vector3((float)(f_downHealth / f_maxDownTime), 1, 1);
            if (f_downHealth <= 0)
            {
                ClientFullDie();
            }
        }
        if (b_canRegen && f_currentHealth != i_maxHealth)
        {
            if (!b_downed)
            {
                f_currentHealth = Mathf.Clamp(f_currentHealth + (f_healthPerSecond * Time.deltaTime), 0, i_maxHealth);
                HUDController.x?.SetHealthBarValue(f_currentHealth, i_maxHealth);

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
        if (!view.IsMine || isDead || NetworkedPlayer.x.GetPlayer() != transform)
            return;

        if (damage >= 0)
        {
            as_mainAudioSource.PlayOneShot(acA_hurtClips[Random.Range(0, acA_hurtClips.Length)]);

            for (int i = 0; i < mA_mySkinRenderers.Length; i++)
                mA_mySkinRenderers[i].material.SetFloat("DamageFlash", 1);
            for (int i = 0; i < mA_myRenderers.Length; i++)
                mA_myRenderers[i].material.SetFloat("DamageFlash", 1);

            StartCoroutine(DamageFlash());
        }

        f_currentHealth = Mathf.Clamp(f_currentHealth - damage, -1, i_maxHealth);
        HUDController.x.SetHealthBarValue(f_currentHealth, i_maxHealth);
        //outline.Color = healthGradient.Evaluate(f_currentHealth / (float)i_maxHealth);

        if (f_currentHealth <= 0)
            ClientDie();

        b_canRegen = false;
        f_currentCount = f_afterHitRegenTime;

    }
    private IEnumerator DamageFlash()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < mA_mySkinRenderers.Length; i++)
            mA_mySkinRenderers[i].material.SetFloat("DamageFlash", 0);
        for (int i = 0; i < mA_myRenderers.Length; i++)
            mA_myRenderers[i].material.SetFloat("DamageFlash", 0);
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
        HUDController.x?.SetHealthBarValue(f_currentHealth, i_maxHealth);
        //outline.Color = healthGradient.Evaluate(f_currentHealth / (float)i_maxHealth);

        cc_cam?.StopSpectating();
    }

    [PunRPC]
    public void Respawn()
    {
        SetMaxHealth();
        isDead = false;
        b_downed = false;
        b_canBeRevived = false;
        ToggleAlive(true);
        pa_anim.PlayerRevived();
        go_reviveObject.SetActive(false);
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].SetActive(true);
        }
        transform.GetChild(0).gameObject.SetActive(true);

    }

    public void FullRespawn()
    {
        view.RPC(nameof(Respawn), RpcTarget.All);
        cc_cam?.SetFollow(transform);
    }

    public void ToggleAlive(bool val)
    {
        GetComponent<Rigidbody>().isKinematic = !val;
        //transform.GetChild(0).gameObject.SetActive(val);
        GetComponent<PlayerMover>().enabled = val;
        GetComponent<ToolHandler>().enabled = val;
        GetComponent<Collider>().enabled = true;
    }

    private void ClientDie()
    {
        view.RPC(nameof(RemoteDie), RpcTarget.All);
        pa_anim.PlayerDied();
    }

    [PunRPC]
    public void RemoteDie()
    {
        //transform.parent = null;
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
        view.RPC(nameof(RemoteRevive), RpcTarget.All);
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
        view.RPC(nameof(RemoteFullDie), RpcTarget.All);
        b_downed = false;


        foreach (PlayerHealth ph in FindObjectsOfType<PlayerHealth>())
        {
            if (!ph.isDead)
            {
                cc_cam?.SetFollow(ph.transform, true);
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
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].SetActive(false);
        }
        if (SceneManager.GetActiveScene().name.Contains("Lobby"))
        {
            FullRespawn();
            return;
        }
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
        if (Time.realtimeSinceStartup - f_lavaBounceTime < 0.5f)
            return;
        f_lavaBounceTime = Time.realtimeSinceStartup;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.Scale(rb.velocity, Vector3.one - Vector3.up);
        rb.AddForce(_v_bounceDirection);

        if (_b_shouldCatchFire)
        {
            anim.SetBool("LavaHit", true);
            ps_burningBumParticles.Play();
            Invoke("StopParticles", 1);
        }
    }
    private void StopParticles()
    {
        ps_burningBumParticles.Stop();
        anim.SetBool("LavaHit", false);
    }

    public float GetCurrentHealth()
    {
        return f_currentHealth;
    }

    public bool IsNug()
    {
        return false;
    }

}
