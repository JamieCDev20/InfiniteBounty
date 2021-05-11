
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

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        if (GetComponentInParent<PlayerInputManager>() == null)
        {
            go_chargeEffects.SetActive(false);
        }

        t_cam = Camera.main.transform;
    }

    private void OnDisable()
    {
        if (GetComponentInParent<PlayerInputManager>() == null)
        {
            go_chargeEffects.SetActive(false);
        }
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
            if (photonView.IsMine)
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
                NetworkManager.x.TeleportalSpawn(transform.position, player.transform.position, _v_lookDirection);
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
                    NetworkManager.x.TeleportalSpawn(transform.position, _hits[i].point, _v_lookDirection);
                    PlayAudio(ac_activationSound);
                    BeginCooldown();
                    return;
                }
            if (_hits.Length > 0)
            {
                //Open at the point you targetted
                if (Vector3.Distance(transform.position, _hits[0].point) <= f_teleportDistance)
                {
                    NetworkManager.x.TeleportalSpawn(transform.position, _hits[0].point, _v_lookDirection);
                    PlayAudio(ac_activationSound);
                    BeginCooldown();
                }
                else
                {
                    //Open in mid-air, at the maximum distance
                    NetworkManager.x.TeleportalSpawn(transform.position, transform.position + _v_lookDirection * f_teleportDistance, _v_lookDirection);
                    PlayAudio(ac_activationSound);
                    BeginCooldown();
                }
            }
            else
            {
                //Open in mid-air, at the maximum distance
                NetworkManager.x.TeleportalSpawn(transform.position, transform.position + _v_lookDirection * f_teleportDistance, _v_lookDirection);
                PlayAudio(ac_activationSound);
                BeginCooldown();
            }
        }
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
