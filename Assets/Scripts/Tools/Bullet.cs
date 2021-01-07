using System;
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
    [SerializeField] private GameObject go_hitEffect;
    [SerializeField] private LayerMask lm_placementLayer;
    [SerializeField] private bool b_isNetworkedObject = true;
    [SerializeField] private string s_resourcePath;
    [SerializeField] private Collider c_myCollider;

    [Header("TrailEffects")]
    [SerializeField] private TrailRenderer tr_bulletTrail;
    [SerializeField] private GameObject go_flameTrails;
    [SerializeField] private GameObject go_electricTrails;

    private bool b_explosive;
    private bool b_gooey;
    private bool b_soaked;
    private Rigidbody rb;

    protected bool b_inPool;
    protected int i_poolIndex;

    public void Setup(int _i_damage, int _i_lodeDamage, Collider _c_playerCol)
    {
        c_myCollider.isTrigger = true;
        i_damage = _i_damage;
        i_lodeDamage = _i_lodeDamage;
        rb = GetComponent<Rigidbody>();
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        StartCoroutine(DeathTimer(f_lifeTime));
        //Invoke("BecomeCollidable", Time.deltaTime);
        BecomeCollidable(_c_playerCol);
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
                nga.TakeDamage(i_damage);
                break;
            case LodeBase lb:
                lb.TakeDamage(i_lodeDamage);
                break;
            case Enemy e:
                e.TakeDamage(i_damage);
                break;
            default:
                break;
        }

        GameObject _go_effect = PoolManager.x.SpawnObject(go_hitEffect);

        StartCoroutine(ReturnToPool(_go_effect));

        _go_effect.SetActive(false);
        _go_effect.transform.parent = transform;
        _go_effect.transform.localPosition = Vector3.zero;
        _go_effect.transform.forward = collision.contacts[0].normal;
        _go_effect.transform.parent = null;
        _go_effect.SetActive(true);

        if (tr_bulletTrail)
            tr_bulletTrail.gameObject.transform.parent = null;

        Die();
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