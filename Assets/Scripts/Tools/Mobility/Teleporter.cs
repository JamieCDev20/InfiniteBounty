
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
    [SerializeField] private GameObject go_leavingEffect;
    [SerializeField] private GameObject go_arrivalEffect;
    [SerializeField] private Material redMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Renderer lightRenderer;


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
            go_leavingEffect.transform.parent = transform;
            go_leavingEffect.transform.localPosition = Vector3.zero;
            go_leavingEffect.SetActive(false);
            go_leavingEffect.SetActive(true);

            StartCoroutine(DoTheTeleport(_v_lookDirection));

        }
    }

    private IEnumerator DoTheTeleport(Vector3 _v_lookDirection)
    {
        if (b_isActive)
        {
            lightRenderer.material = redMat;
            yield return new WaitForSeconds(f_teleportDelay);
            go_leavingEffect.transform.parent = null;
            RaycastHit _hit;
            if (Physics.Raycast(transform.position, _v_lookDirection, out _hit, f_teleportDistance, gameObject.layer))
            {
                transform.root.position = _hit.point;
            }
            else
                transform.root.position += _v_lookDirection * f_teleportDistance;

            BeginCooldown();

            go_arrivalEffect.transform.parent = null;
            go_arrivalEffect.transform.position = transform.position;
            go_arrivalEffect.SetActive(false);
            go_arrivalEffect.SetActive(true);
        }
    }

    private void ComeOffCooldown()
    {
        go_chargeEffects.SetActive(true);
        lightRenderer.material = greenMat;
        b_isActive = true;
    }

    private void BeginCooldown()
    {
        go_chargeEffects.SetActive(false);
        b_isActive = false;
        f_coolDown = f_timeBetweenUsage;
    }

    public override void PlayParticles(bool val) { }

}
