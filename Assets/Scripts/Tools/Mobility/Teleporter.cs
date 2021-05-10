
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MobilityTool
{
    [Header("Teleport Info")]
    [SerializeField] private float f_teleportDistance;
    private bool b_isActive;
    private float f_coolDown;
    [SerializeField] private float f_teleportDelay;
    [Space, SerializeField] private GameObject go_chargeEffects;
    [SerializeField] private Material redMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Renderer lightRenderer;
    private AudioSource as_source;

    [Header("Portals")]
    [SerializeField] private Teleportal go_portalPrefab;
    [SerializeField] private float f_portalLifeSpawn;


    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        if (GetComponentInParent<PlayerInputManager>() != null)
            photonView.ViewID += 26780 + GetComponentInParent<PlayerInputManager>().GetID() * 2 + 1;
        PhotonNetwork.RegisterPhotonView(photonView);
        t_cam = Camera.main.transform;
    }

    private void Update()
    {
        HUDController.x.HideTeleportSign();
        if (!b_isActive)
        {
            f_coolDown -= Time.deltaTime;
            if (f_coolDown < 0) ComeOffCooldown();
        }
        if (b_isActive)
            if(photonView.IsMine)
            foreach (GameObject player in TagManager.x.GetTagSet("Player"))
                if (Vector3.Angle(player.transform.position - transform.position, t_cam.transform.forward) < 5)
                {
                    HUDController.x.ShowTeleportSign();
                    return;
                }
    }

    public override void Use(Vector3 _v_lookDirection)
    {
        if (b_isActive)
        {
            DoTheTeleport(_v_lookDirection);
        }
    }

    private void DoTheTeleport(Vector3 _v_lookDirection)
    {
        foreach (GameObject player in TagManager.x.GetTagSet("Player"))
            if (Vector3.Angle(player.transform.position - transform.position, _v_lookDirection) < 5)
            {
                photonView.RPC(nameof(OpenPortalAtPoint), RpcTarget.All, player.transform.position, _v_lookDirection);
                PlayAudio(ac_activationSound);
                BeginCooldown();
                return;
            }

        if (b_isActive)
        {
            RaycastHit[] _hits;
            _hits = Physics.RaycastAll(transform.position, _v_lookDirection, Mathf.Infinity, LayerMask.NameToLayer("Ignore Raycasts"), QueryTriggerInteraction.Ignore);

            for (int i = 0; i < _hits.Length; i++)
                if (_hits[i].transform.CompareTag("Player"))
                {
                    //Open near the player you targetted
                    photonView.RPC(nameof(OpenPortalAtPoint), RpcTarget.All, _hits[i].point, _v_lookDirection);
                    PlayAudio(ac_activationSound);
                    BeginCooldown();
                    return;
                }
            if (_hits.Length > 0)
            {
                //Open at the point you targetted
                if (Vector3.Distance(transform.position, _hits[0].point) <= f_teleportDistance)
                {
                    photonView.RPC(nameof(OpenPortalAtPoint), RpcTarget.All, _hits[0].point, _v_lookDirection);
                    PlayAudio(ac_activationSound);
                    BeginCooldown();
                }
                else
                {
                    //Open in mid-air, at the maximum distance
                    photonView.RPC(nameof(OpenPortalAtPoint), RpcTarget.All, transform.position + _v_lookDirection * f_teleportDistance, _v_lookDirection);
                    PlayAudio(ac_activationSound);
                    BeginCooldown();
                }
            }
            else
            {
                //Open in mid-air, at the maximum distance
                photonView.RPC(nameof(OpenPortalAtPoint), RpcTarget.All, transform.position + _v_lookDirection * f_teleportDistance, _v_lookDirection);
                PlayAudio(ac_activationSound);
                BeginCooldown();
            }
        }
    }

    [PunRPC]
    public void OpenPortalAtPoint(Vector3 _v_pos, Vector3 _v_lookDirection)
    {
        Teleportal _tp_startPortal = Instantiate(go_portalPrefab);
        Teleportal _tp_endPortal = Instantiate(go_portalPrefab);

        _tp_startPortal.transform.position = transform.position + _v_lookDirection * 5 + Vector3.up;
        _tp_startPortal.transform.parent = null;
        _tp_startPortal.transform.forward = _v_lookDirection;
        _tp_startPortal.GetComponent<Teleportal>().Setup(f_portalLifeSpawn, _tp_endPortal);

        _tp_endPortal.transform.position = _v_pos - _v_lookDirection * 3 + Vector3.up;
        _tp_endPortal.transform.parent = null;
        _tp_endPortal.transform.forward = -_v_lookDirection;
        _tp_endPortal.GetComponent<Teleportal>().Setup(f_portalLifeSpawn, _tp_startPortal);

    }


    private void ComeOffCooldown()
    {
        go_chargeEffects.SetActive(true);
        lightRenderer.material = greenMat;
        b_isActive = true;

        PlayAudio(ac_hitSound);
    }

    private void BeginCooldown()
    {
        go_chargeEffects.SetActive(false);
        b_isActive = false;
        f_coolDown = f_timeBetweenUsage;
    }

    public override void PlayParticles(bool val) { }

}
