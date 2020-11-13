using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{

    [SerializeField] private int i_damage;
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

    public void Setup(AugmentType[] _atA_activeAugments)
    {
        RemoveAugments(_atA_activeAugments);
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        for (int i = 0; i < _atA_activeAugments.Length; i++)
        {
            switch (_atA_activeAugments[i])
            {
                case AugmentType.Flame:
                    GetComponent<Systemic>().b_fire = true;
                    break;

                case AugmentType.Electric:
                    GetComponent<Systemic>().b_electric = true;
                    break;

                case AugmentType.Heavy:
                    GetComponent<Rigidbody>().useGravity = true;
                    break;

                case AugmentType.Size:
                    transform.localScale = transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
                    break;

                case AugmentType.Explosive:
                    b_explosive = true;
                    break;

                case AugmentType.Gooey:
                    b_gooey = true;
                    break;

                case AugmentType.Soaked:
                    b_soaked = true;
                    break;
            }
        }
        StartCoroutine(DeathTimer(f_lifeTime));
    }

    private void RemoveAugments(AugmentType[] _atA_activeAugments)
    {
        if (!_atA_activeAugments.Contains(AugmentType.Flame))
            GetComponent<Systemic>().b_fire = false;

        if (!_atA_activeAugments.Contains(AugmentType.Electric))
            GetComponent<Systemic>().b_electric = false;

        if (!_atA_activeAugments.Contains(AugmentType.Heavy))
            GetComponent<Rigidbody>().useGravity = false;

        if (!_atA_activeAugments.Contains(AugmentType.Explosive))
            b_explosive = false;

        if (!_atA_activeAugments.Contains(AugmentType.Gooey))
            b_gooey = false;

        if (!_atA_activeAugments.Contains(AugmentType.Soaked))
            b_soaked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Hittable"))
            collision.transform.GetComponent<Enemy>().TakeDamage(i_damage);

        go_hitEffect.SetActive(false);
        go_hitEffect.transform.parent = transform;
        go_hitEffect.transform.localPosition = Vector3.zero;
        go_hitEffect.transform.parent = null;        
        go_hitEffect.SetActive(true);

        transform.LookAt(collision.transform);

        if (b_explosive)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position - (transform.forward * 5), transform.forward, out _hit, 10, gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                GameObject _goo = Instantiate(LocationController.x.go_explosionPrefab, _hit.point, Quaternion.identity, _hit.transform.root);
                _goo.transform.localScale = transform.localScale;
                _goo.transform.up = _hit.normal;
            }
        }
        if (b_gooey)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position - (transform.forward * 5), transform.forward, out _hit, 10, gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                GameObject _goo = Instantiate(LocationController.x.go_gooPatchPrefab, _hit.point, Quaternion.identity, _hit.transform.root);
                _goo.transform.localScale = transform.localScale;
                _goo.transform.up = _hit.normal;
            }
        }
        if (b_soaked)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position - (transform.forward * 5), transform.forward, out _hit, 10, gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                GameObject _goo = Instantiate(LocationController.x.go_waterPuddlePrefab, _hit.point, Quaternion.identity, _hit.transform.root);
                _goo.transform.localScale = transform.localScale;
                _goo.transform.up = _hit.normal;
            }
        }

        Die();
    }

    public void MoveBullet(Vector3 _v_dir, float _f_force)
    {
        transform.forward = _v_dir;
        rb.AddForce(transform.forward * _f_force, ForceMode.Impulse);
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
        if(tr_bulletTrail != null)
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