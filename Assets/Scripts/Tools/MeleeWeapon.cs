using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponTool
{
    [Header("Melee Things")]
    [SerializeField] private float f_hitBoxActivationDelay;
    [SerializeField] private float f_hitBoxDur;
    private HammerHitbox hitbox;
    private bool b_isActive;
    private float f_currentTime;

    private void Start()
    {
        hitbox = GetComponentInChildren<HammerHitbox>();
    }

    private void Update()
    {
        f_currentTime -= Time.deltaTime;
    }

    public override void Use(Vector3 _v_forward)
    {
        if (!b_isActive && f_currentTime < 0)
        {
            hitbox.Setup(i_damage, f_knockback, i_lodeDamage, _v_forward, eo_element);
            StartCoroutine(HitBoxControl());            
            //base.Use();
        }
    }

    private IEnumerator HitBoxControl()
    {
        b_isActive = true;
        yield return new WaitForSeconds(f_hitBoxActivationDelay);
        hitbox.SetHitBoxActive(true);
        yield return new WaitForSeconds(f_hitBoxDur);
        hitbox.SetHitBoxActive(false);
        b_isActive = false;
        f_currentTime = f_timeBetweenUsage;
    }

}