using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DynamicAudioManager : MonoBehaviourPun
{

    public static DynamicAudioManager x;

    [SerializeField] private AudioClip mainIntro;
    [SerializeField] private AudioClip mainLoop;
    [SerializeField] private AudioClip combatIntro;
    [SerializeField] private AudioClip combatLoop;
    [SerializeField] private AudioClip bossIntro;
    [SerializeField] private AudioClip bossLoop;

    [Space]

    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource combatSource;
    [SerializeField] private AudioSource bossSource;

    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixer combatMixer;
    [SerializeField] private AudioMixer bossMixer;

    [SerializeField] private float combatLerp = 0.3f;
    [SerializeField] private float mainLerp = 0.3f;
    [SerializeField] private float bossLerp = 0.3f;

    [SerializeField] private float postCombatLoopDelay = 10;

    private bool inCombat;
    private bool wasInCombat;
    private bool isBoss;

    private bool lerpMain = true;

    private void Awake()
    {
        x = this;
        photonView.ViewID = 232323;
        PhotonNetwork.RegisterPhotonView(photonView);
    }

    private void Start()
    {
        mainSource.clip = mainIntro;
        combatSource.clip = combatIntro;
        bossSource.clip = bossIntro;

        mainMixer.SetFloat("Volume", 0);
        mainSource.Play();
    }

    public void StartCombat()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RemoteCombat), RpcTarget.All);
    }

    [PunRPC]
    public void RemoteCombat()
    {

        inCombat = true;
        lerpMain = false;
        CancelInvoke(nameof(SetLerpMain));

        if (inCombat && wasInCombat)
            return;
        combatSource.clip = combatIntro;
        combatSource.Play();
        combatMixer.SetFloat("Volume", 0);
        wasInCombat = true;

    }

    public void EndCombat()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RemoteEndCombat), RpcTarget.All);
    }

    [PunRPC]
    public void RemoteEndCombat()
    {
        wasInCombat = false;
        inCombat = false;
        Invoke(nameof(SetLerpMain), postCombatLoopDelay);
    }

    public void StartBoss()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RemoteBossStart), RpcTarget.All);
    }

    [PunRPC]
    public void RemoteBossStart()
    {
        isBoss = true;
        bossSource.clip = bossIntro;
        bossSource.Play();
        bossMixer.SetFloat("Volume", 0);
    }

    public void EndBoss()
    {
        if (PhotonNetwork.IsMasterClient)

            photonView.RPC(nameof(RemoteBossEnd), RpcTarget.All);
    }

    [PunRPC]
    public void RemoteBossEnd()
    {
        isBoss = false;
    }

    private void SetLerpMain()
    {
        lerpMain = true;
    }

    private void Update()
    {

        float cMain;
        mainMixer.GetFloat("Volume", out cMain);

        float cCombat;
        combatMixer.GetFloat("Volume", out cCombat);

        float cBoss;
        bossMixer.GetFloat("Volume", out cBoss);

        combatMixer.SetFloat("Volume", Mathf.Lerp(cCombat, inCombat ? 0 : -50, combatLerp));
        mainMixer.SetFloat("Volume", Mathf.Lerp(cMain, lerpMain ? 0 : -50, mainLerp));
        bossMixer.SetFloat("Volume", Mathf.Lerp(cBoss, isBoss ? 0 : -50, bossLerp));

        if (!combatSource.isPlaying)
        {
            combatSource.clip = combatLoop;
            combatSource.Play();
        }

        if (!mainSource.isPlaying)
        {
            mainSource.clip = mainLoop;
            mainSource.Play();
        }

        if (isBoss)
            if (!bossSource.isPlaying)
            {
                bossSource.clip = bossLoop;
                bossSource.Play();
            }

    }

}
