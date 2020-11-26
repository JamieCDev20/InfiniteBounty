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
    [SerializeField] private Rigidbody rb;
    [SerializeField] private bool b_isNetworkedObject = true;
    [SerializeField] private string s_resourcePath;

    [Header("TrailEffects")]
    [SerializeField] private TrailRenderer tr_bulletTrail;
    [SerializeField] private GameObject go_flameTrails;
    [SerializeField] private GameObject go_electricTrails;

    private bool b_explosive;
    private bool b_gooey;
    private bool b_soaked;

    protected bool b_inPool;
    protected int i_poolIndex;

    public void Setup(int _i_damage, int _i_lodeDamage)
    {
        i_damage = _i_damage;
        i_lodeDamage = _i_lodeDamage;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        StartCoroutine(DeathTimer(f_lifeTime));

    }

    private void OnCollisionEnter(Collision collision)
    {
        IHitable test = collision.gameObject.GetComponent<IHitable>();
        switch (test)
        {
            case NGoapAgent ngo:
                ngo.TakeDamage(i_damage);
                break;
            case LodeBase lb:
                lb.TakeDamage(i_lodeDamage);
                break;
            case Enemy enmy:
                enmy.TakeDamage(i_damage);
                break;
        }


        go_hitEffect.SetActive(false);
        go_hitEffect.transform.parent = transform;
        go_hitEffect.transform.localPosition = Vector3.zero;
        go_hitEffect.transform.forward = collision.contacts[0].normal;
        go_hitEffect.transform.parent = null;
        go_hitEffect.SetActive(true);

        if (tr_bulletTrail)
            tr_bulletTrail.gameObject.transform.parent = null;

        transform.LookAt(collision.transform);

        Die();
    }

    public void MoveBullet(Vector3 _v_dir, float _f_force)
    {
        transform.rotation = Quaternion.LookRotation(_v_dir, Vector3.up);
        rb.AddForce(transform.forward * _f_force, ForceMode.Impulse);

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