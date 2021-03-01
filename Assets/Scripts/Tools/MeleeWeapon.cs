using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponTool
{
    [Header("Melee Things")]
    [SerializeField] private float f_hitBoxActivationDelay;
    [SerializeField] private float f_hitBoxDur;
    private HammerHitbox hitbox;

    private void Start()
    {
        hitbox = GetComponentInChildren<HammerHitbox>();
    }

    public override void Use(Vector3 _v_forward)
    {
        hitbox.Setup(i_damage, f_knockback, i_lodeDamage);
        StartCoroutine(HitBoxControl());
        //base.Use();
    }

    private IEnumerator HitBoxControl()
    {
        yield return new WaitForSeconds(f_hitBoxActivationDelay);
        hitbox.SetHitBoxActive(true);
        yield return new WaitForSeconds(f_hitBoxDur);
        hitbox.SetHitBoxActive(false);
    }

}