﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{

    [SerializeField] private int i_damage;
    [SerializeField] private int i_lodeDamage;
    [SerializeField] private float f_lifeTime;
    // Hit effect is the explosion, or particle that plays when hitting an enemy
    [SerializeField] private GameObject go_hitEffect;
    // Spawn Item is the item that gets spawned, like a daisy or shoe
    [SerializeField] private GameObject go_spawnItem;
    [SerializeField] private LayerMask lm_placementLayer;
    [SerializeField] private bool b_isNetworkedObject = true;
    [SerializeField] private string s_resourcePath;
    [SerializeField] private Collider c_myCollider;

    [Header("TrailEffects")]
    [SerializeField] private TrailRenderer tr_bulletTrail;
    [SerializeField] private GameObject go_flameTrails;
    [SerializeField] private GameObject go_electricTrails;

    [SerializeField] private PhysicMaterial pm_mat;
    private bool b_explosive;
    private bool b_gooey;
    private bool b_soaked;
    private Rigidbody rb;

    protected bool b_inPool;
    protected int i_poolIndex;
    protected AugmentExplosion ae_explosion;
    [Space, SerializeField] private float f_minimumSpeedForSound;
    private AudioSource as_source;
    [SerializeField] private float f_knockBack = 5;
    [SerializeField] ElementalObject elements;

    public void Setup(int _i_damage, int _i_lodeDamage, Collider _c_playerCol, AugmentProjectile _ap, AugmentExplosion _ae, Element[] _elem)
    {
        as_source = GetComponent<AudioSource>();
        c_myCollider.isTrigger = true;
        i_damage = _i_damage;
        i_lodeDamage = _i_lodeDamage;
        rb = GetComponent<Rigidbody>();
        // Apply bullet augments here
        rb.mass = _ap.f_gravity;
        if (_ap.f_gravity > 0)
            transform.localScale = Vector3.one * _ap.f_bulletScale;
        else
            transform.localScale = Vector3.one;
        if (_ap.pm_phys != null)
            pm_mat = Resources.Load<PhysicMaterial>(_ap.pm_phys);
        // If there's an explosion to be had, create a hiteffect here
        ae_explosion = _ae;
        if(!Utils.ArrayIsNullOrZero(_elem))
            elements.Init(_elem);
        transform.rotation = Quaternion.identity;
        StartCoroutine(DeathTimer(f_lifeTime));
        //Invoke("BecomeCollidable", Time.deltaTime);
        BecomeCollidable(_c_playerCol);
    }

    private void Update()
    {
        if (rb.velocity.sqrMagnitude < f_minimumSpeedForSound * f_minimumSpeedForSound)
            as_source.Pause();
    }

    private void BecomeCollidable(Collider playerCollider)
    {
        Physics.IgnoreCollision(c_myCollider, playerCollider, true);
        c_myCollider.isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IHitable temp = collision.gameObject.GetComponent<IHitable>();
        switch (temp)
        {
            case NGoapAgent nga:
                nga.TakeDamage(i_damage, true);
                break;
            case LodeBase lb:
                lb.TakeDamage(i_lodeDamage, false);
                break;
            case Enemy e:
                e.TakeDamage(i_damage, false);
                break;
            case null:
                break;
            case NugGO n:
                n.TakeDamage(i_damage, true);
                break;
            case LilyPad lp:
                lp.TakeDamage(i_lodeDamage, true);
                break;
            default:
                temp.TakeDamage(i_damage, true);
                break;
        }

        if (temp != null)
        {
            ISuckable _iSuck = collision.transform.GetComponent<ISuckable>();
            if (_iSuck != null)
                _iSuck.GetRigidbody().AddForce(transform.forward * f_knockBack);
        }

        if (go_hitEffect != null)
        {
            // Apply explosion augments
            SpawnOnHit(go_hitEffect, collision.contacts[0].normal);
        }
        if (go_spawnItem != null)
            SpawnOnHit(go_spawnItem, collision.contacts[0].normal);

        if (tr_bulletTrail)
            tr_bulletTrail.gameObject.transform.parent = null;

        Die();
    }

    private void SpawnOnHit(GameObject _go_objToSpawn, Vector3 _v_direction)
    {
        GameObject spawned = PoolManager.x.SpawnObject(_go_objToSpawn);

        spawned.SetActive(false);
        spawned.transform.parent = transform;
        spawned.transform.localPosition = Vector3.zero;
        spawned.transform.forward = _v_direction;
        spawned.transform.parent = null;
        spawned.SetActive(true);
    }

    private IEnumerator ReturnToPool(GameObject _go_effect)
    {
        yield return new WaitForSeconds(2);
        PoolManager.x.ReturnObjectToPool(_go_effect);
    }

    public void MoveBullet(Vector3 _v_dir, float _f_force)
    {
        transform.rotation = Quaternion.LookRotation(_v_dir, Vector3.up);
        rb.AddForce(transform.forward * _f_force, ForceMode.VelocityChange);

        if (tr_bulletTrail)
        {
            tr_bulletTrail.transform.parent = transform;
            tr_bulletTrail.transform.localPosition = Vector3.zero;
            tr_bulletTrail.Clear();
        }
    }
    public void MoveBullet(Vector3 _v_dir, float _f_force, ForceMode fm_force)
    {
        rb.AddForce(_v_dir * _f_force, fm_force);
    }

    private IEnumerator DeathTimer(float _f_lifeTime)
    {
        yield return new WaitForEndOfFrame();
        //c_myCollider.isTrigger = false;

        yield return new WaitForSeconds(_f_lifeTime);
        Die();
    }

    #region Pooling

    public void Die()
    {
        StopAllCoroutines();
        SpawnOnHit(go_hitEffect, transform.forward);
        if (PoolManager.x != null) PoolManager.x.ReturnObjectToPool(gameObject);
        if (tr_bulletTrail != null)
            tr_bulletTrail.Clear();
        if (rb != null)
            rb.velocity = Vector3.zero;
    }


    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool IsNetworkedObject()
    {
        return b_isNetworkedObject;
    }

    public string ResourcePath()
    {
        return s_resourcePath;
    }

    #endregion
}