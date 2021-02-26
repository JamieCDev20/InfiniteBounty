
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
    [SerializeField] private GameObject go_startPortal;
    [SerializeField] private GameObject go_endPortal;
    [SerializeField] private float f_portalLifeSpawn;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!b_isActive)
        {
            f_coolDown -= Time.deltaTime;
            if (f_coolDown < 0) ComeOffCooldown();
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
        if (b_isActive)
        {
            RaycastHit[] _hits;
            _hits = Physics.RaycastAll(transform.position, _v_lookDirection, Mathf.Infinity, LayerMask.NameToLayer("Ignore Raycasts"), QueryTriggerInteraction.Ignore);

            for (int i = 0; i < _hits.Length; i++)
                if (_hits[i].transform.CompareTag("Player"))
                {
                    OpenPortalAtPoint(_hits[i].point, _v_lookDirection);
                    PlayAudio(ac_activationSound);
                    BeginCooldown();
                    return;
                }
            if (_hits.Length > 0)
            {
                if (Vector3.Distance(transform.position, _hits[0].point) <= f_teleportDistance)
                {
                    OpenPortalAtPoint(_hits[0].point, _v_lookDirection);
                    PlayAudio(ac_activationSound);
                    BeginCooldown();
                }
            }
            else
            {
                OpenPortalAtPoint(transform.position + transform.forward * f_teleportDistance, _v_lookDirection);
                PlayAudio(ac_activationSound);
                BeginCooldown();

            }
        }
    }

    private void OpenPortalAtPoint(Vector3 _v_pos, Vector3 _v_lookDirection)
    {
        go_startPortal.transform.position = transform.position + _v_lookDirection * 5;
        go_startPortal.SetActive(true);
        go_startPortal.transform.parent = null;
        DontDestroyOnLoad(go_startPortal);
        go_startPortal.GetComponent<Teleportal>().Setup(f_portalLifeSpawn);

        go_endPortal.transform.position = _v_pos - _v_lookDirection * 3;
        go_endPortal.SetActive(true);
        go_endPortal.transform.parent = null;
        DontDestroyOnLoad(go_endPortal);
        go_endPortal.GetComponent<Teleportal>().Setup(f_portalLifeSpawn);

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
